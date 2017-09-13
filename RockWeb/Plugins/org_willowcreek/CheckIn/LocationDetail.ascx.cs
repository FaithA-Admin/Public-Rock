using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Text.RegularExpressions;

using Rock;
using Rock.Data;
using Rock.Model;
using Rock.CheckIn;
using Rock.Web.Cache;
using org.willowcreek.SystemGuid;
using System.Data.Entity;

using Rock.Web.UI.Controls;
using System.Linq.Dynamic;
using org.willowcreek.CheckIn;
using System.Net;
using System.Net.Sockets;
using Rock.Constants;
using System.Text;
using Rock.Attribute;

namespace RockWeb.Plugins.org_willowcreek.CheckIn
{
    [DisplayName( "Location Detail" )]
    [Category( "org_willowcreek > Check-in" )]
    [Description( "Lists people checked into a location and allows check-in confirmation and editing" )]
    [IntegerField( "Refresh Interval (Seconds)", defaultValue: 10, key: "RefreshInterval" )]
    public partial class LocationDetail : Rock.Web.UI.RockBlock
    {
        private int? _checkInTypeId;
        private int? _locationId;
        private int? _campusId;
        private int? _scheduleId;
        private DateTime? _startDateTime;
        private Location _enRouteLocation;

        private const string EN_ROUTE = "En Route";
        private const string IN_ROOM = "In Room";
        private const string HEALTH_NOTES = "Health Notes";
        private const string CHECKED_OUT = "Checked Out";

        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            timer.Interval = GetAttributeValue( "RefreshInterval" ).AsInteger() * 1000;
        }
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );
            
            using ( var rockContext = new RockContext() )
            {
                var locationService = new LocationService( rockContext );
                _checkInTypeId = PageParameter( "CheckinTypeId" ).AsIntegerOrNull();
                _locationId = PageParameter( "LocationId" ).AsIntegerOrNull();
                _campusId = _locationId.HasValue ? locationService.Get( _locationId.Value ).CampusId : null;
                _enRouteLocation = locationService.Get( Guid.Parse( "6318116F-3253-4E50-9B66-9995343C11A8" ) );

                if ( _checkInTypeId.HasValue && _locationId.HasValue && _enRouteLocation != null )
                {
                    lLocation.Text = locationService.Get( _locationId.Value ).Name;
                    var schedule = ActiveSchedule.GetScheduleParameters( _checkInTypeId.Value, PageParameter( "Now" ) );
                    if ( schedule != null )
                    {
                        _scheduleId = schedule.Item1;
                        _startDateTime = schedule.Item2;
                    }

                    if ( !Page.IsPostBack )
                    {

                        BindEnRouteGrid();
                        BindInRoomGrid();
                        BindHealthNotesGrid();
                        BindCheckedOutGrid();
                    }
                }
            }
        }

        protected override void OnPreRender( EventArgs e )
        {
            base.OnPreRender( e );

            string activeTab = hfActiveTab.Value;
            ShowHideTab( activeTab == EN_ROUTE || activeTab == string.Empty, liEnRoute );
            ShowHideTab( activeTab == EN_ROUTE || activeTab == string.Empty, divEnRoute );
            ShowHideTab( activeTab == IN_ROOM, liInRoom );
            ShowHideTab( activeTab == IN_ROOM, divInRoom );
            ShowHideTab( activeTab == HEALTH_NOTES, liHealthNotes );
            ShowHideTab( activeTab == HEALTH_NOTES, divHealthNotes );
            ShowHideTab( activeTab == CHECKED_OUT, liCheckedOut );
            ShowHideTab( activeTab == CHECKED_OUT, divCheckedOut );

            // Don't refresh the screen while the dialog is open, it loses focus in dropdown menus
            timer.Enabled = string.IsNullOrEmpty( hfActiveDialog.Value );
        }

        private void ShowHideTab( bool show, System.Web.UI.HtmlControls.HtmlGenericControl control )
        {
            if ( show )
            {
                control.AddCssClass( "active" );
            }
            else
            {
                control.RemoveCssClass( "active" );
            }
        }

        private void BindEnRouteGrid()
        {
            using ( var rockContext = new RockContext() )
            {
                // All people who are en route for the selected schedule and eligible to come in this room
                var dataSource = from a in new AttendanceService( rockContext ).Queryable().AsNoTracking()
                                 join gl in new GroupLocationService( rockContext ).Queryable().AsNoTracking() on a.GroupId equals gl.GroupId
                                 where a.LocationId == _enRouteLocation.Id
                                      && a.ScheduleId == _scheduleId.Value
                                      && a.StartDateTime >= _startDateTime.Value
                                      && a.EndDateTime == null
                                      && gl.LocationId == _locationId.Value
                                 orderby a.PersonAlias.Person.NickName, a.AttendanceCode.Code, a.Id
                                 select new
                                 {
                                     a.Id,
                                     PersonId = a.PersonAlias.Person.Id,
                                     Name = a.PersonAlias.Person.NickName,
                                     Group = a.Group.Name,
                                     a.AttendanceCode.Code,
                                     a.Group.GroupTypeId,
                                     a.GroupId
                                 };

                gEnRoute.DataSource = dataSource.ToList();
                gEnRoute.DataBind();
            }
        }

        private void BindInRoomGrid()
        {
            using ( var rockContext = new RockContext() )
            {
                // All people who are checked into this room in the selected schedule 
                var dataSource = from a in new AttendanceService( rockContext ).Queryable().AsNoTracking()
                                 where a.LocationId == _locationId.Value
                                      && a.ScheduleId == _scheduleId.Value
                                      && a.StartDateTime >= _startDateTime.Value
                                      && a.EndDateTime == null
                                 orderby a.PersonAlias.Person.NickName, a.AttendanceCode.Code, a.Id
                                 select new
                                 {
                                     a.Id,
                                     PersonId = a.PersonAlias.Person.Id,
                                     Name = a.PersonAlias.Person.NickName,
                                     Group = a.Group.Name,
                                     a.AttendanceCode.Code,
                                     a.Group.GroupTypeId,
                                     a.GroupId
                                 };

                gInRoom.DataSource = dataSource.ToList();
                gInRoom.DataBind();
            }
        }

        private void BindHealthNotesGrid()
        {
            using ( var rockContext = new RockContext() )
            {
                var dataSource = from a in new AttendanceService( rockContext ).Queryable().AsNoTracking()
                                 join gl in new GroupLocationService( rockContext ).Queryable().AsNoTracking() on a.GroupId equals gl.GroupId
                                 join pa in new PersonAliasService( rockContext ).Queryable().AsNoTracking() on a.PersonAliasId equals pa.Id
                                 join av in new AttributeValueService( rockContext ).Queryable().AsNoTracking() on pa.PersonId equals av.EntityId
                                 join at in new AttributeService (rockContext).Queryable().AsNoTracking() on av.AttributeId equals at.Id
                                 where a.ScheduleId == _scheduleId.Value
                                    && a.StartDateTime >= _startDateTime.Value
                                    && a.EndDateTime == null
                                    && gl.LocationId == _locationId.Value // Checked into a group associated with this room
                                    && at.Key == "Allergy"
                                    && !string.IsNullOrEmpty(av.Value)
                                 orderby a.PersonAlias.Person.NickName, a.AttendanceCode.Code, a.Id
                                 select new
                                 {
                                     a.Id,
                                     PersonId = a.PersonAlias.PersonId,
                                     Name = a.PersonAlias.Person.NickName,
                                     Group = a.Group.Name,
                                     a.AttendanceCode.Code,
                                     a.Group.GroupTypeId,
                                     a.GroupId,
                                     Note = av.Value
                                 };

                gHealthNotes.DataSource = dataSource.ToList();
                gHealthNotes.DataBind();
            }
        }

        protected void gHealthNotes_GridRebind( object sender, GridRebindEventArgs e )
        {
            BindHealthNotesGrid();
        }

        private void BindCheckedOutGrid()
        {
            using ( var rockContext = new RockContext() )
            {
                var dataSource = from a in new AttendanceService( rockContext ).Queryable().AsNoTracking()
                                 join gl in new GroupLocationService( rockContext ).Queryable().AsNoTracking() on a.GroupId equals gl.GroupId
                                 where a.ScheduleId == _scheduleId.Value
                                      && a.StartDateTime >= _startDateTime.Value
                                      && a.EndDateTime != null
                                      && gl.LocationId == _locationId.Value // Checked into a group associated with this room
                                 orderby a.PersonAlias.Person.NickName, a.AttendanceCode.Code, a.Id
                                 select new
                                 {
                                     a.Id,
                                     PersonId = a.PersonAlias.PersonId,
                                     Name = a.PersonAlias.Person.NickName,
                                     Group = a.Group.Name,
                                     a.AttendanceCode.Code,
                                     a.Group.GroupTypeId,
                                     a.GroupId
                                 };

                gCheckedOut.DataSource = dataSource.ToList();
                gCheckedOut.DataBind();
            }
        }

        protected void gEnRoute_GridRebind( object sender, GridRebindEventArgs e )
        {
            BindEnRouteGrid();
        }

        private void ShowEditDialog( RowEventArgs e, bool editGroups = true )
        {
            ddlArea.Enabled = editGroups;
            ddlSmallGroup.Enabled = editGroups;

            var attendanceId = e.RowKeyValues[0] as int?;
            var personId = e.RowKeyValues[1] as int?;
            var groupId = e.RowKeyValues[2] as int?;
            var groupTypeId = e.RowKeyValues[3] as int?;

            hfAttendanceId.Value = attendanceId.ToString();
            hfPersonId.Value = personId.ToString();
            hfGroupId.Value = groupId.ToString();
            hfGroupTypeId.Value = groupTypeId.ToString();

            if ( personId.HasValue )
            {
                using ( var rockContext = new RockContext() )
                {
                    var personService = new PersonService( rockContext );
                    var person = personService.Get( personId.Value );
                    person.LoadAttributes();
                    tbNickName.Text = person.NickName;
                    tbHealthNote.Text = person.AttributeValues["Allergy"].Value;

                    tbNotes.Text = string.Empty;
                    var noteType = new NoteTypeService( rockContext ).Get( Guid.Parse( NoteTypeGuids.CHECKIN ) );
                    var noteService = new NoteService( rockContext );
                    var note = noteService.Get( noteType.Id, attendanceId.Value ).OrderByDescending(x => x.CreatedDateTime).FirstOrDefault();
                    if ( note != null && note.Text != null )
                    {
                        tbNotes.Text = note.Text;
                    }

                    var groupTypeService = new GroupTypeService( rockContext );
                    var checkInConfig = groupTypeService.Get( _checkInTypeId.Value );
                    var areaDataSource = checkInConfig.ChildGroupTypes;

                    var attendanceService = new AttendanceService( rockContext );
                    var attendance = attendanceService.Get( attendanceId.Value );
                    var locationId = attendanceService.Queryable().AsNoTracking().Where( x => x.Id == attendanceId.Value ).Select( x => x.LocationId ).FirstOrDefault();
                    if ( locationId.HasValue )
                    {
                        var limitedAreaDataSource = areaDataSource.Where( x => x.Groups.Any( g => g.GroupLocations.Any( l => l.LocationId == locationId.Value ) )
                            || x.ChildGroupTypes.Any( t => t.Groups.Any( g => g.GroupLocations.Any( l => l.LocationId == locationId.Value ) ) ) ).ToList();

                        // Don't update the list if the current area doesn't have any locations (usually because En Route is not in use at this location)
                        if (limitedAreaDataSource.Any())
                        {
                            areaDataSource = limitedAreaDataSource;
                        }
                    }
                    ddlArea.DataSource = areaDataSource;
                    ddlArea.DataBind();

                    if ( groupTypeId.HasValue )
                    {
                        var parent = groupTypeService.Get( groupTypeId.Value ).ParentGroupTypes.Where( t => t.TakesAttendance ).SingleOrDefault();
                        
                        if ( areaDataSource.Any(x => x.Id == groupTypeId.Value) )
                        {
                            ddlArea.SelectedValue = groupTypeId.ToString();
                        }
                        else if ( parent != null && areaDataSource.Any( x => x.Id == parent.Id ) )
                        {
                            ddlArea.SelectedValue = parent.Id.ToString();
                        }

                        BindSmallGroupDDL();
                    }

                    // Adults that checked in the child
                    var checkInAdults = attendance.SearchResultGroup.Members
                        .Where( x => x.Person.GetFamilyRole( rockContext ).Guid == Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT.AsGuid()
                            && x.Person.PhoneNumbers.Any())
                        .Select( x => new { ID = x.PersonId, Name = x.Person.FullName, x.Person.PhoneNumbers, Sequence = 1, Caption = "" } ).ToList();

                    // Adults in the child's family who are not already in the list
                    var parents = person.GetFamilyMembers( false, rockContext ).ToList()
                        .Where( x => x.Person.GetFamilyRole( rockContext ).Guid == Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT.AsGuid()
                            && x.Person.PhoneNumbers.Any()
                            && !checkInAdults.Where(a => a.ID == x.PersonId).Any())
                        .Select(x => new { ID = x.PersonId, Name = x.Person.FullName, x.Person.PhoneNumbers, Sequence = 2, Caption = " <small>(Parent)</small>" })
                        .ToList();

                    var adults = checkInAdults.Concat( parents ).OrderBy( x => x.Sequence ).ThenBy( x => x.ID ).ToList();

                    if ( adults.Any() )
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine( "<ul style=\"padding-left: 0px; margin: 0px; list-style:none;\">" );
                        foreach ( var adult in adults )
                        {
                            sb.AppendLine( "<li style=\"float:left; margin-right: 15px; text-decoration:none; margin-bottom: 5px;\">" );
                            sb.AppendLine( adult.Name + adult.Caption );
                            foreach ( var phone in adult.PhoneNumbers )
                            {
                                sb.AppendLine( "<br>" + phone.NumberFormatted + " <small>" + phone.NumberTypeValue + "</small>" );
                            }
                            sb.AppendLine( "</li>" );
                        }
                        sb.AppendLine( "</ul>" );
                        lAdults.Text = sb.ToString();
                        //sb.Append( "<table><tr>" );
                        //foreach ( var adult in adults )
                        //{
                        //    sb.AppendLine( "<td style=\"padding-right:10px; vertical-align:top;\"><small>" + adult.Caption + "</small><br>" + adult.Name );
                        //    foreach ( var phone in adult.PhoneNumbers )
                        //    {
                        //        sb.AppendLine( "<br>" + phone.NumberFormatted + " <small>" + phone.NumberTypeValue + "</small>" );
                        //    }
                        //    sb.AppendLine( "</td>" );
                        //}
                        //sb.Append( "</tr></table>" );
                        //lAdults.Text = sb.ToString();
                    }
                    else
                    {
                        lAdults.Text = "None";
                    }
                }
            }

            // Printer list
            var printerGuid = Rock.SystemGuid.DefinedValue.DEVICE_TYPE_PRINTER.AsGuid();
            ddlKiosk.Items.Clear();
            using ( var rockContext = new RockContext() )
            {
                ddlKiosk.DataSource = new DeviceService( rockContext )
                    .Queryable().AsNoTracking()
                    .Where( d =>  d.DeviceType.Guid.Equals( printerGuid ) && d.IPAddress.Length > 0 ) 
                    .OrderBy( d => d.Name )
                    .Select( d => new
                    {
                        d.Id,
                        d.Name
                    } )
                    .ToList();
            }
            ddlKiosk.DataBind();
            ddlKiosk.Items.Insert( 0, new ListItem( None.Text, None.IdValue ) );

            var selectedPrinter = Session["LocationDetail_SelectedPrinter"];
            if (selectedPrinter != null && ddlKiosk.Items.FindByValue(selectedPrinter.ToString()) != null)
            {
                ddlKiosk.SelectedValue = selectedPrinter.ToString();
            }
            btnPrint.Enabled = ( ddlKiosk.SelectedValue.AsIntegerOrNull() > 0 );

            ShowDialog( "Edit" );
        }

        protected void gEnRoute_Edit( object sender, RowEventArgs e )
        {
            ShowEditDialog( e );
        }

        protected void gInRoom_Edit( object sender, RowEventArgs e )
        {
            ShowEditDialog( e );
        }

        protected void gHealthNotes_Edit( object sender, RowEventArgs e )
        {
            ShowEditDialog( e );
        }

        protected void gCheckedOut_Edit( object sender, RowEventArgs e )
        {
            ShowEditDialog( e, false );
        }

        protected void ddlArea_SelectedIndexChanged( object sender, EventArgs e )
        {
            BindSmallGroupDDL();
        }

        private void BindSmallGroupDDL()
        {
            using ( var rockContext = new RockContext() )
            {
                var groupTypeService = new GroupTypeService( rockContext );

                var groupType = groupTypeService.Get( ddlArea.SelectedValue.AsInteger() );

                // Get all the small groups that are scheduled for the current time
                var dsSmallGroup = groupType.Groups.Where( g => g.GroupLocations.Any( l => l.Schedules.Any( s => s.Id == _scheduleId.Value ) ) ).ToList();

                foreach (var gt in groupType.ChildGroupTypes)
                {
                    dsSmallGroup.InsertRange( 0, gt.Groups );
                }

                ddlSmallGroup.DataSource = dsSmallGroup;
                ddlSmallGroup.DataBind();

                var groupId = hfGroupId.Value.AsIntegerOrNull();
                if ( groupId.HasValue && dsSmallGroup.Any( x => x.Id == groupId.Value ) )
                {
                    ddlSmallGroup.SelectedValue = groupId.Value.ToString();
                }
            }
        }

        private void Save()
        {
            var personId = hfPersonId.Value.AsIntegerOrNull();
            if ( personId.HasValue )
            {
                using ( var rockContext = new RockContext() )
                {
                    var changes = new List<string>();

                    var personService = new PersonService( rockContext );
                    var person = personService.Get( personId.Value );
                    person.LoadAttributes();

                    History.EvaluateChange( changes, "Nick Name", person.NickName, tbNickName.Text );
                    person.NickName = tbNickName.Text;

                    History.EvaluateChange( changes, "Allergy", person.AttributeValues["Allergy"].Value, tbHealthNote.Text );
                    person.AttributeValues["Allergy"].Value = tbHealthNote.Text;

                    var attendanceId = hfAttendanceId.Value.AsIntegerOrNull();
                    var groupId = hfGroupId.Value.AsIntegerOrNull();

                    if ( attendanceId.HasValue && groupId.HasValue )
                    {
                        var noteType = new NoteTypeService( rockContext ).Get( Guid.Parse( NoteTypeGuids.CHECKIN ) );
                        var noteService = new NoteService( rockContext );

                        // Update the existing note if there is one, otherwise create one
                        var note = noteService.Get( noteType.Id, attendanceId.Value ).OrderByDescending( x => x.CreatedDateTime ).FirstOrDefault();
                        if ( note != null )
                        {
                            note.Text = tbNotes.Text;
                        }
                        else if ( !string.IsNullOrWhiteSpace(tbNotes.Text) )
                        {
                            note = new Note
                            {
                                NoteTypeId = noteType.Id,
                                EntityId = attendanceId.Value,
                                Text = tbNotes.Text,
                                Caption = "",
                                IsAlert = false
                            };
                            noteService.Add( note );
                        }

                        var attendanceService = new AttendanceService( rockContext );
                        var attendance = attendanceService.Get( attendanceId.Value );

                        attendance.GroupId = ddlSmallGroup.SelectedValueAsInt();
                    }

                    person.SaveAttributeValue( "Allergy", rockContext );
                    rockContext.SaveChanges();
                }
            }
        }

        protected void dlgEdit_SaveClick( object sender, EventArgs e )
        {
            Save();

            hfAttendanceId.Value = string.Empty;
            hfPersonId.Value = string.Empty;
            hfGroupId.Value = string.Empty;
            hfGroupTypeId.Value = string.Empty;

            HideDialog();
            BindEnRouteGrid();
            BindInRoomGrid();
            BindHealthNotesGrid();
            BindCheckedOutGrid();
        }

        protected void gEnRoute_CheckIn( object sender, RowEventArgs e )
        {
            if ( _locationId != null )
            {
                var attendanceId = e.RowKeyValues["Id"] as int?;
                if ( attendanceId != null )
                {
                    using ( var rockContext = new RockContext() )
                    {
                        var attendanceService = new AttendanceService( rockContext );
                        var attendance = attendanceService.Get( attendanceId.Value );
                        attendance.LocationId = _locationId;
                        attendance.CampusId = _campusId;
                        rockContext.SaveChanges();
                        BindEnRouteGrid();
                        BindInRoomGrid();
                    }
                }
            }
        }

        protected void gCheckedOut_CheckIn( object sender, RowEventArgs e )
        {
            if ( _locationId != null )
            {
                var attendanceId = e.RowKeyValues["Id"] as int?;
                if ( attendanceId != null )
                {
                    using ( var rockContext = new RockContext() )
                    {
                        var attendanceService = new AttendanceService( rockContext );
                        var attendance = attendanceService.Get( attendanceId.Value );
                        attendance.EndDateTime = null;
                        attendance.LocationId = _locationId;
                        attendance.CampusId = _campusId;
                        rockContext.SaveChanges();
                        BindCheckedOutGrid();
                        BindInRoomGrid();
                        BindHealthNotesGrid();
                    }
                }
            }
        }

        protected void gEnRoute_CheckOut( object sender, RowEventArgs e )
        {
            var attendanceId = e.RowKeyValues["Id"] as int?;
            if ( attendanceId != null )
            {
                using ( var rockContext = new RockContext() )
                {
                    var attendanceService = new AttendanceService( rockContext );
                    var attendance = attendanceService.Get( attendanceId.Value );
                    attendance.EndDateTime = DateTime.Now;
                    rockContext.SaveChanges();
                    BindEnRouteGrid();
                    BindCheckedOutGrid();
                    BindHealthNotesGrid();
                }
            }
        }

        protected void gInRoom_GridRebind( object sender, GridRebindEventArgs e )
        {
            BindInRoomGrid();
        }

        protected void gInRoom_EnRoute( object sender, RowEventArgs e )
        {
            if ( _locationId != null )
            {
                var attendanceId = e.RowKeyValues["Id"] as int?;
                if ( attendanceId != null )
                {
                    using ( var rockContext = new RockContext() )
                    {
                        var attendanceService = new AttendanceService( rockContext );
                        var attendance = attendanceService.Get( attendanceId.Value );
                        attendance.LocationId = _enRouteLocation.Id;
                        rockContext.SaveChanges();
                        BindEnRouteGrid();
                        BindInRoomGrid();
                    }
                }
            }
        }

        protected void gCheckedOut_EnRoute( object sender, RowEventArgs e )
        {
            if ( _locationId != null )
            {
                var attendanceId = e.RowKeyValues["Id"] as int?;
                if ( attendanceId != null )
                {
                    using ( var rockContext = new RockContext() )
                    {
                        var attendanceService = new AttendanceService( rockContext );
                        var attendance = attendanceService.Get( attendanceId.Value );
                        attendance.EndDateTime = null;
                        attendance.LocationId = _enRouteLocation.Id;
                        rockContext.SaveChanges();
                        BindEnRouteGrid();
                        BindCheckedOutGrid();
                        BindHealthNotesGrid();
                    }
                }
            }
        }

        protected void gInRoom_CheckOut( object sender, RowEventArgs e )
        {
            var attendanceId = e.RowKeyValues["Id"] as int?;
            if ( attendanceId != null )
            {
                using ( var rockContext = new RockContext() )
                {
                    var attendanceService = new AttendanceService( rockContext );
                    var attendance = attendanceService.Get( attendanceId.Value );
                    attendance.EndDateTime = DateTime.Now;
                    rockContext.SaveChanges();
                    BindInRoomGrid();
                    BindCheckedOutGrid();
                    BindHealthNotesGrid();
                }
            }
        }

        protected void gCheckedOut_GridRebind( object sender, GridRebindEventArgs e )
        {
            BindCheckedOutGrid();
        }



        #region Dialog

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="dialog">The dialog.</param>
        /// <param name="setValues">if set to <c>true</c> [set values].</param>
        private void ShowDialog( string dialog, bool setValues = false )
        {
            hfActiveDialog.Value = dialog.ToUpper().Trim();
            ShowDialog( setValues );
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="setValues">if set to <c>true</c> [set values].</param>
        private void ShowDialog( bool setValues = false )
        {
            switch ( hfActiveDialog.Value )
            {
                case "EDIT":
                    dlgEdit.Show();
                    break;
            }
        }

        /// <summary>
        /// Hides the dialog.
        /// </summary>
        private void HideDialog()
        {
            switch ( hfActiveDialog.Value )
            {
                case "EDIT":
                    dlgEdit.Hide();
                    break;
            }

            hfActiveDialog.Value = string.Empty;
        }

        #endregion

        protected void timer_Tick( object sender, EventArgs e )
        {
            BindEnRouteGrid();
            BindInRoomGrid();
            BindHealthNotesGrid();
            BindCheckedOutGrid();
        }

        protected void btnPrint_Click( object sender, EventArgs e )
        {
            Save();

            try
            {
                var personId = hfPersonId.Value.AsIntegerOrNull();
                var attendanceId = hfAttendanceId.Value.AsIntegerOrNull();
                var groupTypeId = ddlArea.SelectedValue.AsIntegerOrNull(); // hfGroupTypeId.Value.AsIntegerOrNull();
                var groupId = ddlSmallGroup.SelectedValue.AsIntegerOrNull(); // hfGroupId.Value.AsIntegerOrNull();
                hfGroupTypeId.Value = groupTypeId.ToString();
                hfGroupId.Value = groupId.ToString();

                if ( personId.HasValue && attendanceId.HasValue && groupTypeId.HasValue && groupId.HasValue )
                {
                    using ( var rockContext = new RockContext() )
                    {
                        var attendanceService = new AttendanceService( rockContext );
                        var attendance = attendanceService.Get( attendanceId.Value );
                        attendance.LoadAttributes();

                        var commonMergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( null );

                        var schedule = new CheckInSchedule()
                        {
                            Schedule = new ScheduleService( rockContext ).Get( _scheduleId.Value ).Clone( false ),
                            Selected = true
                        };

                        var location = new CheckInLocation()
                        {
                            Location = new LocationService( rockContext ).Get( _locationId.Value ).Clone( false ),
                            Schedules = new List<CheckInSchedule> { schedule },
                            Selected = true
                        };

                        var group = new CheckInGroup
                        {
                            Group = new GroupService( rockContext ).Get( groupId.Value ).Clone( false ),
                            Locations = new List<CheckInLocation> { location },
                            Selected = true
                        };

                        var groupType = new CheckInGroupType
                        {
                            GroupType = GroupTypeCache.Read( groupTypeId.Value ),
                            Groups = new List<CheckInGroup> { group },
                            Labels = new List<CheckInLabel>(),
                            Selected = true
                        };

                        var person = new CheckInPerson
                        {
                            Person = new PersonService( rockContext ).Get( personId.Value ).Clone( false ),
                            SecurityCode = attendance.AttendanceCode.Code,
                            GroupTypes = new List<CheckInGroupType> { groupType },
                            FirstTime = attendanceService.Queryable().AsNoTracking().Where(a => a.PersonAlias.PersonId == personId.Value).Count() <= 1,
                            Selected = true
                        };

                        // Only print the child's tag, never the parent tag.
                        var labelCache = GetGroupTypeLabels( groupType.GroupType ).Where( l => l.LabelType == KioskLabelType.Person ).OrderBy( l => l.Order ).FirstOrDefault();

                        if (labelCache != null)
                        {
                            person.SetOptions( labelCache );

                            var mergeObjects = new Dictionary<string, object>();
                            foreach ( var keyValue in commonMergeFields )
                            {
                                mergeObjects.Add( keyValue.Key, keyValue.Value );
                            }

                            mergeObjects.Add( "Location", location );
                            mergeObjects.Add( "Group", group );
                            mergeObjects.Add( "Person", person );
                            mergeObjects.Add( "GroupType", groupType );

                            
                            foreach ( var customValue in attendance.Attributes )
                            {
                                mergeObjects.Add( customValue.Key, attendance.AttributeValues[customValue.Key].Value );
                            }

                            var groupMembers = new GroupMemberService( rockContext ).Queryable().AsNoTracking()
                                                .Where( m =>
                                                    m.PersonId == person.Person.Id &&
                                                    m.GroupId == group.Group.Id )
                                                .ToList();
                            mergeObjects.Add( "GroupMembers", groupMembers );

                            var label = new CheckInLabel( labelCache, mergeObjects, person.Person.Id );
                            label.FileGuid = labelCache.Guid;
                            label.PrintTo = PrintTo.Kiosk;
                            label.PrinterDeviceId = ddlKiosk.SelectedValue.AsIntegerOrNull();
                            if ( label.PrinterDeviceId.HasValue )
                            {
                                var printerDevice = new DeviceService( rockContext ).Get( label.PrinterDeviceId.Value );
                                if ( printerDevice != null )
                                {
                                    label.PrinterAddress = printerDevice.IPAddress;
                                }
                            }

                            groupType.Labels.Add( label );

                            // Print (Success.acsx.cs)

                            string printContent = labelCache.FileContent;
                            foreach ( var mergeField in label.MergeFields )
                            {
                                if ( !string.IsNullOrWhiteSpace( mergeField.Value ) )
                                {
                                    printContent = Regex.Replace( printContent, string.Format( @"(?<=\^FD){0}(?=\^FS)", mergeField.Key ), ZebraFormatString( mergeField.Value ) );
                                }
                                else
                                {
                                    // Remove the box preceding merge field
                                    printContent = Regex.Replace( printContent, string.Format( @"\^FO.*\^FS\s*(?=\^FT.*\^FD{0}\^FS)", mergeField.Key ), string.Empty );
                                    // Remove the merge field
                                    printContent = Regex.Replace( printContent, string.Format( @"\^FD{0}\^FS", mergeField.Key ), "^FD^FS" );
                                }
                            }

                            Socket socket = null;
                            string currentIporHostname = string.Empty;
                            currentIporHostname = label.PrinterAddress;

                            bool successfulPrint = false;
                            var maxWaitTimeAttributeId = new AttributeService( rockContext ).Queryable().Where( a => a.Guid.ToString() == AttributeGuids.MAX_MILLISECONDS_WAIT_TIME_TO_CONNECT_TO_CHECKIN_PRINTER ).Select( a => a.Id ).FirstOrDefault();
                            var maxMillisecondsWaitTimeToConnectToCheckinPrinter = new AttributeValueService( rockContext ).Queryable().Where( av => av.AttributeId == maxWaitTimeAttributeId ).Select( av => av.Value ).FirstOrDefault().AsInteger();
                            var numberOfPrintRetriesAttributeId = new AttributeService( rockContext ).Queryable().Where( a => a.Guid.ToString() == AttributeGuids.NUMBER_OF_CHECKIN_LABEL_PRINT_RETRIES ).Select( a => a.Id ).FirstOrDefault();
                            var numberofCheckinLabelPrintRetries = new AttributeValueService( rockContext ).Queryable().Where( av => av.AttributeId == numberOfPrintRetriesAttributeId ).Select( av => av.Value ).FirstOrDefault().AsInteger();

                            int numberofRetries = numberofCheckinLabelPrintRetries;
                            while ( !successfulPrint )
                            {
                                try
                                {
                                    connectToPrinter( ref socket, ref currentIporHostname, maxMillisecondsWaitTimeToConnectToCheckinPrinter );
                                    if ( socket.Connected )
                                    {
                                        printLabel( socket, printContent );
                                        successfulPrint = true;
                                    }
                                    else
                                    {
                                        throw new Exception( "Could not connect following printer: " + currentIporHostname );
                                    }
                                }
                                catch ( Exception ex )
                                {
                                    ExceptionLogService.LogException( ex, Context, this.RockPage.PageId, this.RockPage.Site.Id );
                                    if ( numberofRetries == 0 )
                                    {
                                        //phResults.Controls.Add( new LiteralControl( "<br/>NOTE: Could not connect to printer!" + ( GetAttributeValue( "TranslateBlockIntoSpanish" ).AsBoolean() ? " (Impresora no disponible!)" : string.Empty ) ) );
                                        break;
                                    }
                                    else
                                    {
                                        numberofRetries--;
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch ( Exception ex )
            {
                LogException( ex );
            }

        }

        /// <summary>
        /// Print label
        /// </summary>
        /// <param name="socket">The socket</param>
        /// <param name="printContent">The print content</param>
        private static void printLabel( Socket socket, string printContent )
        {
            var ns = new NetworkStream( socket );
            byte[] toSend = System.Text.Encoding.ASCII.GetBytes( printContent );
            ns.Write( toSend, 0, toSend.Length );
        }

        /// <summary>
        /// Connects to printer
        /// </summary>
        /// <param name="socket">The socket</param>
        /// <param name="currentIporHostname">The printer host name or IP address</param>
        /// <param name="label">The label to print</param>
        private void connectToPrinter( ref Socket socket, ref string currentIporHostname, int maxMillisecondsWaitTimeToConnectToCheckinPrinter )
        {
            if ( socketConnected( ref socket ) )
            {
                closeSocket( ref socket );
            }


            IPAddress ipAddress;
            IPEndPoint printerIp;
            if ( IPAddress.TryParse( currentIporHostname, out ipAddress ) )
            {
                printerIp = new IPEndPoint( ipAddress, 9100 );
            }
            else
            {
                IPAddress[] ips;
                try
                {
                    ips = Dns.GetHostAddresses( currentIporHostname );
                    printerIp = new IPEndPoint( IPAddress.Parse( ips[0].ToString() ), 9100 );
                }
                catch ( Exception ex )
                {
                    throw new Exception( "Could not resolve the IP address of the following printer: " + currentIporHostname, ex );
                }
            }

            socket = new Socket( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
            IAsyncResult result = socket.BeginConnect( printerIp, null, null );
            bool success = result.AsyncWaitHandle.WaitOne( maxMillisecondsWaitTimeToConnectToCheckinPrinter, true );
        }

        /// <summary>
        /// Checks if the socket is connected
        /// </summary>
        /// <param name="socket"></param>
        /// <returns>Returns True if socket is connected otherwise false.</returns>
        private bool socketConnected( ref Socket socket )
        {
            if ( socket != null && socket.Connected )
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Closes an active socket.
        /// </summary>
        /// <param name="socket"></param>
        private void closeSocket( ref Socket socket )
        {
            socket.Shutdown( SocketShutdown.Both );
            socket.Close();
        }

        // Copied from CreateLabels
        private List<KioskLabel> GetGroupTypeLabels( GroupTypeCache groupType )
        {
            var labels = new List<KioskLabel>();

            //groupType.LoadAttributes();
            foreach ( var attribute in groupType.Attributes.OrderBy( a => a.Value.Order ) )
            {
                if ( attribute.Value.FieldType.Guid == Rock.SystemGuid.FieldType.BINARY_FILE.AsGuid() &&
                    attribute.Value.QualifierValues.ContainsKey( "binaryFileType" ) &&
                    attribute.Value.QualifierValues["binaryFileType"].Value.Equals( Rock.SystemGuid.BinaryFiletype.CHECKIN_LABEL, StringComparison.OrdinalIgnoreCase ) )
                {
                    Guid? binaryFileGuid = groupType.GetAttributeValue( attribute.Key ).AsGuidOrNull();
                    if ( binaryFileGuid != null )
                    {
                        var labelCache = KioskLabel.Read( binaryFileGuid.Value );
                        labelCache.Order = attribute.Value.Order;
                        if ( labelCache != null )
                        {
                            labels.Add( labelCache );
                        }
                    }
                }
            }

            return labels;
        }

        // Copied from Success.ascx.cs
        private string ZebraFormatString( string input, bool isJson = false )
        {
            if ( isJson )
            {
                return input
                    .Replace( "é", @"\\82" )
                    .Replace( "ü", @"\\81" )
                    .Replace( "á", @"\\a0" )
                    .Replace( "í", @"\\a1" )
                    .Replace( "ó", @"\\a2" )
                    .Replace( "ú", @"\\a3" )
                    .Replace( "¿", @"\\a8" )
                    .Replace( "¡", @"\\ad" )
                    .Replace( "ñ", @"\\a4" ).Replace( "Ñ", @"\\a5" );
            }
            else
            {
                return input
                    .Replace( "é", @"\82" )
                    .Replace( "ü", @"\81" )
                    .Replace( "á", @"\a0" )
                    .Replace( "í", @"\a1" )
                    .Replace( "ó", @"\a2" )
                    .Replace( "ú", @"\a3" )
                    .Replace( "¿", @"\a8" )
                    .Replace( "¡", @"\ad" )
                    .Replace( "ñ", @"\a4" ).Replace( "Ñ", @"\a5" );
            }
        }

        protected void ddlKiosk_SelectedIndexChanged( object sender, EventArgs e )
        {
            Session["LocationDetail_SelectedPrinter"] = ddlKiosk.SelectedValue;
            btnPrint.Enabled = ( ddlKiosk.SelectedValue.AsIntegerOrNull() > 0 );
        }

       
        protected void pillInRoom_Click( object sender, EventArgs e )
        {
            hfActiveTab.Value = IN_ROOM;
        }

        protected void pillEnRoute_Click( object sender, EventArgs e )
        {
            hfActiveTab.Value = EN_ROUTE;
        }

        protected void pillHealthNotes_Click( object sender, EventArgs e )
        {
            hfActiveTab.Value = HEALTH_NOTES;
        }

        protected void pillCheckedOut_Click( object sender, EventArgs e )
        {
            hfActiveTab.Value = CHECKED_OUT;
        }
    }
}