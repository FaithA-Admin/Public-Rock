using Quartz;
using Rock.Data;
using Rock.Model;
using Rock.Attribute;
using System;
using System.Linq;

using org.willowcreek.Model.Extensions;

namespace org.willowcreek.Jobs
{
    [IntegerField("Days", "Number of days workflows are allowed to run.", true, 60, "", 1, null)]
    [TextField("Delayed Workflow Type Ids", "List of workflow type Ids to expire after X days. (separated by commas)", true, "", "", 2, null, false, null)]
    [TextField("Immediate Workflow Type Ids", "List of workflow type Ids to expire immediately. (separated by commas)", true, "", "", 3, null, false, null)]
    class WorkflowCleanUp : IJob
    {

        public void Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            var rockContext = new RockContext();
            WorkflowService workflowService = new WorkflowService(rockContext);
            var delayed = workflowService.Expire(dataMap.GetInt("Days"), dataMap.GetString("DelayedWorkflowTypeIds"));
            var immediate = workflowService.Expire(0, dataMap.GetString("ImmediateWorkflowTypeIds"));

            context.Result = delayed.Count + " delayed, " + immediate.Count + " immediate";
            rockContext.SaveChanges();
        }

    }
}
