using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Workflow;

using org.willowcreek.SystemGuid;
using org.willowcreek.YouthCovenant.Logic;

namespace org.willowcreek.YouthCovenant.Workflow.Action
{
    [ActionCategory( "Protection" )]
    [Description( "Calculate the proper Youth Covenant Status" )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Update Youth Covenant Status" )]

    [WorkflowAttribute( "Applicant", "(IN) Person who the application process will be started for.", true, "", "", 1, null, new string[] { "Rock.Field.Types.PersonFieldType" } )]
    [DefinedValueField(SystemGuid.DefinedTypeGuids.YOUTH_COVENANT_STATUS, "Override Status", "(IN) This field manually sets the status to the chosen value rather than determining it", false, order: 2)]
    public class UpdateYouthCovenantStatus : ActionComponent
    {
        private const string YOUTH_COVENANT_STATUS_KEY = "YouthCovenantStatus";
        private const string YOUTH_COVENANT_PROCESS_INITIATED_KEY = "YouthCovenantProcessInitiated";

        public override bool Execute( RockContext rockContext, WorkflowAction action, object entity, out List<string> errorMessages )
        {
            errorMessages = new List<string>();
            try
            {
                var personChanges = new List<string>();
                var ps = new PersonAliasService( rockContext );
                var applicant = ps.Get( GetAttributeValue( action, "Applicant", true ).AsGuid() );
                var personApplicant = applicant.Person;
                personApplicant.SetYouthCovenantStatus( ref personChanges, rockContext, GetAttributeValue( action, "OverrideStatus", true).AsGuidOrNull() );

                HistoryService.SaveChanges( rockContext, typeof( Person ), Rock.SystemGuid.Category.HISTORY_PERSON_ACTIVITY.AsGuid(), personApplicant.Id, personChanges, true );
                personApplicant.SaveAttributeValue( YOUTH_COVENANT_STATUS_KEY );
                personApplicant.SaveAttributeValue( YOUTH_COVENANT_PROCESS_INITIATED_KEY );

                return true;
            }
            catch ( Exception ex )
            {
                //while ( ex.InnerException != null )
                //{
                //    ex = ex.InnerException;
                //}
                ExceptionLogService.LogException( ex, System.Web.HttpContext.Current );
                errorMessages.Add( ex.Message );
                return false;
            }
        }
    }
}
