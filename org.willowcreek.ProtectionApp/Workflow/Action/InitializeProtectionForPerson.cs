using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Workflow;
using org.willowcreek.Model.Extensions;
using org.willowcreek.ProtectionApp.Model;
using org.willowcreek.ProtectionApp.Data;
using org.willowcreek.FileManagement;
using org.willowcreek.FileManagement.Model;
using org.willowcreek.FileManagement.Data;
using org.willowcreek.ProtectionApp.Logic;

namespace org.willowcreek.ProtectionApp.Workflow.Action
{
    [ActionCategory("Protection")]
    [Description("Initialize the Protection process.")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Initialize Protection For Person")]

    //IN
    [WorkflowAttribute("Applicant", "(IN) Person who the application process will be started for.", true, "", "", 1, null, new string[] { "Rock.Field.Types.PersonFieldType" })]
    [WorkflowAttribute("Requester", "(IN) Person who started the application process.", true, "", "", 2, null, new string[] { "Rock.Field.Types.PersonFieldType" })]
    [WorkflowAttribute("Campus", "(IN) The campus attribute to save.", false, "", "", 3, null, new string[] { "Rock.Field.Types.CampusFieldType" })]
    [WorkflowAttribute("Ministry", "(IN) The ministry attribute to save.", false, "", "", 4, null, new string[] { "Rock.Field.Types.DefinedValueFieldType" })]
    [WorkflowAttribute("Send Application Form", "(IN) Boolen attribute that stores if the application should be sent.", true, "", "", 5, null, new string[] { "Rock.Field.Types.BooleanFieldType" })]
    [WorkflowAttribute("Request References", "(IN) Boolen attribute that stores if the references should be requested.", true, "", "", 6, null, new string[] { "Rock.Field.Types.BooleanFieldType" })]
    [WorkflowAttribute("Run Background Check", "(IN) Boolen attribute that stores if the background check should be run.", true, "", "", 7, null, new string[] { "Rock.Field.Types.BooleanFieldType" })]
    [WorkflowAttribute("Send Policy Acknowledgment Form", "(IN) Boolen attribute that stores if the policy acknowledgment should be sent.", true, "", "", 8, null, new string[] { "Rock.Field.Types.BooleanFieldType" })]
    [WorkflowAttribute("Workflows", "(IN) List of guid(s) of other workflows.", false, "", "", 9, null, new string[] { "Rock.Field.Types.TextFieldType" })]

    class InitializeProtectionForPerson : ActionComponent
    {
        /// <summary>
        /// Archive appropriate dates/docs based on selection from user
        /// </summary>
        /// <param name="rockContext"></param>
        /// <param name="action"></param>
        /// <param name="entity"></param>
        /// <param name="errorMessages"></param>
        /// <returns></returns>
        public override bool Execute(RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            try
            {
                var personChanges = new List<string>();

                //get context(s)
                ProtectionAppContext protContext = new ProtectionAppContext();
                FileManagementContext fmContext = new FileManagementContext();

                //get service(s)
                BinaryFileService fileService = new BinaryFileService(rockContext);
                BinaryFileTypeService bftService = new BinaryFileTypeService(rockContext);
                BinaryFileDataService bfdService = new BinaryFileDataService(rockContext);
                PersonAliasService ps = new PersonAliasService(rockContext);
                PersonService personService = new PersonService(rockContext);
                PersonDocumentService pdService = new PersonDocumentService(fmContext);

                //get global attribute cache
                var globalAttributes = GlobalAttributesCache.Read();

                //get Protection Archive File Type
                var protectionArchiveType = bftService.Queryable().FirstOrDefault(b => b.Name == "Protection Archive");

                //Get Applicant Alias
                var applicantGuidString = this.GetAttributeValue(action, "Applicant", rockContext);
                Guid applicantGuid = Guid.Parse(applicantGuidString);
                PersonAlias applicant = ps.Get(Guid.Parse(applicantGuid.ToString()));

                //Get Requester Alias
                var requesterGuidString = this.GetAttributeValue(action, "Requester", rockContext);
                Guid requesterGuid = Guid.Parse(requesterGuidString);
                PersonAlias requester = ps.Get(Guid.Parse(requesterGuid.ToString()));

                //Get Applicant
                Person personApplicant = personService.Get(applicant.Person.Guid);
                personApplicant.LoadAttributes();   //slow

                //Get Requester
                Person personRequester = personService.Get(requester.Person.Guid);
                personRequester.LoadAttributes();   //slow

                //Get Old Status
                string oldStatus = personApplicant.AttributeValues["ProtectionStatus"].Value;

                //Get Campus and Ministry
                var campusGuidString = this.GetAttributeValue(action, "Campus", rockContext);
                var ministryGuidString = this.GetAttributeValue(action, "Ministry", rockContext);

                //Archive Protection Fields if selected
                var application = Convert.ToBoolean(this.GetAttributeValue(action, "SendApplicationForm", rockContext));
                var references = Convert.ToBoolean(this.GetAttributeValue(action, "RequestReferences", rockContext));
                var backgroundCheck = Convert.ToBoolean(this.GetAttributeValue(action, "RunBackgroundCheck", rockContext));
                var policyAcknowledgement = Convert.ToBoolean(this.GetAttributeValue(action, "SendPolicyAcknowledgmentForm", rockContext));
                var protectionStatus = string.IsNullOrEmpty(personApplicant.AttributeValues["ProtectionStatus"].Value) ? new Guid() : new Guid(personApplicant.AttributeValues["ProtectionStatus"].Value);

                // Set Protection Status if necessary
                if (protectionStatus != ProtectionAppWorkflowHelper.PROTECTION_STATUS_NEEDS_REVIEW
                    && protectionStatus != ProtectionAppWorkflowHelper.PROTECTION_STATUS_DECLINED)
                {
                    //If person has all dates empty or expired, set to "Process Initiated"
                    if (ProtectionEvaluator.IsProtectionFullyExpired(rockContext, personApplicant))
                    {
                        History.EvaluateChange(personChanges, "ProtectionStatus", personApplicant.AttributeValues["ProtectionStatus"].ValueFormatted, "Process Initiated");
                        personApplicant.SetAttributeValue("ProtectionStatus", ProtectionAppWorkflowHelper.PROTECTION_STATUS_PROCESS_INITIATED);
                    }
                    //Else set it to "In Progress"
                    else
                    {
                        History.EvaluateChange(personChanges, "ProtectionStatus", personApplicant.AttributeValues["ProtectionStatus"].ValueFormatted, "In Progress");
                        personApplicant.SetAttributeValue("ProtectionStatus", ProtectionAppWorkflowHelper.PROTECTION_STATUS_IN_PROGRESS);
                    }
                }

                // The Process Initiated date needs to be updated every time a protection workflow is fired off to reset the expiration timer
                SetProcessInitiatedDate(personApplicant, personChanges, DateTime.Now);

                //Set Workflow URL
                var newWorkflowUrl = globalAttributes.GetValue("InternalApplicationRoot").EnsureTrailingForwardslash() + "page/291?workflowId=" + action.Activity.WorkflowId;
                var oldWorkflowUrl = personApplicant.AttributeValues["ProtectionLatestWorkflow"].Value;
                History.EvaluateChange(personChanges, "ProtectionLatestWorkflow", oldWorkflowUrl, newWorkflowUrl);
                personApplicant.SetAttributeValue("ProtectionLatestWorkflow", newWorkflowUrl);

                //Set Latest Workflow Requester
                var newLatestProtectionWorkflowRequester = "<a href=\"" + globalAttributes.GetValue("InternalApplicationRoot").EnsureTrailingForwardslash() + "Person/" + personRequester.Id.ToString() + "\">" + personRequester.FirstName + " " + personRequester.LastName + "</a>";
                var oldLatestProtectionWorkflowRequester = personApplicant.AttributeValues["ProtectionLatestWorkflowRequester"].Value;
                History.EvaluateChange(personChanges, "ProtectionLatestWorkflowRequester", oldLatestProtectionWorkflowRequester, newLatestProtectionWorkflowRequester);
                personApplicant.SetAttributeValue("ProtectionLatestWorkflowRequester", newLatestProtectionWorkflowRequester);

                //Variable to hold all existing dates
                DateTime? oldAppDate = null, oldRef1Date = null, oldRef2Date = null, oldRef3Date = null, oldBcDate = null, oldPolicyDate = null;
                string oldAppDoc = "", oldRef1Doc = "", oldRef2Doc = "", oldRef3Doc = "", oldBcDoc = "";
                string oldBcResult = "";

                if (application)
                {
                    //Get existing data
                    oldAppDate = (DateTime?)personApplicant.AttributeValues["ProtectionApplicationDate"].ValueAsType;
                    oldAppDoc = personApplicant.AttributeValues["ProtectionApplication"].Value;

                    //Clear current
                    History.EvaluateChange(personChanges, "ProtectionApplicationDate", oldAppDate, null);
                    History.EvaluateChange(personChanges, "ProtectionApplication", oldAppDoc, null);
                    personApplicant.SetAttributeValue("ProtectionApplicationDate", null);
                    //TODO Show View with link
                    personApplicant.SetAttributeValue("ProtectionApplication", null);

                    //Add documents to File List
                    //Update binaryfile record to filetype Protection Archive
                    ArchiveDocument(true, ref oldAppDoc, applicant.Guid, protectionArchiveType.Id, fileService, bfdService, pdService);
                }

                if (references)
                {
                    //Get Archive timeline
                    var validRefDays = Convert.ToInt32(globalAttributes.GetValue("ProtectionPopulateReferencesDays"));

                    //Get existing data
                    oldRef1Date = (DateTime?)personApplicant.AttributeValues["ProtectionReference1Date"].ValueAsType;
                    oldRef1Doc = personApplicant.AttributeValues["ProtectionReference1"].Value;

                    oldRef2Date = (DateTime?)personApplicant.AttributeValues["ProtectionReference2Date"].ValueAsType;
                    oldRef2Doc = personApplicant.AttributeValues["ProtectionReference2"].Value;

                    oldRef3Date = (DateTime?)personApplicant.AttributeValues["ProtectionReference3Date"].ValueAsType;
                    oldRef3Doc = personApplicant.AttributeValues["ProtectionReference3"].Value;

                    var oldRefListDoc = personApplicant.AttributeValues["ProtectionReferenceList"].Value;

                    //Clear current if older than archive date
                    if (oldRef1Date != null && oldRef1Date < DateTime.Now.AddDays(-validRefDays))
                    {
                        History.EvaluateChange(personChanges, "ProtectionReference1Date", oldRef1Date, null);
                        History.EvaluateChange(personChanges, "ProtectionReference1", oldRef1Doc, null);
                        personApplicant.SetAttributeValue("ProtectionReference1Date", null);
                        personApplicant.SetAttributeValue("ProtectionReference1", null);
                    }
                    else
                    {
                        //Clear date and doc so we don't put in archive table
                        oldRef1Date = null;
                        oldRef1Doc = null;
                    }

                    if (oldRef2Date != null && oldRef2Date < DateTime.Now.AddDays(-validRefDays))
                    {
                        History.EvaluateChange(personChanges, "ProtectionReference2Date", oldRef2Doc, null);
                        History.EvaluateChange(personChanges, "ProtectionReference2", oldRef2Doc, null);
                        personApplicant.SetAttributeValue("ProtectionReference2Date", null);
                        personApplicant.SetAttributeValue("ProtectionReference2", null);
                    }
                    else
                    {
                        //Clear date and doc so we don't put in archive table
                        oldRef2Date = null;
                        oldRef2Doc = null;
                    }

                    if (oldRef3Date != null && oldRef3Date < DateTime.Now.AddDays(-validRefDays))
                    {
                        History.EvaluateChange(personChanges, "ProtectionReference3Date", oldRef3Doc, null);
                        History.EvaluateChange(personChanges, "ProtectionReference3", oldRef3Doc, null);
                        personApplicant.SetAttributeValue("ProtectionReference3Date", null);
                        personApplicant.SetAttributeValue("ProtectionReference3", null);
                    }
                    else
                    {
                        //Clear date and doc so we don't put in archive table
                        oldRef3Date = null;
                        oldRef3Doc = null;
                    }

                    History.EvaluateChange(personChanges, "ProtectionReferenceList", oldRefListDoc, null);
                    personApplicant.SetAttributeValue("ProtectionReferenceList", null);

                    //Add documents to File List
                    //Update binaryfile record to filetype Protection Archive
                    ArchiveDocument(true, ref oldRef1Doc, applicant.Guid, protectionArchiveType.Id, fileService, bfdService, pdService);
                    ArchiveDocument(true, ref oldRef2Doc, applicant.Guid, protectionArchiveType.Id, fileService, bfdService, pdService);
                    ArchiveDocument(true, ref oldRef3Doc, applicant.Guid, protectionArchiveType.Id, fileService, bfdService, pdService);
                    ArchiveDocument(true, ref oldRefListDoc, applicant.Guid, protectionArchiveType.Id, fileService, bfdService, pdService);
                }

                if (backgroundCheck)
                {
                    //Get existing data
                    oldBcDate = (DateTime?)personApplicant.AttributeValues["BackgroundCheckDate"].ValueAsType;
                    oldBcDoc = personApplicant.AttributeValues["BackgroundCheckDocument"].Value;
                    //TODO VALIDATE YES/NO PASS/FAIL
                    oldBcResult = (string)personApplicant.AttributeValues["BackgroundCheckResult"].ValueAsType;

                    //Clear current
                    History.EvaluateChange(personChanges, "BackgroundCheckDate", oldBcDate, null);
                    History.EvaluateChange(personChanges, "BackgroundCheckDocument", oldBcDoc, null);
                    History.EvaluateChange(personChanges, "BackgroundCheckResult", oldBcResult, null);
                    personApplicant.SetAttributeValue("BackgroundCheckDate", null);
                    personApplicant.SetAttributeValue("BackgroundCheckDocument", null);
                    personApplicant.SetAttributeValue("BackgroundCheckResult", null);

                    ArchiveDocument(true, ref oldBcDoc, applicant.Guid, protectionArchiveType.Id, fileService, bfdService, pdService);
                }

                if (policyAcknowledgement)
                {
                    //Get existing data
                    oldPolicyDate = (DateTime?)personApplicant.AttributeValues["PolicyAcknowledgmentDate"].ValueAsType;

                    //Clear current
                    History.EvaluateChange(personChanges, "PolicyAcknowledgmentDate", oldPolicyDate, null);
                    personApplicant.SetAttributeValue("PolicyAcknowledgmentDate", null);
                }

                //Save to Queue
                var queueService = new QueueService(protContext);
                var queue = new Queue
                {
                    RequesterFirstName = personRequester.FirstName,
                    RequesterLastName = personRequester.LastName,
                    RequesterPersonAliasGuid = requester.Guid,
                    ApplicantFirstName = personApplicant.FirstName,
                    ApplicantLastName = personApplicant.LastName,
                    ApplicantPersonAliasGuid = applicant.Guid,
                    //Changing this column
                    WorkflowId = action.Activity.WorkflowId,
                    Campus = campusGuidString,
                    Ministry = ministryGuidString,
                    CreatedDateTime = DateTime.Now,
                    Guid = new Guid()
                };
                queueService.Add(queue);

                if (personChanges.Any())
                {
                    //save applicant person
                    personApplicant.SaveAttributeValues(rockContext);
                    HistoryService.SaveChanges(
                        rockContext,
                        typeof(Person),
                        Rock.SystemGuid.Category.HISTORY_PERSON_ACTIVITY.AsGuid(),
                        personApplicant.Id,
                        personChanges,
                        true);

                    //If more than one change, insert into protection app archive table
                    if (personChanges.Count > 1 && !protectionStatus.IsEmpty())   //TODO: model may need to change to accomidate a null status
                    {
                        //Add record to Protection Archive Table
                        Guid temp;
                        var archive = new Archive
                        {
                            ArchiveDateTime = DateTime.Now,
                            ProtectionStatus = protectionStatus,
                            ApplicationBinaryFileId = Guid.TryParse(oldAppDoc, out temp) ? (Guid?)temp : null,
                            ApplicationDate = oldAppDate,
                            BackgroundCheckBinaryFileId = Guid.TryParse(oldBcDoc, out temp) ? (Guid?)temp : null,
                            BackgroundCheckDate = oldBcDate,
                            BackgroundCheckResult = oldBcResult,
                            Reference1BinaryFileId = Guid.TryParse(oldRef1Doc, out temp) ? (Guid?)temp : null,
                            Reference1Date = oldRef1Date,
                            Reference2BinaryFileId = Guid.TryParse(oldRef2Doc, out temp) ? (Guid?)temp : null,
                            Reference2Date = oldRef2Date,
                            Reference3BinaryFileId = Guid.TryParse(oldRef3Doc, out temp) ? (Guid?)temp : null,
                            Reference3Date = oldRef3Date,
                            PolicyAcknowledgmentDate = oldPolicyDate,
                            PersonAliasGuid = applicantGuid
                        };

                        var archiveService = new ArchiveService(protContext);
                        archiveService.Add(archive);
                    }
                }

                protContext.SaveChanges();
                rockContext.SaveChanges();
                fmContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                ExceptionLogService.LogException(ex, System.Web.HttpContext.Current);
                errorMessages.Add(ex.Message);
                return false;
            }
        }

        private static void SetProcessInitiatedDate(IHasAttributes applicant, List<string> personChanges, DateTime processInitiated)
        {
            var key = "ProtectionProcessInitiated";
            var oldValue = applicant.AttributeValues[key].ValueAsType as DateTime?;
            applicant.SetAttributeValue(key, processInitiated);
            History.EvaluateChange(personChanges, key, oldValue, processInitiated, true);
        }

        /// <summary>
        /// Save file to archive table, duplicate in binaryfile table if necessary
        /// </summary>
        /// <param name="duplicate"></param>
        /// <param name="oldDoc"></param>
        /// <param name="applicantGuid"></param>
        /// <param name="protectionArchiveTypeId"></param>
        /// <param name="fileService"></param>
        /// <param name="bfdService"></param>
        /// <param name="pdService"></param>
        private void ArchiveDocument(bool duplicate, ref string oldDoc, Guid applicantGuid, int protectionArchiveTypeId, BinaryFileService fileService, BinaryFileDataService bfdService, PersonDocumentService pdService)
        {
            Guid fileGuid;
            Guid.TryParse(oldDoc, out fileGuid);
            Guid binaryFieldId = fileGuid;

            if (!fileGuid.IsEmpty())
            {
                var oldFile = fileService.Get(fileGuid);

                if ( oldFile != null )
                {
                    if ( duplicate )
                    {
                        var archiveFile = new BinaryFile();
                        archiveFile.CopyAttributesFrom( oldFile );
                        archiveFile.CopyPropertiesFrom( oldFile );
                        archiveFile.BinaryFileTypeId = protectionArchiveTypeId;
                        archiveFile.Guid = Guid.NewGuid();
                        archiveFile.Id = 0;

                        var archiveData = new BinaryFileData();
                        archiveData.CopyAttributesFrom( oldFile.DatabaseData );
                        archiveData.CopyPropertiesFrom( oldFile.DatabaseData );
                        archiveData.Guid = Guid.NewGuid();
                        archiveData.Id = 0;
                        bfdService.Add( archiveData );

                        archiveFile.DatabaseData = archiveData;
                        fileService.Add( archiveFile );

                        //Update oldappdoc since it will be deleted
                        oldDoc = archiveFile.Guid.ToString();

                        binaryFieldId = archiveFile.Guid;
                    }
                    else
                    {
                        oldFile.BinaryFileTypeId = protectionArchiveTypeId;
                    }

                    //Add file to persondocument table
                    var archiveDoc = new PersonDocument
                    {
                        PersonAliasGuid = applicantGuid,
                        BinaryFileId = binaryFieldId,
                        CreatedDateTime = DateTime.Now
                    };
                    pdService.Add( archiveDoc );
                }
            }
        }
    }
}
