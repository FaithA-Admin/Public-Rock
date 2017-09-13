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

using org.willowcreek.ProtectionApp.Data;
using org.willowcreek.ProtectionApp.Model;
using org.willowcreek.Model.Extensions;
using org.willowcreek.ProtectionApp.Logic;

namespace org.willowcreek.ProtectionApp.Workflow.Action
{
    [ActionCategory("Protection")]
    [Description("Loads Protection Summary from person.")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Load Protection Summary From Person")]

    //IN
    [WorkflowAttribute("Applicant", "(IN) The protection applicant who's summary will be retrieved.", true, "", "", 0, null, new string[] { "Rock.Field.Types.PersonFieldType" })]

    //OUT
    [WorkflowAttribute("Summary", "(OUT) HTML to display as the protection summary for this applicant.", true, "", "", 1, null, new string[] { "Rock.Field.Types.HTMLFieldType" })]
    [WorkflowAttribute("Send Application Form", "(OUT) Boolen attribute that stores if the application should be sent.", true, "", "", 2, null, new string[] { "Rock.Field.Types.BooleanFieldType" })]
    [WorkflowAttribute("Request References", "(OUT) Boolen attribute that stores if the references should be requested.", true, "", "", 3, null, new string[] { "Rock.Field.Types.BooleanFieldType" })]
    [WorkflowAttribute("Run Background Check", "(OUT) Boolen attribute that stores if the background check should be run.", true, "", "", 4, null, new string[] { "Rock.Field.Types.BooleanFieldType" })]
    [WorkflowAttribute("Send Policy Acknowledgment Form", "(OUT) Boolen attribute that stores if the policy acknowledgment should be sent.", true, "", "", 5, null, new string[] { "Rock.Field.Types.BooleanFieldType" })]
    [WorkflowAttribute("Workflows", "(OUT) List of Id(s) of other workflows.", false, "", "", 6, null, new string[] { "Rock.Field.Types.TextFieldType" })]

    class LoadProtectionSummaryFromPerson : ActionComponent
    {

        public override bool Execute(RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            try
            {
                bool needsApplicationForm = true;
                bool needsReferences = true;
                bool needsBackgroundCheck = true;
                bool needsPolicyAcknowledgementForm = true;

                string appDate = String.Empty;
                string ref1Date = String.Empty;
                string ref2Date = String.Empty;
                string ref3Date = String.Empty;
                string bcDate = String.Empty;
                string paDate = String.Empty;

                ProtectionEvaluator pe = new ProtectionEvaluator();

                var applicantGuid = this.GetAttributeValue(action, "Applicant", rockContext);

                Guid applicant;// = Guid.Parse(applicantGuid.ToString());
                if (!Guid.TryParse(applicantGuid, out applicant))
                    return true;

                needsApplicationForm = pe.CheckProtectionStepStatus(rockContext, "ProtectionApplicationDate", applicant, out appDate);

                int refCount = 0;
                refCount = Convert.ToInt32(pe.CheckProtectionStepStatus(rockContext, "ProtectionReference1Date", applicant, out ref1Date));
                refCount += Convert.ToInt32(pe.CheckProtectionStepStatus(rockContext, "ProtectionReference2Date", applicant, out ref2Date));
                refCount += Convert.ToInt32(pe.CheckProtectionStepStatus(rockContext, "ProtectionReference3Date", applicant, out ref3Date));
                if (refCount >= 2)
                    needsReferences = true;
                else
                    needsReferences = false;

                needsBackgroundCheck = pe.CheckProtectionStepStatus(rockContext, "BackgroundCheckDate", applicant, out bcDate);
                needsPolicyAcknowledgementForm = pe.CheckProtectionStepStatus(rockContext, "PolicyAcknowledgmentDate", applicant, out paDate);

                string protectionSummary = "<TABLE style='border:1px solid gray;border-collapse:collapse;'>"
                                         + "<TR><TD style='border:1px solid gray;padding-left:4px;padding-right:4px;text-align:center;'>Application</TD>"
                                         + "<TD style='border:1px solid gray;padding-left:4px;padding-right:4px;text-align:center;'>References</TD>"
                                         + "<TD style='border:1px solid gray;padding-left:4px;padding-right:4px;text-align:center;'>Background Check</TD>"
                                         + "<TD style='border:1px solid gray;padding-left:4px;padding-right:4px;text-align:center;'>Policy Acknowledgement</TD></TR>"
                                         + "<TR style='border:1px solid gray;padding-left:4px;padding-right:4px;text-align:center;'><TD>" + appDate + "</TD>"
                                         + "<TD style='border:1px solid gray;padding-left:4px;padding-right:4px;text-align:center;'>" + ref1Date + "</BR>" + ref2Date + "<BR>" + ref3Date + "</TD>"
                                         + "<TD style='border:1px solid gray;padding-left:4px;padding-right:4px;text-align:center;'>" + bcDate + "</TD>"
                                         + "<TD style='border:1px solid gray;padding-left:4px;padding-right:4px;text-align:center;'>" + paDate + "</TD></TR></TABLE>";

                this.SetAttributeValue(action, "Summary", rockContext, protectionSummary);
                this.SetAttributeValue(action, "SendApplicationForm", rockContext, needsApplicationForm.ToString());
                this.SetAttributeValue(action, "RequestReferences", rockContext, needsReferences.ToString());
                this.SetAttributeValue(action, "RunBackgroundCheck", rockContext, needsBackgroundCheck.ToString());
                this.SetAttributeValue(action, "SendPolicyAcknowledgmentForm", rockContext, needsPolicyAcknowledgementForm.ToString());

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
