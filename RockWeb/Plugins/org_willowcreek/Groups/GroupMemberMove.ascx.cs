using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;
using org.willowcreek;
using System.Linq.Dynamic;

namespace RockWeb.Plugins.org_willowcreek.Groups
{
    [DisplayName( "Group Member Move" )]
    [Category( "org_willowcreek > Groups" )]
    [Description( "Move members from one group to another" )]
    public partial class GroupMemberMove : RockBlock
    {
        public enum SelectionType
        {
            Simple,
            Advanced
        }

        public SelectionType MemberSelectionType { get { return tglPeople.Checked ? SelectionType.Simple : SelectionType.Advanced; } }

        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( !Page.IsPostBack )
            {
                ShowDetail();
            }

        }

        private void ShowDetail()
        {
            var moveFrom = GetBlockUserPreference( "MoveFrom" );
            if ( !string.IsNullOrWhiteSpace( moveFrom ) )
            {
                gpFrom.SetValue( moveFrom.AsInteger() );
            }

            var moveTo = GetBlockUserPreference( "MoveTo" );
            if ( !string.IsNullOrWhiteSpace( moveTo ) )
            {
                gpTo.SetValue( moveTo.AsInteger() );
            }

            if (MemberSelectionType == SelectionType.Simple)
            {
                pnlSimple.Visible = true;
                pnlAdvanced.Visible = false;
                ListGroupRoles();
            }
            else
            {
                pnlSimple.Visible = false;
                pnlAdvanced.Visible = true;
                PopulateAttendanceGroupList();
                BindGrid();
            }
            NewRoleDisplay();
        }

        protected void gpFrom_SelectItem( object sender, EventArgs e )
        {
            SetBlockUserPreference( "MoveFrom", gpFrom.SelectedValue );
            ListGroupRoles();
            PopulateAttendanceGroupList();
            BindGrid();
            NewRoleDisplay();
        }

        private void PopulateAttendanceGroupList()
        {
            ddlAttendanceGroup.Items.Clear();
            var groupId = gpFrom.SelectedValue.AsIntegerOrNull();
            if ( groupId.HasValue )
            {
                using ( var rockContext = new RockContext() )
                {
                    var groupService = new GroupService( rockContext );
                    var group = groupService.Get( groupId.Value );
                    
                    // Recursively add the selected group and all its parent groups
                    PopulateWithAllParentGroups( groupService, group );

                    // Select the From Group to start
                    ddlAttendanceGroup.SetValue( group.Id.ToString() );
                }
            }

            // If the previously selected group is in this list, select it instead
            var attendanceGroup = GetBlockUserPreference( "AttendanceGroup" );
            if ( !string.IsNullOrWhiteSpace( attendanceGroup ) )
            {
                var item = ddlAttendanceGroup.Items.FindByValue( attendanceGroup );
                if ( item != null )
                {
                    ddlAttendanceGroup.SetValue( item.Value );
                }
            }
        }

        private void PopulateWithAllParentGroups(GroupService groupService, Group group)
        {
            ddlAttendanceGroup.Items.Insert(0, new ListItem( group.Name, group.Id.ToString() ) );
            if ( group.ParentGroup != null )
            {
                PopulateWithAllParentGroups( groupService, group.ParentGroup );
            }
        }

