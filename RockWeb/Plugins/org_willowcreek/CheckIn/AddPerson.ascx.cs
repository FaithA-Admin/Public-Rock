using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Attribute;
using Rock.CheckIn;
using Rock.Model;
using Rock.Data;
using Rock.Web.Cache;
using System.Data.Entity;
using System.ComponentModel;
using System.Text;

namespace RockWeb.Plugins.org_willowcreek.CheckIn
{
    [DisplayName( "Add Person" )]
    [Category( "org_willowcreek > Check-in" )]
    [Description( "Displays a list of families to select for checkin." )]
    public partial class AddPerson : CheckInBlock
    {
        protected void Page_Load( object sender, EventArgs e )
        {
            if ( !IsPostBack )
            {
                BindGrades();
            }
        }

        private void BindGrades()
        {
            var grades = DefinedTypeCache.Read( Rock.SystemGuid.DefinedType.SCHOOL_GRADES.AsGuid() ).DefinedValues.Where( x => x.Value.AsInteger() <= 12 ).OrderBy( x => x.Order ).Select( x => new KeyValuePair<int?, string>( x.Value.AsInteger(), x.Description ) ).ToList();
            grades.Insert( 0, new KeyValuePair<int?, string>( 0, "Preschool" ) );
            grades.Insert( 0, new KeyValuePair<int?, string>( 0, "Not in school" ) );
            grades.Insert( 0, new KeyValuePair<int?, string>( null, "" ) );
            ddlGrade.DataSource = grades;
            ddlGrade.DataBind();
        }

        private string ValidationMessage()
        {
            var sb = new StringBuilder();

            if ( string.IsNullOrWhiteSpace( tbFirstName.Text ) )
            {
                sb.Append( "<li>First Name" );
            }

            if ( string.IsNullOrWhiteSpace( tbLastName.Text ) )
            {
                sb.Append( "<li>Last Name" );
            }

            if ( !chkMale.Checked && !chkFemale.Checked )
            {
                sb.Append( "<li>Gender" );
            }

            if ( dpBirthdate.SelectedDate == null )
            {
                sb.Append( "<li>Birth Date" );
            }
            else if ( dpBirthdate.SelectedDate.Value.Year == 1 )
            {
                sb.Append( "<li>Birth Year" );
            }

            if ( ddlGrade.SelectedIndex <= 0 )
            {
                sb.Append( "<li>Grade" );
            }

            if ( !string.IsNullOrWhiteSpace( sb.ToString() ) )
            {
                return "<P>Please provide the following:<ul>" + sb.ToString() + "</ul>";
            }

            return null;
        }

