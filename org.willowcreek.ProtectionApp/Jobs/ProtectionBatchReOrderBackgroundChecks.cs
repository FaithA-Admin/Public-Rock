using Quartz;
using org.willowcreek.ProtectionApp.Logic;
using Rock.Data;
using System;
using org.willowcreek.ProtectionApp.Data;

namespace org.willowcreek.ProtectionApp.Jobs
{
    class ProtectionBatchReOrderBackgroundChecks : IJob
    {
        /// <summary>
        /// Job that batch re-orders all expired background checks of volunteers that belong to any volunteer group within Rock.
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            //Get volunteers that expire next week
            var paContext = new ProtectionAppContext();
            var activeVolunteers = ProtectionRerunBackgroundChecks.GetExpiringVolunteersByBC(paContext);

            if (activeVolunteers.Count > 0)
            {
                //Fire off background checks for them
                var message = ProtectionRerunBackgroundChecks.RerunAll(paContext, activeVolunteers);
                context.Result = message;
            }
            else
            {
                context.Result = "No active volunteer's expired background checks were found.";
            }

            paContext.SaveChanges();
        }
    }
}