        private void BindGrid()
        {
            var groupId = gpFrom.SelectedValue.AsIntegerOrNull();
            if ( groupId.HasValue )
            {
                using ( var rockContext = new RockContext() )
                {
                    var groupMemberService = new GroupMemberService( rockContext );
                    var qry = groupMemberService.Queryable( "Person,GroupRole", true ).AsNoTracking()
                        .Where( m => m.GroupId == groupId );

                    var date = DateTime.Today.AddDays( -7 * 8 );
                    var dataSource = qry.ToList().Select(m => new {
                        m.Id,
                        Name = m.Person.FullNameReversed,
                        GroupRole = m.GroupRole.Name,
                        m.Person.Gender,
                        m.Person.BirthDate,
                        Attendance = (from a in new AttendanceService( rockContext ).Queryable().AsNoTracking()
                                      join pa in new PersonAliasService( rockContext).Queryable().AsNoTracking() on a.PersonAliasId equals pa.Id
                                      join g in new GroupService (rockContext).Queryable().AsNoTracking() on a.GroupId equals g.Id
                                      where pa.PersonId == m.Person.Id
                                      && a.StartDateTime >= date
                                      select g).ToList()
                                      .Where(x => HasAncestor(x, ddlAttendanceGroup.SelectedValue.AsInteger() ) )
                                      .Count()
                    } ).ToList();

                    if (gMembers.SortProperty == null)
                    {
                        gMembers.SortProperty = new SortProperty()
                        {
                            Property = GetBlockUserPreference( "MemberSortProperty" ),
                            Direction = GetBlockUserPreference( "MemberSortDirection" ) == "Ascending" ? SortDirection.Ascending : SortDirection.Descending
                        };
                    }
                    if ( gMembers.SortProperty == null )
                    {
                        gMembers.SortProperty = new SortProperty() { Property = "Name", Direction = SortDirection.Ascending };
                    }
                    switch ( gMembers.SortProperty.Property )
                    {
                        case "GroupRole":
                            if ( gMembers.SortProperty.Direction == SortDirection.Ascending )
                            {
                                dataSource = dataSource.OrderBy( x => x.GroupRole ).ToList();
                            }
                            else
                            {
                                dataSource = dataSource.OrderByDescending( x => x.GroupRole ).ToList();
                            }
                            break;
                        case "Gender":
                            if ( gMembers.SortProperty.Direction == SortDirection.Ascending )
                            {
                                dataSource = dataSource.OrderBy( x => x.Gender ).ToList();
                            }
                            else
                            {
                                dataSource = dataSource.OrderByDescending( x => x.Gender ).ToList();
                            }
                            break;
                        case "BirthDate":
                            if ( gMembers.SortProperty.Direction == SortDirection.Ascending )
                            {
                                dataSource = dataSource.OrderBy( x => x.BirthDate ).ToList();
                            }
                            else
                            {
                                dataSource = dataSource.OrderByDescending( x => x.BirthDate ).ToList();
                            }
                            break;
                        case "Attendance":
                            if ( gMembers.SortProperty.Direction == SortDirection.Ascending )
                            {
                                dataSource = dataSource.OrderBy( x => x.Attendance ).ToList();
                            }
                            else
                            {
                                dataSource = dataSource.OrderByDescending( x => x.Attendance ).ToList();
                            }
                            break;
                        default:
                            if ( gMembers.SortProperty.Direction == SortDirection.Ascending )
                            {
                                dataSource = dataSource.OrderBy( x => x.Name ).ToList();
                            }
                            else
                            {
                                dataSource = dataSource.OrderByDescending( x => x.Name ).ToList();
                            }
                            break;
                    }

                    gMembers.DataSource = dataSource;
                    gMembers.DataBind();

                    SetBlockUserPreference( "MemberSortProperty", gMembers.SortProperty.Property );
                    SetBlockUserPreference( "MemberSortDirection", gMembers.SortProperty.DirectionString );
                }
            }
        }

        protected void gMembers_GridRebind( object sender, GridRebindEventArgs e )
        {
            BindGrid();
        }

        private bool HasAncestor(Group group, int idToFind)
        {
            if (group.Id == idToFind)
            {
                return true;
            }
            else if (group.ParentGroupId == null)
            {
                return false;
            }
            else
            {
                return HasAncestor( group.ParentGroup, idToFind );
            }
        }
        
        private void ListGroupRoles()
        {
            cblRoles.Items.Clear();

            using ( var rockContext = new RockContext() )
            {
                var groupMemberService = new GroupMemberService( rockContext );
                var groupTypeRoleService = new GroupTypeRoleService( rockContext );

                var groupId = gpFrom.SelectedValue.AsIntegerOrNull();

                if ( groupId.HasValue )
                {
                    var qry = ( from gm in groupMemberService.Queryable().AsNoTracking()
                                join r in groupTypeRoleService.Queryable().AsNoTracking() on gm.GroupRoleId equals r.Id
                                where gm.GroupId == groupId
                                group r by new { r.Id, r.Name } into newGroup
                                select new { Id = newGroup.Key.Id, Count = newGroup.Count(), Name = newGroup.Key.Name } )
                                .OrderByDescending( x => x.Count ).ToList();

                    if ( qry.Any() )
                    {
                        var listItems = qry.Select( x => new ListItem( x.Count + " " + x.Name.PluralizeIf( x.Count != 1 ), x.Id.ToString() ) ).ToArray();
                        cblRoles.Items.AddRange( listItems );
                        cblRoles.Items[0].Selected = ( cblRoles.Items.Count == 1 );
                    }
                }
            }
        }

        protected void gpTo_SelectItem( object sender, EventArgs e )
        {
            SetBlockUserPreference( "MoveTo", gpTo.SelectedValue );

            NewRoleDisplay();
        }

