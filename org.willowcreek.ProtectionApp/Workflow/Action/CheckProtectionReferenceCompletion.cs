using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using org.willowcreek.ProtectionApp.Data;
using org.willowcreek.ProtectionApp.Model;
using org.willowcreek.ProtectionApp.Rest;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Workflow;
using org.willowcreek.Model.Extensions;

namespace org.willowcreek.Workflow.Action
{
    [ActionCategory("Protection")]
    [Description("Checks if a reference was previously completed.")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Check Protection Reference Completion")]

    //IN
    [WorkflowAttribute("Applicant", "(IN) Person who the application process will be started for.", true, "", "", 1, null, new string[] { "Rock.Field.Types.PersonFieldType" })]
    [WorkflowAttribute("Reference", "(IN) The reference for the applicant.", true, "", "", 2, null, new string[] { "Rock.Field.Types.PersonFieldType" })]
    [WorkflowAttribute("Reference Number", "(IN) Which reference? 1, 2 or 3.", true, "", "", 3, null, new string[] { "Rock.Field.Types.IntegerFieldType" })]

    //OUT
    [WorkflowAttribute("Completed", "(OUT) Boolen attribute that stores if the reference was already completed.", true, "", "", 4, null, new string[] { "Rock.Field.Types.BooleanFieldType" })]

    class CheckProtectionReferenceCompletion : ActionComponent
    {
        /// <summary>
        /// Determine if current reference number already completed for applicant
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
                PersonAliasService personAliasService = new PersonAliasService(rockContext);
                PersonService personService = new PersonService(rockContext);

                var applicantAttributeValue = this.GetAttributeValue(action, "Applicant", rockContext);
                var applicantGuid = Guid.Parse(applicantAttributeValue);
                var applicantPersonAlias = personAliasService.Get(applicantGuid);
                var applicant = personService.Get(applicantPersonAlias.Person.Guid);

                // No need to get the person object for the reference, especially since old workflows deleted their reference person and we may do so again eventually
                var referenceAttributeValue = this.GetAttributeValue(action, "Reference", rockContext);
                var referenceGuid = Guid.Parse(referenceAttributeValue);

                var referenceNumber = this.GetAttributeValue(action, "ReferenceNumber", rockContext);

                var rService = new ReferenceService(new ProtectionAppContext());
                var reference = rService.Queryable().Where(x => x.ApplicantPersonAliasGuid == applicantGuid &&
                                                                x.ReferencePersonAliasGuid == referenceGuid
                                                                ).OrderByDescending(x => x.CreatedDateTime).FirstOrDefault();

                if (reference != null)
                {
                    //Get correct reference attribute
                    var refAttribute = "";
                    switch (referenceNumber)
                    {
                        case "1":
                            refAttribute = "ProtectionReference1Date";
                            break;
                        case "2":
                            refAttribute = "ProtectionReference2Date";
                            break;
                        case "3":
                            refAttribute = "ProtectionReference3Date";
                            break;
                    }

                    //Check reference date on applicant
                    applicant.LoadAttributes(rockContext);
                    //Reference is complete if we have a date
                    var referenceComplete = !string.IsNullOrEmpty(applicant.AttributeValues[refAttribute].Value);

                    Guid guid = GetAttributeValue(action, "Completed").AsGuid();
                    SetWorkflowAttributeValue(action, guid, referenceComplete.ToString());
                }

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
    }
}
