using Quartz;

namespace org.willowcreek.ProtectionApp.Jobs
{
    class ProtectionReviewer : IJob
    {

        public void Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            /*TODO:
                    Call the ProtectionEvaluator class to evaluate protection.
                    May want to do two things: 1) recent protection applications (workflows) quickly.
                                               2) all people with "P" badge of any color. review for expire, needs review, approved (rollback because workflow expired)
             */

        }

    }
}