        protected void lbNext_Click( object sender, EventArgs e )
        {
            var validationMessage = ValidationMessage();
            if ( !string.IsNullOrEmpty( validationMessage ) )
            {
                maWarning.Show( validationMessage, Rock.Web.UI.Controls.ModalAlertType.Warning );
            }
            else
            {

                try
                {
                    if ( KioskCurrentlyActive )
                    {
                        using ( var rockContext = new RockContext() )
                        {
                            var person = new Person();
                            person.RecordStatusValueId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_ACTIVE.AsGuid() ).Id;
                            person.RecordTypeValueId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON ).Id;
                            person.FirstName = tbFirstName.Text;
                            person.LastName = tbLastName.Text;
                            person.Gender = chkMale.Checked ? Gender.Male : Gender.Female;
                            person.SetBirthDate( dpBirthdate.SelectedDate );
                            if ( !string.IsNullOrWhiteSpace( ddlGrade.SelectedValue ) && ddlGrade.SelectedValue != "0" )
                            {
                                person.GradeOffset = ddlGrade.SelectedValue.AsInteger();
                            }

                            if ( hfSameFamily.Value.AsBoolean() )
                            {
                                person.ConnectionStatusValueId = DefinedValueCache.Read( org.willowcreek.SystemGuid.DefinedValueGuids.PROMISELAND_FAMILY ).Id;

                                PersonService.AddPersonToFamily( person, true, CurrentCheckInState.CheckIn.CurrentFamily.Group.Id, ChildGroupRoleId, rockContext );
                            }
                            else
                            {
                                person.ConnectionStatusValueId = DefinedValueCache.Read( org.willowcreek.SystemGuid.DefinedValueGuids.PROMISELAND_GUEST ).Id;

                                var familyGroup = PersonService.SaveNewPerson( person, rockContext );

                                var qGroupService = new GroupService( rockContext ).Queryable().AsNoTracking();
                                var qGroupMemberService = new GroupMemberService( rockContext ).Queryable().AsNoTracking();

                                var adults = Adults();

                                // Set the guest's family campus to match the person checking in, until we can determine the current campus of the check in kiosk.
                                familyGroup.CampusId = new PersonService( rockContext ).Get( adults.First().PersonId ).GetCampusIds().First();

                                // Set Can Check-In relationships for all adults in the current family
                                var knownRelationshipGroupTypeId = GroupTypeCache.Read( Rock.SystemGuid.GroupType.GROUPTYPE_KNOWN_RELATIONSHIPS ).Id;



                                var childKnownRelationshipGroupId = ( from g in qGroupService
                                                                      join m in qGroupMemberService on g.Id equals m.GroupId
                                                                      where g.GroupTypeId == knownRelationshipGroupTypeId && m.PersonId == person.Id
                                                                      select g.Id ).FirstOrDefault();

                                foreach ( var adult in adults )
                                {
                                    var adultKnownRelationshipGroupId = ( from g in qGroupService
                                                                          join m in qGroupMemberService on g.Id equals m.GroupId
                                                                          where g.GroupTypeId == knownRelationshipGroupTypeId && m.PersonId == adult.PersonId
                                                                          select g.Id ).FirstOrDefault();

                                    // If this person has no Known Relationship group, create one.
                                    // TODO: This should really be common code with the similar code in Relationships.ascx.cs BindData()
                                    if (adultKnownRelationshipGroupId == 0)
                                    {
                                        var role = new GroupTypeRoleService( rockContext ).Get( Rock.SystemGuid.GroupRole.GROUPROLE_KNOWN_RELATIONSHIPS_OWNER.AsGuid() );
                                        if ( role != null && role.GroupTypeId.HasValue )
                                        {
                                            var groupService = new GroupService( rockContext );
                                            var group = new Group();
                                            group.Name = role.GroupType.Name;
                                            group.GroupTypeId = role.GroupTypeId.Value;
                                            groupService.Add( group );
                                            rockContext.SaveChanges();

                                            var groupMember = new GroupMember();
                                            groupMember.PersonId = adult.PersonId;
                                            groupMember.GroupRoleId = role.Id;
                                            groupMember.GroupId = group.Id;
                                            group.Members.Add( groupMember );
                                            rockContext.SaveChanges();

                                            adultKnownRelationshipGroupId = group.Id;
                                        }
                                    }

                                    PersonService.AddPersonToGroup( new PersonService( rockContext ).Get( adult.PersonId ), false, childKnownRelationshipGroupId, 8, rockContext );
                                    PersonService.AddPersonToGroup( person, false, adultKnownRelationshipGroupId, 9, rockContext );
                                }

                            }

                            rockContext.SaveChanges();

                            // Save the attributes after saving the person
                            if ( !string.IsNullOrWhiteSpace( tbAllergy.Text ) )
                            {
                                person.LoadAttributes();
                                person.SetAttributeValue( "Allergy", tbAllergy.Text );
                                person.SaveAttributeValue( "Allergy", rockContext );
                            }

                            ProcessSelection( maWarning );
                        }
                    }
                }
                catch ( Exception ex )
                {
                    while ( ex.InnerException != null )
                    {
                        ex = ex.InnerException;
                    }
                    ExceptionLogService.LogException( ex, System.Web.HttpContext.Current );
                }
            }
        }

        protected void lbBack_Click( object sender, EventArgs e )
        {
            GoBack();
        }

        protected void lbCancel_Click( object sender, EventArgs e )
        {
            CancelCheckin();
        }

        protected void lbYes_Click( object sender, EventArgs e )
        {
            hfSameFamily.Value = true.ToString();

            // If all the adults on the family share a last name, populate the last name field
            var adultLastNames = Adults().Select(x => x.Person.LastName).Distinct();
            if (adultLastNames.Count() == 1)
            {
                tbLastName.Text = adultLastNames.First();
            }

            pnlParent.Visible = false;
            pnlChild.Visible = true;
            SaveState();
        }

        private List<GroupMember> Adults()
        {
            var qGroupMemberService = new GroupMemberService( new RockContext() ).Queryable().AsNoTracking();
            return qGroupMemberService.Where( x => x.GroupId == CurrentCheckInState.CheckIn.CurrentFamily.Group.Id && x.GroupRoleId == AdultGroupRoleId ).ToList();
        }

        protected void lbNo_Click( object sender, EventArgs e )
        {
            hfSameFamily.Value = false.ToString();
            pnlParent.Visible = false;
            pnlChild.Visible = true;
            SaveState();
        }

        protected void lbParentBack_Click( object sender, EventArgs e )
        {
            GoBack();
        }

        protected void lbParentCancel_Click( object sender, EventArgs e )
        {
            CancelCheckin();
        }

        private GroupTypeCache FamilyGroupType
        {
            get
            {
                return GroupTypeCache.Read( Rock.SystemGuid.GroupType.GROUPTYPE_FAMILY.AsGuid() );
            }
        }

        private int ChildGroupRoleId
        {
            get
            {
                return FamilyGroupType.Roles.First( a => a.Guid == Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_CHILD.AsGuid() ).Id;
            }
        }

        private int AdultGroupRoleId
        {
            get
            {
                return FamilyGroupType.Roles.First( a => a.Guid == Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT.AsGuid() ).Id;
            }
        }

        protected void chkMale_CheckedChanged( object sender, EventArgs e )
        {
            cblGender.SelectedValue = "Male";
            chkMale.Checked = true; // Do not allow unchecking
            chkFemale.Checked = false;
        }

        protected void chkFemale_CheckedChanged( object sender, EventArgs e )
        {
            cblGender.SelectedValue = "Female";
            chkFemale.Checked = true; // Do not allow unchecking
            chkMale.Checked = false;
        }
    }
}