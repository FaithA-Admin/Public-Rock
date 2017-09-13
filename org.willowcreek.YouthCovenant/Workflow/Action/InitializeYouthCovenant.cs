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

using org.willowcreek.SystemGuid;
using org.willowcreek.FileManagement.Data;
using org.willowcreek.FileManagement.Model;
using org.willowcreek.YouthCovenant.Logic;

namespace org.willowcreek.YouthCovenant.Workflow.Action
{
    [ActionCategory( "Protection" )]
    [Description( "Initialize the Youth Covenant process." )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Initialize Youth Covenant" )]

    [WorkflowAttribute( "Applicant", "(IN) Person who the application process will be started for.", true, "", "", 1, null, new string[] { "Rock.Field.Types.PersonFieldType" } )]
    [WorkflowAttribute( "Requester", "(IN) Person who started the application process.", true, "", "", 2, null, new string[] { "Rock.Field.Types.PersonFieldType" } )]
    [WorkflowAttribute( "Campus", "(IN) The campus attribute to save.", false, "", "", 3, null, new string[] { "Rock.Field.Types.CampusFieldType" } )]
    [WorkflowAttribute( "Ministry", "(IN) The ministry attribute to save.", false, "", "", 4, null, new string[] { "Rock.Field.Types.DefinedValueFieldType" } )]
    [WorkflowAttribute( "Send Application", "(IN) Boolen attribute that stores if the application should be sent.", true, "", "", 5, null, new string[] { "Rock.Field.Types.BooleanFieldType" } )]
    [WorkflowAttribute( "Request Reference", "(IN) Boolen attribute that stores if the references should be requested.", true, "", "", 6, null, new string[] { "Rock.Field.Types.BooleanFieldType" } )]
    [WorkflowAttribute( "Workflows", "(IN) List of guid(s) of other workflows.", false, "", "", 7, null, new string[] { "Rock.Field.Types.TextFieldType" } )]
    public class InitializeYouthCovenant : ActionComponent
    {
        // Global Attributes
        private const string INTERNAL_APPLICATION_ROOT_KEY = "InternalApplicationRoot";

        // Person Attributes
        private const string YOUTH_COVENANT_STATUS_KEY = "YouthCovenantStatus";
        private const string YOUTH_COVENANT_KEY = "YouthCovenant";
        private const string YOUTH_COVENANT_DATE_KEY = "YouthCovenantDate";
        private const string YOUTH_COVENANT_REFERENCE_KEY = "YouthCovenantReference1";
        private const string YOUTH_COVENANT_REFERENCE_DATE_KEY = "YouthCovenantReference1Date";
        private const string YOUTH_COVENANT_REFERENCE_LIST_KEY = "YouthCovenantReferenceList";
        private const string YOUTH_COVENANT_PROCESS_INITIATED_KEY = "YouthCovenantProcessInitiated";
        private const string YOUTH_COVENANT_LATEST_WORKFLOW_KEY = "YouthCovenantLatestWorkflow";
        private const string YOUTH_COVENANT_LATEST_WORKFLOW_REQUESTER_KEY = "YouthCovenantLatestWorkflowRequester";

        public override bool Execute( RockContext rockContext, WorkflowAction action, object entity, out List<string> errorMessages )
        {
            errorMessages = new List<string>();
            try
            {
                var personChanges = new List<string>();

                var fmContext = new FileManagementContext();

                var ps = new PersonAliasService( rockContext );
                var personService = new PersonService( rockContext );
                var fileService = new BinaryFileService( rockContext );
                var bftService = new BinaryFileTypeService( rockContext );
                var bfdService = new BinaryFileDataService( rockContext );
                var pdService = new PersonDocumentService( fmContext );

                var globalAttributes = GlobalAttributesCache.Read();
                var internalApplicationRoot = globalAttributes.GetValue( INTERNAL_APPLICATION_ROOT_KEY ).EnsureTrailingForwardslash();

                //Get Applicant Alias
                var applicant = ps.Get( GetAttributeValue( action, "Applicant", true ).AsGuid() );

                //Get Applicant
                var personApplicant = applicant.Person;
                personApplicant.LoadAttributes();
                var oldYouthCovenant = personApplicant.AttributeValues[YOUTH_COVENANT_KEY].Value;
                var oldYouthCovenantDate = personApplicant.AttributeValues[YOUTH_COVENANT_DATE_KEY].ValueAsType as DateTime?;
                var oldReference = personApplicant.AttributeValues[YOUTH_COVENANT_REFERENCE_KEY].Value;
                var oldReferenceDate = personApplicant.AttributeValues[YOUTH_COVENANT_REFERENCE_DATE_KEY].ValueAsType as DateTime?;
                var oldReferenceList = personApplicant.AttributeValues[YOUTH_COVENANT_REFERENCE_LIST_KEY].Value;
                var oldProcessInitiatedDate = personApplicant.AttributeValues[YOUTH_COVENANT_PROCESS_INITIATED_KEY].ValueAsType as DateTime?;
                var oldWorkflowUrl = personApplicant.AttributeValues[YOUTH_COVENANT_LATEST_WORKFLOW_KEY].Value;
                var oldLatestProtectionWorkflowRequester = personApplicant.AttributeValues[YOUTH_COVENANT_LATEST_WORKFLOW_REQUESTER_KEY].Value;

                //Get Requester Alias
                var requester = ps.Get( GetAttributeValue( action, "Requester", true ).AsGuid() );

                //Get Requester
                Person personRequester = requester.Person;
                personRequester.LoadAttributes();

                // Update the Process Initiated Date
                // This must happen every time a protection workflow is fired off to reset the expiration timer
                var newProcessInitiatedDate = DateTime.Now;
                History.EvaluateChange( personChanges, YOUTH_COVENANT_PROCESS_INITIATED_KEY, oldProcessInitiatedDate, newProcessInitiatedDate, true );
                personApplicant.SetAttributeValue( YOUTH_COVENANT_PROCESS_INITIATED_KEY, newProcessInitiatedDate );

                //Set Workflow URL
                var newWorkflowUrl = internalApplicationRoot + "page/291?workflowId=" + action.Activity.WorkflowId;
                History.EvaluateChange( personChanges, YOUTH_COVENANT_LATEST_WORKFLOW_KEY, oldWorkflowUrl, newWorkflowUrl );
                personApplicant.SetAttributeValue( YOUTH_COVENANT_LATEST_WORKFLOW_KEY, newWorkflowUrl );

                //Set Latest Workflow Requester
                var newLatestProtectionWorkflowRequester = "<a href=\"" + internalApplicationRoot + "Person/" + personRequester.Id.ToString() + "\">" + personRequester.NickName + " " + personRequester.LastName + "</a>";
                History.EvaluateChange( personChanges, YOUTH_COVENANT_LATEST_WORKFLOW_REQUESTER_KEY, oldLatestProtectionWorkflowRequester, newLatestProtectionWorkflowRequester );
                personApplicant.SetAttributeValue( YOUTH_COVENANT_LATEST_WORKFLOW_REQUESTER_KEY, newLatestProtectionWorkflowRequester );

                // If the application is being sent
                // NOTE: Youth covenants don't need to be refreshed, so much of this logic may be unnecessary
                if ( GetAttributeValue( action, "SendApplication", true ).AsBoolean() )
                {
                    // Clear out the old values
                    personApplicant.SetAttributeValue( YOUTH_COVENANT_KEY, null );
                    personApplicant.SetAttributeValue( YOUTH_COVENANT_DATE_KEY, null );
                    History.EvaluateChange( personChanges, YOUTH_COVENANT_KEY, oldYouthCovenant, null );
                    History.EvaluateChange( personChanges, YOUTH_COVENANT_DATE_KEY, oldYouthCovenantDate, null );

                    // Archive any existing youth covenant
                    ArchiveDocument( true, ref oldYouthCovenant, applicant.Guid, fileService, bfdService, pdService );
                }

                // If a reference is being requested
                if ( GetAttributeValue( action, "RequestReference", true ).AsBoolean() )
                {
                    // Clear out the old values
                    personApplicant.SetAttributeValue( YOUTH_COVENANT_REFERENCE_KEY, null );
                    personApplicant.SetAttributeValue( YOUTH_COVENANT_REFERENCE_DATE_KEY, null );
                    personApplicant.SetAttributeValue( YOUTH_COVENANT_REFERENCE_LIST_KEY, null );
                    History.EvaluateChange( personChanges, YOUTH_COVENANT_REFERENCE_KEY, oldReference, null );
                    History.EvaluateChange( personChanges, YOUTH_COVENANT_REFERENCE_DATE_KEY, oldReferenceDate, null );
                    History.EvaluateChange( personChanges, YOUTH_COVENANT_REFERENCE_DATE_KEY, oldReferenceList, null );

                    // Archive any existing references 
                    ArchiveDocument( true, ref oldReference, applicant.Guid, fileService, bfdService, pdService );
                }

                // Update the Youth Covenant Status based on the above changes
                personApplicant.SetYouthCovenantStatus(ref personChanges, rockContext );

                // TODO: Add to the Queue table? 

                personApplicant.SaveAttributeValues( rockContext );
                HistoryService.SaveChanges( rockContext, typeof( Person ), Rock.SystemGuid.Category.HISTORY_PERSON_ACTIVITY.AsGuid(), personApplicant.Id, personChanges, true );

                // TODO: Insert into the Archive table?

                // Save and Exit
                rockContext.SaveChanges();
                fmContext.SaveChanges();
                return true;
            }
            catch ( Exception ex )
            {
                //while ( ex.InnerException != null )
                //{
                //    ex = ex.InnerException;
                //}
                ExceptionLogService.LogException( ex, System.Web.HttpContext.Current );
                errorMessages.Add( ex.Message );
                return false;
            }
        }

        // TODO: Merge this and the one in InitializeProtectionForPerson into common logic
        private void ArchiveDocument( bool duplicate, ref string oldDoc, Guid applicantGuid, BinaryFileService fileService, BinaryFileDataService bfdService, PersonDocumentService pdService )
        {
            var bftService = new BinaryFileTypeService( new RockContext() );
            var protectionArchiveTypeId = bftService.Queryable().FirstOrDefault( b => b.Name == "Protection Archive" ).Id;

            Guid fileGuid;
            Guid.TryParse( oldDoc, out fileGuid );
            Guid binaryFieldId = fileGuid;

            if ( !fileGuid.IsEmpty() )
            {
                var oldFile = fileService.Get( fileGuid );

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
