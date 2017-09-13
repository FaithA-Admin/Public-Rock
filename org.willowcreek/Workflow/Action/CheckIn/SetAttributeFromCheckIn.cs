using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;

using Rock.Attribute;
using Rock.CheckIn;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace Rock.Workflow.Action.CheckIn
{
    /// <summary>
    /// Saves the selected check-in data as attendance
    /// </summary>
    [ActionCategory( "Check-In" )]
    [Description( "" )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Set Attribute From CheckIn" )]
    [TextField( "CheckIn Custom Value Key", key: "CustomKey", order:1 )]
    [WorkflowAttribute("Attribute", "The workflow attribute you want to set")]
    public class SetAttributeFromCheckIn : CheckInActionComponent
    {
        /// <summary>
        /// Executes the specified workflow.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="action">The workflow action.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="errorMessages">The error messages.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public override bool Execute( RockContext rockContext, Model.WorkflowAction action, Object entity, out List<string> errorMessages )
        {
            var checkInState = GetCheckInState( entity, out errorMessages );

            var attribute = AttributeCache.Read( GetAttributeValue( action, "Attribute" ).AsGuid(), rockContext );
            if ( attribute != null )
            {
                var customKey = GetAttributeValue( action, "CustomKey", true ).ResolveMergeFields( GetMergeFields( action ) );
                object value;
                if ( checkInState.CheckIn.CustomValues.TryGetValue( customKey, out value ) )
                {
                    SetWorkflowAttributeValue( action, attribute.Guid, value.ToString() );
                    action.AddLogEntry( string.Format( "Set '{0}' attribute to '{1}'.", attribute.Name, value ) );
                }
            }

            return true;
        }
    }
}