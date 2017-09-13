using Quartz;
using Rock.Data;
using Rock.Model;
using Rock.Attribute;
using System;
using System.Linq;
using Rock;
using Rock.Utility;

namespace org.willowcreek.Jobs
{
    class SyncSecurityRoleMembers : IJob
    {
        public SyncSecurityRoleMembers()
        {
        }

        public void Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;

            var rockContext = new RockContext();

            var securityGroups = rockContext.Database.SqlQuery<string>( "wcJob_SyncSecurityRoleMembers" ).FirstOrDefault();

            if( !securityGroups.IsNullOrWhiteSpace() )
            {
                foreach ( var group in securityGroups.Split( ',' ) )
                {
                    if ( !group.IsNullOrWhiteSpace() && group.AsIntegerOrNull() != null )
                    {
                        //Flush security roles in group
                        Rock.Security.Role.Flush( group.AsInteger() );
                    }
                }
            }
        }
    }
}
