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
    [Description("Evaluates an applicant's protection status")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Evaluate Protection Status for Person")]

    //IN
    [WorkflowAttribute("Applicant", "(IN) The protection applicant who will be evaluated.", true, "", "", 0, null, new string[] { "Rock.Field.Types.PersonFieldType" })]

    class EvaluateProtectionForPerson : ActionComponent
    {
        public override bool Execute(RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            try
            {
                var applicantGuid = this.GetAttributeValue(action, "Applicant", rockContext);
                Guid applicant;
                if (!Guid.TryParse(applicantGuid, out applicant))
                {
                    errorMessages.Add("Applicant could not be found.");
                    return false;
                }
                var personAliasService = new PersonAliasService(rockContext);
                var person = personAliasService.GetPerson(applicant);
                ProtectionEvaluator.EvaluateAll(rockContext, new List<Person> { person });
                rockContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                ExceptionLogService.LogException(ex, System.Web.HttpContext.Current);
                errorMessages.Add(ex.Message);
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    ExceptionLogService.LogException(ex, System.Web.HttpContext.Current);
                    errorMessages.Add(ex.Message);
                }
                return false;
            }
        }
    }
}
