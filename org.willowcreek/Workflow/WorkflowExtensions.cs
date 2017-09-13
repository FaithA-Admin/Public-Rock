using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock.Model;
using Rock.Data;

namespace org.willowcreek.Workflow
{
    public static class WorkflowExtensions
    {
        /// <summary>
        /// Process a <see cref="Rock.Model.Workflow"/>
        /// </summary>
        /// <param name="workflow">The <see cref="Rock.Model.Workflow"/> to process</param>
        /// <param name="workflowService">The <see cref="WorkflowService"/> used to get this workflow. If this is unavailable, use the <see cref="RockContext"/> instead.</param>
        public static void Process(this Rock.Model.Workflow workflow, WorkflowService workflowService)
        {
            List<string> errorMessages;
            workflowService.Process(workflow, out errorMessages);
            foreach (var message in errorMessages)
            {
                ExceptionLogService.LogException(new Exception(message), System.Web.HttpContext.Current);
            }
        }

        /// <summary>
        /// Process a <see cref="Rock.Model.Workflow"/>
        /// </summary>
        /// <param name="workflow">The <see cref="Rock.Model.Workflow"/> to process</param>
        /// <param name="workflowContext">The <see cref="RockContext"/> object that was used to create this workflow. This should be used only if the <see cref="WorkflowService"/> is unavailable.</param>
        public static void Process(this Rock.Model.Workflow workflow, RockContext workflowContext)
        {
            var workflowService = new WorkflowService(workflowContext);
            workflow.Process(workflowService);
        }

        /// <summary>
        /// Process a <see cref="Rock.Model.Workflow"/> asynchronously
        /// </summary>
        /// <param name="workflow">The <see cref="Rock.Model.Workflow"/> to process</param>
        /// <param name="workflowService">The <see cref="WorkflowService"/> used to get this workflow. If this is unavailable, use the <see cref="RockContext"/> instead.</param>
        public static void ProcessAsync(this Rock.Model.Workflow workflow, WorkflowService workflowService)
        {
            Task.Run(() => workflow.Process(workflowService));
        }

        /// <summary>
        /// Process a <see cref="Rock.Model.Workflow"/> asynchronously
        /// </summary>
        /// <param name="workflow">The <see cref="Rock.Model.Workflow"/> to process</param>
        /// <param name="workflowContext">The <see cref="RockContext"/> object that was used to create this workflow. This should be used only if the <see cref="WorkflowService"/> is unavailable.</param>
        public static void ProcessAsync(this Rock.Model.Workflow workflow, RockContext workflowContext)
        {
            Task.Run(() => workflow.Process(workflowContext));
        }
    }
}
