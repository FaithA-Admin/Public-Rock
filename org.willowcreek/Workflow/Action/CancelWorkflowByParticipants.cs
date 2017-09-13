using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Workflow;

using org.willowcreek.Model.Extensions;

namespace org.willowcreek.Workflow.Action
{
    [ActionCategory("Workflow Control")]
    [Description("Cancels previous workflow(s) of same workflow type and matching participants; assuming it's not already complete.")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Cancel Workflow By Participants")]

    //IN
    [WorkflowAttribute("Initiator", "(IN) Key 1 for determinating canceling workflow(s).  Usually, initiator, creator or requester.", true, "", "", 1, null, new string[] { "Rock.Field.Types.PersonFieldType" })]
    [WorkflowAttribute("Recipient", "(IN) Key 2 for determinating canceling workflow(s).  Usually, recipient, requestee, respondent, or applicant.", true, "", "", 2, null, new string[] { "Rock.Field.Types.PersonFieldType" })]
    [TextField("Status", "(IN) The text to place in the status column of the canceled workflow.", true, "Canceled", "", 3, null, false, "Rock.Field.Types.TextFieldType")]

    //OUT
    [WorkflowAttribute("Canceled Workflow(s)", "(OUT) List of workflow Id(s) that were canceled.", false, "", "", 4, null, new string[] { "Rock.Field.Types.TextFieldType" })]

    /// <summary>
    /// Cancels a list of workflows based on workflow type and two participating people.
    /// </summary>
    class CancelWorkflowByParticipants : ActionComponent
    {

        public override bool Execute(RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            try
            {
                var workflowId = action.Activity.WorkflowId;

                //Causes cancel of self
                if (workflowId <= 0)
                {
                    try
                    {
                        var parent = (WorkflowActivity)action.ParentAuthority;
                        workflowId = parent.Workflow.Id;
                        if (workflowId <= 0)
                            return true;
                    }
                    catch
                    {
                        return true;
                    }
                }

                WorkflowService workflowService = new WorkflowService(rockContext);
                var workflows = workflowService.Cancel(action.Activity.Workflow.WorkflowTypeId,
                                                        this.GetAttributeId(action, "Initiator", rockContext).GetValueOrDefault(),
                                                        this.GetAttributeValue(action, "Initiator", rockContext).AsGuidOrNull(),
                                                        this.GetAttributeId(action, "Recipient", rockContext).GetValueOrDefault(),
                                                        this.GetAttributeValue(action, "Recipient", rockContext).AsGuidOrNull(),
                                                        workflowId,
                                                        this.GetAttributeValue(action, "Status"));

                //Save a list of canceled workflows
                string s = string.Join(",", workflows.Select(r => r.Id.ToString()));
                this.SetAttributeValue(action, "CanceledWorkflow(s)", rockContext, s);

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
