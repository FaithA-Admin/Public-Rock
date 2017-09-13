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

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace Rock.Workflow.Action
{
    /// <summary>
    /// Adds person to a group using a workflow attribute.
    /// </summary>
    [ActionCategory("Groups")]
    [Description("Adds person to a group(s) using a workflow Single or Multi Select attribute.")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Group Member Add From Single or Multi Select Attribute")]

    [WorkflowAttribute("Person", "Workflow attribute that contains the person to add to the group.", true, "", "", 0, null,
        new string[] { "Rock.Field.Types.PersonFieldType" })]

    [WorkflowAttribute("Group", "Workflow Single or Multi Select Attribute that contains the group to add the person to.", true, "", "", 1, null,
        new string[] { "Rock.Field.Types.SelectSingleFieldType", "Rock.Field.Types.SelectMultiFieldType" })]
    [BooleanField("Continue On Error", "Should processing continue even if an error occurs?", false,"",2)]
    public class AddPersonToGroupWFFromSelectUserControlAttribute : ActionComponent
    {
        /// <summary>
        /// Executes the specified workflow.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="action">The action.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="errorMessages">The error messages.</param>
        /// <returns></returns>
        public override bool Execute(RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages)
        {
            bool continueOnError = GetAttributeValue(action, "ContinueOnError").AsBoolean();
            errorMessages = new List<string>();

            List<KeyValuePair<Group, int?>> groupsAndRoles = new List<KeyValuePair<Group, int?>>(); 
     
            var guidGroupAttribute = GetAttributeValue(action, "Group").AsGuidOrNull();

            if (guidGroupAttribute.HasValue)
            {
                var attributeGroup = AttributeCache.Read(guidGroupAttribute.Value, rockContext);
               
                if (attributeGroup != null)
                {
                    var groupIds = action.GetWorklowAttributeValue(attributeGroup.Guid).Split(',');
                   
                    if (groupIds != null || groupIds.Length != 0)
                    {
                        foreach( var groupid in groupIds)
                        {
                            if(groupid.AsIntegerOrNull() != null)
                            {
                                var group = new GroupService(rockContext).Get(groupid.AsInteger());

                                if (group == null)
                                {
                                    errorMessages.Add("No group was provided");
                                }                               

                                if(group != null)
                                {
                                    var groupRoleId = group.GroupType.DefaultGroupRoleId;

                                    if (!groupRoleId.HasValue)
                                    {
                                        errorMessages.Add("Provided group (" + group.Name + ") doesn't have a default group role");
                                    }
                                    else
                                    {
                                        groupsAndRoles.Add(new KeyValuePair<Group, int?>(group, groupRoleId));
                                    }                                    
                                }                               
                            }                          
                        }                      
                    }
                }
            }           

            // determine the person that will be added to the group
            Person person = null;

            // get the Attribute.Guid for this workflow's Person Attribute so that we can lookup the value
            var guidPersonAttribute = GetAttributeValue(action, "Person").AsGuidOrNull();

            if (guidPersonAttribute.HasValue)
            {
                var attributePerson = AttributeCache.Read(guidPersonAttribute.Value, rockContext);
                if (attributePerson != null)
                {
                    string attributePersonValue = action.GetWorklowAttributeValue(guidPersonAttribute.Value);
                    if (!string.IsNullOrWhiteSpace(attributePersonValue))
                    {
                        if (attributePerson.FieldType.Class == typeof(Rock.Field.Types.PersonFieldType).FullName)
                        {
                            Guid personAliasGuid = attributePersonValue.AsGuid();
                            if (!personAliasGuid.IsEmpty())
                            {
                                person = new PersonAliasService(rockContext).Queryable()
                                    .Where(a => a.Guid.Equals(personAliasGuid))
                                    .Select(a => a.Person)
                                    .FirstOrDefault();
                            }
                        }
                        else
                        {
                            errorMessages.Add("The attribute used to provide the person was not of type 'Person'.");
                        }
                    }
                }
            }

            if (person == null)
            {
                errorMessages.Add(string.Format("Person could not be found for selected value ('{0}')!", guidPersonAttribute.ToString()));
            }

            // Add Person to Group
            if (!errorMessages.Any() || continueOnError)
            {
                foreach(var groupAndRole in groupsAndRoles)
                {
                    Group group = (Group)groupAndRole.Key;
                    int? groupRoleId = (int?)groupAndRole.Value;
                    var groupMemberService = new GroupMemberService(rockContext);
                    var groupMember = new GroupMember();
                    groupMember.PersonId = person.Id;
                    groupMember.GroupId = group.Id;
                    groupMember.GroupRoleId = groupRoleId.Value;
                    groupMember.GroupMemberStatus = GroupMemberStatus.Active;
                    if (groupMember.IsValid)
                    {
                        groupMemberService.Add(groupMember);
                        rockContext.SaveChanges();
                    }
                    else
                    {
                        // if the group member couldn't be added (for example, one of the group membership rules didn't pass), add the validation messages to the errormessages
                        errorMessages.AddRange(groupMember.ValidationResults.Select(a => a.ErrorMessage));
                    }
                }               
            }

            errorMessages.ForEach(m => action.AddLogEntry(m, true));

            if(continueOnError)
            {
                //Do not logg errors if continue on error is true.
                errorMessages.Clear();
            }

            return true;
        }

    }
}