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

namespace org.willowcreek.Workflow.Action
{
    [ActionCategory("Workflow Control")]
    [Description("Cancels workflow(s) using it's Id(s).")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Cancel Workflow By Id")]

    //IN
    [WorkflowAttribute("Workflow Id(s)", "(IN) The Id(s) of the workflow(s) to cancel.", true, "", "", 0, null, new string[] { "Rock.Field.Types.IntegerFieldType" })]
    [TextField("Status", "(IN) The text to place in the status column of the canceled workflow.", true, "Canceled", "", 1, null, false, "Rock.Field.Types.TextFieldType")]

    class CancelWorkflowById : ActionComponent
    {

        public override bool Execute(RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            

            return true;
        }

    }
}
