using Quartz;
using org.willowcreek.ProtectionApp.Logic;
using Rock.Data;

namespace org.willowcreek.ProtectionApp.Jobs
{
    class ProtectionExpiration : IJob
    {
        /// <summary>
        /// Job to Expire all workflows attached to a person who is no longer in progress protection status
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            //Get all workflows not in progress
            var rockContext = new RockContext();
            //Updated to weed out any in (Process Initiated, In Progress, Needs Review)
            var workflows = ProtectionEvaluator.GetWorkflowsNotInProtectionStatus(rockContext);

            //Expire all
            workflows.ForEach(w => w.MarkComplete());
            workflows.ForEach(w => w.Status = "Ended");

            context.Result = workflows.Count + " ended";
            rockContext.SaveChanges();
        }
    }
}
