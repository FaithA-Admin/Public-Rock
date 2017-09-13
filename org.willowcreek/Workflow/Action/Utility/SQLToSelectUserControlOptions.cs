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
    [ActionCategory("Utility")]
    [Description("Runs the specified SQL query to perform an action against the database and stores the scalar result as values on the specified Single Select or Multi Select user control.<br/>The scalar result format must be either 'value1,value2,value3,...' or 'value1^text1,value2^text2,value3^text3,...")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "SQL To Select User Control Options")]
    [CodeEditorField("SQLQuery", "The SQL query to run. <span class='tip tip-lava'></span>", Web.UI.Controls.CodeEditorMode.Sql, Web.UI.Controls.CodeEditorTheme.Rock, 400, true, "", "", 0)]
    [WorkflowAttribute("ResultUserControlOptionsAttribute", "A required Dropdown type attribute to set to the scaler result of SQL query.", true, "", "", 1, null, new string[]{ "Rock.Field.Types.SelectSingleFieldType", "Rock.Field.Types.SelectMultiFieldType" })]
    [BooleanField("Continue On Error", "Should processing continue even if SQL Error occurs?", false, "", 2)]
    public class SQLToSelectUserControlOptions : ActionComponent
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
            errorMessages = new List<string>();

            var query = GetAttributeValue(action, "SQLQuery");

            var mergeFields = GetMergeFields(action);
            query = query.ResolveMergeFields(mergeFields);

            try
            {
                object sqlResult = DbService.ExecuteScaler(query);
                action.AddLogEntry("SQL query has been run");

                if (sqlResult != null)
                {
                    Guid? attributeGuid = GetAttributeValue(action, "ResultUserControlOptionsAttribute").AsGuidOrNull();
                    if (attributeGuid.HasValue)
                    {
                        var attribute = AttributeCache.Read(attributeGuid.Value, rockContext);
                        if (attribute != null)
                        {
                            string resultValue = sqlResult.ToString();

                            if (attribute.EntityTypeId == new Rock.Model.Workflow().TypeId)
                            {                              
                               if(action.Activity.Workflow.Attributes.ContainsKey(attribute.Key))
                                {
                                    action.Activity.Workflow.Attributes[attribute.Key].QualifierValues["values"] = new Rock.Field.ConfigurationValue(resultValue);
                                    action.AddLogEntry(string.Format("Set '{0}' attribute to '{1}'.", attribute.Name, resultValue));
                                }                               
                            }
                            else if (attribute.EntityTypeId == new Rock.Model.WorkflowActivity().TypeId)
                            {                               
                               if(action.Activity.Attributes.ContainsKey(attribute.Key))
                                {
                                    action.Activity.Attributes[attribute.Key].QualifierValues["values"] = new Rock.Field.ConfigurationValue(resultValue);
                                    action.AddLogEntry(string.Format("Set '{0}' attribute to '{1}'.", attribute.Name, resultValue));
                                }                               
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                action.AddLogEntry(ex.Message, true);

                if (!GetAttributeValue(action, "ContinueOnError").AsBoolean())
                {
                    errorMessages.Add(ex.Message);
                    return false;
                }
                else
                {
                    return true;
                }
            }

        }
    }
}
