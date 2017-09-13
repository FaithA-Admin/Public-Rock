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
    [Description( "Saves the new family parent." )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "New Family - Parent/Guardian" )]
    [WorkflowAttribute( "First Name", "", true, "", "", 0 )]
    [WorkflowAttribute( "Last Name", "", true, "", "", 1 )]
    [WorkflowAttribute( "Gender", "", true, "", "", 2 )]
    [WorkflowAttribute( "Mobile Phone", "", true, "", "", 3 )]
    [WorkflowAttribute( "Email", "", true, "", "", 4 )]    
    [WorkflowAttribute( "CampusShortCode", "", true, "", "", 5 )]
    [WorkflowAttribute( "Result Person Id", "An optional attribute to set the Person Id of the new person that was created.", false, "", "", 6 )]
    [WorkflowAttribute( "Result Family Id", "An optional attribute to set the Family Id of the New Family that was created.", false, "", "", 7 )]
    public class NewFamily_ParentGuardian : ActionComponent
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

            var firstName = action.GetWorklowAttributeValue( GetAttributeValue( action, "FirstName" ).AsGuid());
            var lastName = action.GetWorklowAttributeValue( GetAttributeValue( action, "LastName" ).AsGuid() );
            var gender = action.GetWorklowAttributeValue( GetAttributeValue( action, "Gender" ).AsGuid() );
            var email = action.GetWorklowAttributeValue( GetAttributeValue( action, "Email" ).AsGuid() );
            var mobilePhone = action.GetWorklowAttributeValue( GetAttributeValue( action, "MobilePhone" ).AsGuid() );
            var campusShortCode = action.GetWorklowAttributeValue( GetAttributeValue( action, "CampusShortCode" ).AsGuid() );
           
            try
            {

                Person person = new Person();
                person.RecordStatusValueId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_ACTIVE.AsGuid() ).Id;
                person.RecordTypeValueId = DefinedValueCache.Read( Rock.SystemGuid.DefinedValue.PERSON_RECORD_TYPE_PERSON ).Id;
                person.FirstName = firstName;
                person.LastName = lastName;
                switch ( gender )
                {
                    case "0":
                        person.Gender = Gender.Unknown;
                        break;
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

                if ( !email.IsNullOrWhiteSpace() )
                {
                    person.Email = email;
                }               
                
                PhoneNumber mobile = new PhoneNumber();
                var numberType = DefinedValueCache.Read( org.willowcreek.SystemGuid.DefinedValueGuids.MOBILE_PHONE );
                string cleanNumber = PhoneNumber.CleanNumber( mobilePhone );
                if ( numberType != null && !string.IsNullOrWhiteSpace( cleanNumber ) )
                {
                    mobile.NumberTypeValueId = numberType.Id;
                    mobile.Number = cleanNumber;
                    mobile.NumberFormatted = PhoneNumber.FormattedNumber("",cleanNumber);
                    mobile.IsMessagingEnabled = true;
                    person.PhoneNumbers.Add( mobile );                   
                }

                person.ConnectionStatusValueId = DefinedValueCache.Read( org.willowcreek.SystemGuid.DefinedValueGuids.PROMISELAND_FAMILY ).Id;
                
                //Create Family
                var familyGroup = PersonService.SaveNewPerson( person, rockContext );
                int? familyId = null;
                if ( familyGroup != null )
                {
                    familyId = familyGroup.Id;
                }
                
                //Set role of parent as adult
                var familyGroupType = GroupTypeCache.Read( Rock.SystemGuid.GroupType.GROUPTYPE_FAMILY.AsGuid() );                
                var groupMemberService = new GroupMemberService( rockContext );
                var parent = groupMemberService.Queryable("Person",false).Where( m => m.GroupId == familyGroup.Id).FirstOrDefault();
                parent.GroupRoleId = familyGroupType.Roles.First( a => a.Guid == Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT.AsGuid() ).Id;

                //Set Campus
                var campus = new CampusService( rockContext ).Queryable().Where( c => c.ShortCode == campusShortCode ).FirstOrDefault();
                if(campus != null)
                {
                    familyGroup.Campus = campus;
                }

                rockContext.SaveChanges();

                //Set result attributes

                var personsIds = SetWorkflowAttributeValue( action, "ResultPersonId", person.Id.ToString() + "-adult" );
                if ( personsIds != null )
                {
                    action.AddLogEntry( string.Format( "Set '{0}' attribute to '{1}'.", personsIds.Name, person.Id.ToString() + "-adult" ) );
                }

                var family = SetWorkflowAttributeValue( action, "ResultFamilyId", familyId.Value );
                if ( family != null )
                {
                    action.AddLogEntry( string.Format( "Set '{0}' attribute to '{1}'.", family.Name, familyId.Value ) );
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
