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

namespace org.willowcreek.YouthCovenant.Logic
{
    public static class YouthCovenantEvaluator
    {
        private const string YOUTH_COVENANT_STATUS_KEY = "YouthCovenantStatus";
        private const string YOUTH_COVENANT_DATE_KEY = "YouthCovenantDate";
        private const string YOUTH_COVENANT_REFERENCE_KEY = "YouthCovenantReference1";
        private const string YOUTH_COVENANT_REFERENCE_DATE_KEY = "YouthCovenantReference1Date";
        private const string YOUTH_COVENANT_PROCESS_INITIATED_KEY = "YouthCovenantProcessInitiated";
        private const string YOUTH_COVENANT_REFERENCE_LIST_KEY = "YouthCovenantReferenceList";

        public static void SetYouthCovenantStatus(this Person personApplicant, ref List<string> personChanges, RockContext rockContext, Guid? overrideStatus = null)
        {
            if ( personApplicant.Attributes == null )
            {
                personApplicant.LoadAttributes();
            }

            var statusGuid = string.Empty;

            if ( overrideStatus != null )
            {
                statusGuid = overrideStatus.Value.ToString();
            }
            else
            {
                var youthCovenantStatus = personApplicant.AttributeValues[YOUTH_COVENANT_STATUS_KEY].Value.AsGuidOrNull();

                if ( youthCovenantStatus != DefinedValueGuids.YouthCovenantStatus.NEEDS_REVIEW.AsGuid()
                    && youthCovenantStatus != DefinedValueGuids.YouthCovenantStatus.INELIGIBLE.AsGuid() )
                {
                    var globalAttributes = GlobalAttributesCache.Read();
                    var daysToWaitWithNoActivity = Convert.ToInt32( globalAttributes.GetValue( "ProtectionWorkflowExpirationNoActivity" ) );
                    var daysToWaitWithActivity = Convert.ToInt32( globalAttributes.GetValue( "ProtectionWorkflowExpirationSomeActivity" ) );
                    var expirationDate = personApplicant.GraduationYear == null ? new DateTime( 3000, 1, 1 ) : new DateTime( personApplicant.GraduationYear.Value - 4, 9, 1 );
                    var processInitiatedDate = personApplicant.AttributeValues[YOUTH_COVENANT_PROCESS_INITIATED_KEY].ValueAsType as DateTime?;
                    var referenceDate = personApplicant.AttributeValues[YOUTH_COVENANT_REFERENCE_DATE_KEY].ValueAsType as DateTime?;
                    var youthCovenantDate = personApplicant.AttributeValues[YOUTH_COVENANT_DATE_KEY].ValueAsType as DateTime?;

                    // If there are no values at all
                    if ( processInitiatedDate == null && youthCovenantDate == null && referenceDate == null )
                    {
                        statusGuid = null;
                    }
                    // If the current date is past the allowed time limit based on how much activity the applicant did, or the applicant's graduation date indicates they are too old
                    else if ( expirationDate <= DateTime.Now
                        || ( processInitiatedDate != null && youthCovenantStatus == DefinedValueGuids.YouthCovenantStatus.PROCESS_INITIATED.AsGuid() && processInitiatedDate.Value.AddDays( daysToWaitWithNoActivity ) <= DateTime.Now )
                        || ( processInitiatedDate != null && youthCovenantStatus == DefinedValueGuids.YouthCovenantStatus.IN_PROGRESS.AsGuid() && processInitiatedDate.Value.AddDays( daysToWaitWithActivity ) <= DateTime.Now )
                        )
                    {
                        statusGuid = DefinedValueGuids.YouthCovenantStatus.EXPIRED;
                    }
                    // If the youth covenant and reference have been approved
                    else if ( youthCovenantDate != null && referenceDate != null )
                    {
                        statusGuid = DefinedValueGuids.YouthCovenantStatus.APPROVED;

                        // Erase the Process Initiated Date
                        History.EvaluateChange( personChanges, YOUTH_COVENANT_PROCESS_INITIATED_KEY, personApplicant.AttributeValues[YOUTH_COVENANT_PROCESS_INITIATED_KEY].ValueFormatted, null );
                        personApplicant.SetAttributeValue( YOUTH_COVENANT_PROCESS_INITIATED_KEY, null );
                    }
                    // If a reference was submitted but not auto-approved
                    else if ( !string.IsNullOrWhiteSpace( personApplicant.AttributeValues[YOUTH_COVENANT_REFERENCE_KEY].Value ) && referenceDate == null )
                    {
                        statusGuid = DefinedValueGuids.YouthCovenantStatus.NEEDS_REVIEW;
                    }
                    // If the process was initiated but no values have come in yet
                    else if ( processInitiatedDate != null && youthCovenantDate == null && string.IsNullOrWhiteSpace( personApplicant.AttributeValues[YOUTH_COVENANT_REFERENCE_LIST_KEY].Value ) && referenceDate == null )
                    {
                        statusGuid = DefinedValueGuids.YouthCovenantStatus.PROCESS_INITIATED;
                    }
                    // Any other combination of values
                    else
                    {
                        statusGuid = DefinedValueGuids.YouthCovenantStatus.IN_PROGRESS;
                    }
                }
            }

            DefinedValueCache.FlushCache( statusGuid ); // This may no longer be necessary, but for now it is required because the next line returns null without it. -CM 2/22/2017
            var definedValue = DefinedValueCache.Read( statusGuid );
            if ( definedValue != null )
            {
                var statusText = definedValue.Value;
                History.EvaluateChange( personChanges, YOUTH_COVENANT_STATUS_KEY, personApplicant.AttributeValues[YOUTH_COVENANT_STATUS_KEY].ValueFormatted, statusText );
                personApplicant.SetAttributeValue( YOUTH_COVENANT_STATUS_KEY, statusGuid );
            }
        }
    }
}
