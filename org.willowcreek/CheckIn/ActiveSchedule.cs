using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock.Data;
using Rock.Model;

namespace org.willowcreek.CheckIn
{
    public static class ActiveSchedule
    {
        public static Tuple<int, DateTime> GetScheduleParameters( int checkInTypeId, string nowString = null )
        {
            DateTime now; //new DateTime( 2017, 5, 21, 10, 5, 0 );
            if (!DateTime.TryParseExact( nowString, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out now ))
            {
                now = DateTime.Now;
            }

            using ( var rockContext = new RockContext() )
            {
                // Get all the schedules associated with the selected Check-In Configuration
                var schedules = GetSchedules( rockContext, checkInTypeId );

                if ( schedules.Any() )
                {
                    var schedulesWithTimes = schedules.Select( s => new { Schedule = s, StartTimes = s.GetCheckInTimes( now.Date ).Select( t => t.CheckInStart ).ToList() } ).Where( s => s.StartTimes.Any() ).ToList();

                    if ( schedulesWithTimes.Any() )
                    {
                        // Get all schedules that have started checking in today
                        var startedSchedules = schedulesWithTimes.Where( s => s.StartTimes.Any( t => t < now ) ).ToList();

                        if ( startedSchedules.Any() )
                        {
                            // Return the schedule that started checking in most recently
                            return startedSchedules.Select( s => new { s.Schedule, LastStartTime = s.StartTimes.OrderByDescending( x => x ).First() } )
                                .OrderByDescending( x => x.LastStartTime )
                                .Select( x => new Tuple<int, DateTime>( x.Schedule.Id, x.LastStartTime ) )
                                .First();
                        }

                        // If none have started checking in yet, get the next to start
                        return schedulesWithTimes.Select( s => new { s.Schedule, FirstStartTime = s.StartTimes.OrderBy( x => x ).First() } )
                            .OrderBy( x => x.FirstStartTime )
                            .Select( x => new Tuple<int, DateTime>( x.Schedule.Id, x.FirstStartTime ) )
                            .First();
                    }
                }
            }

            return null;
        }

        public static List<Schedule> GetSchedules(RockContext rockContext, int checkInTypeId)
        {
            var descendantGroupTypeIds = new GroupTypeService( rockContext ).GetAllAssociatedDescendents( checkInTypeId ).Select( a => a.Id );
            var schedules = new GroupLocationService( rockContext )
                .Queryable().AsNoTracking()
                .Where( a =>
                    a.Group.GroupType.Id == checkInTypeId ||
                    descendantGroupTypeIds.Contains( a.Group.GroupTypeId ) )
                .SelectMany( a => a.Schedules )
                .Distinct()
                .ToList();
            return schedules;
        }

        public static List<Schedule> GetSchedules( RockContext rockContext, int checkInTypeId, DateTime date )
        {
            var schedules = GetSchedules( rockContext, checkInTypeId );
            if ( schedules.Any() )
            {
                var schedulesWithTimes = schedules.Select( s => new { Schedule = s, StartTimes = s.GetCheckInTimes( date.Date ).Select( t => t.CheckInStart ).ToList() } ).Where( s => s.StartTimes.Any() ).ToList();
                return schedulesWithTimes.Select(x => x.Schedule).ToList();
            }
            return null;
        }
    }
}
