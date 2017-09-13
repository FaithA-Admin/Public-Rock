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

namespace org.willowcreek.Workflow.Action
{
    [ActionCategory("Workflow Control")]
    [Description("Load list of workflow Id(s) by workflow type and matching participants.")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Load Workflow Id By Participants")]

    //IN
    [WorkflowAttribute("Initiator", "(IN) Key 1 for determinating retrieving workflow Id(s).  Usually, initiator, creator or requester.", true, "", "", 1, null, new string[] { "Rock.Field.Types.PersonFieldType" })]
    [WorkflowAttribute("Recipient", "(IN) Key 2 for determinating retrieving workflow Id(s).  Usually, recipient, requestee, respondent, or applicant.", true, "", "", 2, null, new string[] { "Rock.Field.Types.PersonFieldType" })]

    //OUT
    [WorkflowAttribute("Workflows", "(OUT) List of Id(s) of other workflows.", true, "", "", 3, null, new string[] { "Rock.Field.Types.TextFieldType" })]

    class LoadWorkflowIdByParticipants : ActionComponent
    {

        public override bool Execute(RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            try
            {
                WorkflowService workflowService = new WorkflowService(rockContext);
                var workflows = workflowService.Get(action.Activity.Workflow.WorkflowTypeId,
                                                    this.GetAttributeId(action, "Initiator", rockContext).GetValueOrDefault(),
                                                    this.GetAttributeValue(action, "Initiator", rockContext).AsGuidOrNull(),
                                                    this.GetAttributeId(action, "Recipient", rockContext).GetValueOrDefault(),
                                                    this.GetAttributeValue(action, "Recipient", rockContext).AsGuidOrNull(),
                                                    action.Activity.WorkflowId);

                //Save a list of canceled workflows
                string s = string.Join(",", workflows.Select(r => r.Id.ToString()));
                this.SetAttributeValue(action, "Workflows", rockContext, s);

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
