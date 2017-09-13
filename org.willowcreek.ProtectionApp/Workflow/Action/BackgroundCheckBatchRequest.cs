// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
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
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Workflow;
using Rock;
using org.willowcreek.Security.BackgroundCheck;

namespace org.willowcreek.Workflow.Action
{
    /// <summary>
    /// Sends a Background Check Request.
    /// </summary>
    [ActionCategory("Background Check")]
    [Description("Reruns a Background Check Request.")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Background Check ReRun Request")]

    [WorkflowAttribute("Person Attribute", "The Person attribute that contains the person who the background check should be submitted for.", true, "", "", 1, null,
        new string[] { "Rock.Field.Types.PersonFieldType" })]
    [WorkflowAttribute("Old Report ID", "The attribute that contains the old Report ID for the person who the background check should be reran for", false, "", "", 2, null)]
    public class BackgroundCheckRequest : ActionComponent
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

            try
            {
                var provider = new ProtectMyMinistry();
                var personAttribute = AttributeCache.Read(GetAttributeValue(action, "PersonAttribute").AsGuid());
                var oldReportIdAttribute = AttributeCache.Read(GetAttributeValue(action, "OldReportID").AsGuid());

                return provider.RerunRequest(rockContext, action.Activity.Workflow, personAttribute, oldReportIdAttribute, out errorMessages);
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                ExceptionLogService.LogException(ex, System.Web.HttpContext.Current);
                errorMessages.Add(ex.Message);
                return false;
            }
        }
    }
}