        private void NewRoleDisplay()
        {
            var newRoleDisplay = false;
            rblRole.Items.Clear();
            var fromGroupId = gpFrom.SelectedValue.AsIntegerOrNull();
            var toGroupId = gpTo.SelectedValue.AsIntegerOrNull();

            if (fromGroupId.HasValue && toGroupId.HasValue)
            {
                using ( var rockContext = new RockContext() )
                {
                    var groupService = new GroupService( rockContext );

                    var match = ( from fg in groupService.Queryable()
                                  join tg in groupService.Queryable() on fg.GroupTypeId equals tg.GroupTypeId
                                  where fg.Id == fromGroupId.Value && tg.Id == toGroupId.Value
                                  select 1 ).Any();

                    if ( !match )
                    {
                        newRoleDisplay = true;

                        var groupTypeRoleService = new GroupTypeRoleService( rockContext );
                        var listItems = ( from g in groupService.Queryable().AsNoTracking()
                                          join r in groupTypeRoleService.Queryable().AsNoTracking() on g.GroupTypeId equals r.GroupTypeId
                                          where g.Id == toGroupId.Value
                                          select new { r.Name, r.Id } ).ToArray().Select(x => new ListItem(x.Name, x.Id.ToString())).ToArray();
                        rblRole.Items.AddRange( listItems );
                    }
                }
            }

            rblRole.Visible = newRoleDisplay;
        }

        protected void btnMove_Click( object sender, EventArgs e )
        {
            SetBlockUserPreference( "MoveFrom", gpFrom.SelectedValue );
            SetBlockUserPreference( "MoveTo", gpTo.SelectedValue );

            var fromGroupId = gpFrom.SelectedValue.AsIntegerOrNull();
            var toGroupId = gpTo.SelectedValue.AsIntegerOrNull();

            if ( fromGroupId.HasValue && toGroupId.HasValue && fromGroupId != toGroupId )
            {
                using ( var rockContext = new RockContext() )
                {
                    var moved = 0;
                    var existing = 0;
                    var groupMemberService = new GroupMemberService( rockContext );
                    List<GroupMember> members;

                    if ( MemberSelectionType == SelectionType.Simple )
                    {
                        members = groupMemberService.Queryable().Where( m => m.GroupId == fromGroupId.Value && cblRoles.SelectedValuesAsInt.Contains( m.GroupRoleId ) ).ToList();
                    }
                    else // Advanced
                    {
                        var selectedGroupMemberIds = gMembers.SelectedKeys.Select( m => (int)m );
                        members = groupMemberService.Queryable().Where( m => selectedGroupMemberIds.Contains( m.Id ) ).ToList();
                    }

                    foreach ( var member in members )
                    {
                        if ( !groupMemberService.Queryable().Where( m => m.PersonId == member.PersonId && m.GroupId == toGroupId.Value ).Any() )
                        {
                            member.GroupId = toGroupId.Value;
                            member.DateTimeAdded = DateTime.Now;
                            var groupRoleId = rblRole.SelectedValue.AsIntegerOrNull();
                            if ( rblRole.Visible && groupRoleId != null )
                            {
                                member.GroupRoleId = rblRole.SelectedValue.AsInteger();
                            }
                            moved++;
                        }
                        else
                        {
                            groupMemberService.Delete( member );
                            existing++;
                        }
                    }

                    rockContext.SaveChanges();

                    var groupService = new GroupService( rockContext );

                    nbSuccess.Text = moved.ToString() + " " + "person".PluralizeIf( moved != 1 ) + " moved from " + gpFrom.ItemName + " to " + gpTo.ItemName + ".";
                    if ( existing > 0 )
                    {
                        nbSuccess.Text += "<br>" + existing.ToString() + " " + "person".PluralizeIf( moved != 1 ) + " already in " + gpTo.ItemName + ", removed from " + gpFrom.ItemName + ".";
                    }
                    nbSuccess.Visible = true;

                    if ( MemberSelectionType == SelectionType.Simple )
                    {
                        ListGroupRoles();
                    }
                    else // Advanced
                    {
                        BindGrid();
                    }
                    
                }
            }
        }

        protected void ddlAttendanceGroup_SelectedIndexChanged( object sender, EventArgs e )
        {
            SetBlockUserPreference( "AttendanceGroup", ddlAttendanceGroup.SelectedValue );
            BindGrid();
        }

        protected void tglPeople_CheckedChanged( object sender, EventArgs e )
        {
            ShowDetail();
        }
    }
}