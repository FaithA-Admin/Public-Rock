using Quartz;
using org.willowcreek.ProtectionApp.Logic;
using Rock.Data;
using System;

namespace org.willowcreek.ProtectionApp.Jobs
{
    class ProtectionEvaluation : IJob
    {
        /// <summary>
        /// Job to evaluate everyons protection status
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            //Get every person with a protection status
            var rockContext = new RockContext();
            var people = ProtectionEvaluator.GetPeopleByProtectionStatus(rockContext);

            context.Result += "Evaluating " + people.Count + " protection status'";
            rockContext.SaveChanges();

            //Evaluate All
            var message = ProtectionEvaluator.EvaluateAll(rockContext, people);
            context.Result = message;

            rockContext.SaveChanges();
        }
    }
}
