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
    [ExportMetadata( "ComponentName", "Set Attendance Attribute" )]
    //[TextField( "CheckIn Custom Value Key", key: "CustomKey", order:1 )]
    [AttributeField( SystemGuid.EntityType.ATTENDANCE, "Attribute", order:1)]
    [WorkflowTextOrAttribute("Value", "Attribute Value", "The value to put in the Attendance Attribute", true, "", "", 2, "Source")]
    public class SetAttendanceAttribute : CheckInActionComponent
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
            if ( checkInState != null )
            {
                var attendanceService = new AttendanceService( rockContext );
                var personAliasService = new PersonAliasService( rockContext );

                var family = checkInState.CheckIn.CurrentFamily;
                if ( family != null )
                {
                    foreach ( var person in family.GetPeople( true ) )
                    {
                        foreach ( var groupType in person.GetGroupTypes( true ) )
                        {
                            foreach ( var checkInGroup in groupType.GetGroups( true ) )
                            {
                                foreach ( var location in checkInGroup.GetLocations( true ) )
                                {
                                    foreach ( var schedule in location.GetSchedules( true ) )
                                    {
                                        var attendance = (from a in attendanceService.Queryable()
                                                         join pa in personAliasService.Queryable() on a.PersonAliasId equals pa.Id
                                                         where a.LocationId == location.Location.Id
                                                                && a.ScheduleId == schedule.Schedule.Id
                                                                && a.GroupId == checkInGroup.Group.Id
                                                                && pa.PersonId == person.Person.Id
                                                         orderby a.StartDateTime descending
                                                         select a).FirstOrDefault();

                                        attendance.LoadAttributes();

                                        var mergeFields = GetMergeFields( action );
                                        var attributeValue = GetAttributeValue( action, "Source", true ).ResolveMergeFields( mergeFields );
                                        var key = AttributeCache.Read( GetAttributeValue(action, "Attribute" ).AsGuid() ).Key;
                                        attendance.SetAttributeValue( key, attributeValue );
                                        attendance.SaveAttributeValues( rockContext );
                                    }
                                }
                            }
                        }
                    }
                }

                rockContext.SaveChanges();
                return true;
            }

            return false;
        }
    }
}