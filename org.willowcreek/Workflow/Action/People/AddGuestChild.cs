// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Rock.Attribute;
using Rock.Communication;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Web.UI;

namespace Rock.Workflow.Action
{
    /// <summary>
    /// Runs a SQL query
    /// </summary>
    [ActionCategory( "New Family" )]
    [Description( "Runs the specified SQL query to perform an action against the database." )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Add Guest Child" )]
    [WorkflowAttribute( "Adult", required: true, order: 0)]
    //[WorkflowAttribute( "Parent Id", "", true, "", "", 1 )]
    [WorkflowAttribute( "First Name", "", true, "", "", 2 )]
    [WorkflowAttribute( "Last Name", "", true, "","",3 )]
    [WorkflowAttribute( "Gender", "", false, "", "", 4 )]
    [WorkflowAttribute( "Birthdate", "", true, "", "", 5 )]
    [WorkflowAttribute( "Grade", "", false, "", "", 6 )]
    [WorkflowAttribute( "Health Note", "", false, "", "", 7 )]
    //[WorkflowAttribute( "Relationship", "", true, "", "", 8 )]   
    [WorkflowAttribute( "Result Person Id", "An optional attribute to set the Person Id of the new person that was created.", false, "", "", 9 )]
    public class AddGuestChild : ActionComponent
    {
        /// <summary>
        /// Executes the specified workflow.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="action">The action.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="errorMessages">The error messages.</param>
        /// <returns></returns>
        public override bool Execute( RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages )
        {
            errorMessages = new List<string>();

            //var parentId = action.GetWorklowAttributeValue( GetAttributeValue( action, "ParentId" ).AsGuid() ).Split('-')[0].AsInteger();
            var adultPersonAliasGuid = action.GetWorklowAttributeValue( GetAttributeValue( action, "Adult" ).AsGuid() ).AsGuid();
            var adultPersonAlias = new PersonAliasService( rockContext ).Get( adultPersonAliasGuid );
            var firstName = action.GetWorklowAttributeValue( GetAttributeValue( action, "FirstName" ).AsGuid());
            var lastName = action.GetWorklowAttributeValue( GetAttributeValue( action, "LastName" ).AsGuid() );
            var gender = action.GetWorklowAttributeValue( GetAttributeValue( action, "Gender" ).AsGuid() );
            var birthdate = action.GetWorklowAttributeValue( GetAttributeValue( action, "Birthdate" ).AsGuid() ).AsDateTime();
            var grade = action.GetWorklowAttributeValue( GetAttributeValue( action, "Grade" ).AsGuid() );
            var healthNote = action.GetWorklowAttributeValue( GetAttributeValue( action, "HealthNote" ).AsGuid() );
            var relationship = action.GetWorklowAttributeValue( GetAttributeValue( action, "Relationship" ).AsGuid() );            
            var resultPersonId = action.GetWorklowAttributeValue( GetAttributeValue( action, "ResultPersonId" ).AsGuid() );
            
            try
            {

                Person person = new Person();
                person.RecordStatusValueId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_ACTIVE.AsGuid() ).Id;
                person.RecordTypeValueId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON ).Id;
                person.FirstName = firstName;
                person.LastName = lastName;
                switch ( gender )
                {
                    case "1":
                        person.Gender = Gender.Male;
                        break;
                    case "2":
                        person.Gender = Gender.Female;
                        break;
                    default:
                        person.Gender = Gender.Unknown;
                        break;
                }
                person.SetBirthDate(birthdate);

                var familyGroupType = GroupTypeCache.Read( Rock.SystemGuid.GroupType.GROUPTYPE_FAMILY.AsGuid() );
                var childGroupRoleId = familyGroupType.Roles.First( a => a.Guid == Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_CHILD.AsGuid() ).Id;

                person.ConnectionStatusValueId = DefinedValueCache.Read( org.willowcreek.SystemGuid.DefinedValueGuids.PROMISELAND_GUEST ).Id;

                    //Create Family
                    var familyGroup = PersonService.SaveNewPerson( person, rockContext );
                    int? familyGroupId = null;
                    if ( familyGroup != null )
                    {
                        familyGroupId = familyGroup.Id;
                    }

                familyGroup.CampusId = adultPersonAlias.Person.GetCampusIds().First();

                    //Set role of child
                    
                    var groupMemberService = new GroupMemberService( rockContext );
                    var child = groupMemberService.Queryable( "Person", false ).Where( m => m.GroupId == familyGroup.Id ).FirstOrDefault();
                    child.GroupRoleId = childGroupRoleId;

                    //set grade offset
                    if ( grade != "na1" && grade != "na2" && !string.IsNullOrWhiteSpace(grade) )
                    {
                        child.Person.GradeOffset = grade.AsInteger();
                    }                   

                    //Set Can Check-In relationship
                    var knownRelationshipGroupTypeId = GroupTypeCache.Read( Rock.SystemGuid.GroupType.GROUPTYPE_KNOWN_RELATIONSHIPS ).Id;
                    var adultKnownRelationshipGroupId = from g in new GroupService( rockContext ).Queryable()
                                                        join m in new GroupMemberService( rockContext ).Queryable() on g.Id equals m.GroupId
                                                        where g.GroupTypeId == knownRelationshipGroupTypeId && m.PersonId == adultPersonAlias.PersonId
                                                        select g.Id;
                    var childKnownRelationshipGroupId = from g in new GroupService( rockContext ).Queryable()
                                                        join m in new GroupMemberService( rockContext ).Queryable() on g.Id equals m.GroupId
                                                        where g.GroupTypeId == knownRelationshipGroupTypeId && m.PersonId == person.Id
                                                        select g.Id;

                    var adultGroupMember = new GroupMember();
                    adultGroupMember.GroupRoleId = 8;
                    adultGroupMember.PersonId = adultPersonAlias.PersonId;
                    adultGroupMember.GroupId = childKnownRelationshipGroupId.FirstOrDefault();
                    adultGroupMember.GroupMemberStatus = GroupMemberStatus.Active;

                    var childGroupMember = new GroupMember();
                    childGroupMember.GroupRoleId = 9;
                    childGroupMember.PersonId = person.Id;
                    childGroupMember.GroupId = adultKnownRelationshipGroupId.FirstOrDefault();
                    childGroupMember.GroupMemberStatus = GroupMemberStatus.Active;

                    new GroupMemberService( rockContext ).Add( adultGroupMember );
                    new GroupMemberService( rockContext ).Add( childGroupMember );

                var healthNoteAttributeValue = new AttributeValue();
                healthNoteAttributeValue.Value = healthNote;
                healthNoteAttributeValue.EntityId = person.Id;
                var allergyAttributeGuid = org.willowcreek.SystemGuid.AttributeGuids.ALLERGY.AsGuid();
                healthNoteAttributeValue.AttributeId = new AttributeService( rockContext ).Queryable().Where( a => a.Guid == allergyAttributeGuid ).Select( s => s.Id ).FirstOrDefault();
                new AttributeValueService( rockContext ).Add(healthNoteAttributeValue);

                

                rockContext.SaveChanges();

               
                
                //Set result attributes

                string resultPersonsIds = resultPersonId + "," + person.Id.ToString() + "-child";
                var personsIds = SetWorkflowAttributeValue( action, "ResultPersonId", resultPersonsIds);
                if ( personsIds != null )
                {
                    action.AddLogEntry( string.Format( "Set '{0}' attribute to '{1}'.", personsIds.Name, resultPersonsIds ) );
                }

                return true;
            }
            catch ( Exception ex )
            {
                action.AddLogEntry( ex.Message, true );
                errorMessages.Add( ex.Message );
                return false;
            }

        }
    }
}
