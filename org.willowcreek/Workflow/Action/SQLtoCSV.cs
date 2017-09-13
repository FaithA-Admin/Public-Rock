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
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using System.IO;
using System.Text;
using System.Data;

namespace Rock.Workflow.Action
{
    /// <summary>
    /// Runs a SQL query
    /// </summary>
    [ActionCategory( "Utility" )]
    [Description( "Runs the specified SQL query to perform an action against the database." )]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "SQL to CSV" )]
    [CodeEditorField( "SQLQuery", "The SQL query to run. <span class='tip tip-lava'></span>", Web.UI.Controls.CodeEditorMode.Sql, Web.UI.Controls.CodeEditorTheme.Rock, 400, true, "", "", 0 )]
    [WorkflowAttribute("File Name Attribute", "The name of the file that will be sent to the user.", true, "", "", 1)]
    [WorkflowAttribute( "Redirect Location Attribute", "The URL where the file can be found", false, "", "", 2 )]
    [BooleanField( "Continue On Error", "Should processing continue even if SQL Error occurs?", false, "", 3 )]
    public class SQLtoCSV : ActionComponent
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

            var query = GetAttributeValue( action, "SQLQuery" );

            var mergeFields = GetMergeFields( action );
            query = query.ResolveMergeFields( mergeFields );

            try
            {
                var sqlResult = DbService.GetDataTable( query, System.Data.CommandType.Text, null );
                action.AddLogEntry( "SQL query has been run" );

                if ( sqlResult != null && sqlResult.Rows.Count > 0 )
                {
                    var csv = new StringBuilder();
                    foreach (DataRow row in sqlResult.Rows)
                    {
                        // Does not yet handle values that include a comma
                        csv.AppendLine(string.Join(",", row.ItemArray));
                    }

                    var binaryFileType = new BinaryFileTypeService(rockContext).Get(Rock.SystemGuid.BinaryFiletype.DEFAULT.AsGuid());

                    var fileName = "File.csv";
                    Guid? attributeGuid = GetAttributeValue(action, "FileNameAttribute").AsGuidOrNull();
                    if (attributeGuid.HasValue)
                    {
                        var attribute = AttributeCache.Read(attributeGuid.Value, rockContext);
                        if (attribute != null)
                        {
                            if (attribute.EntityTypeId == new Rock.Model.Workflow().TypeId)
                            {
                                fileName = action.Activity.Workflow.GetAttributeValue(attribute.Key);
                            }
                            else if (attribute.EntityTypeId == new Rock.Model.WorkflowActivity().TypeId)
                            {
                                fileName = action.Activity.GetAttributeValue(attribute.Key);
                            }
                        }
                    }

                    var binaryFile = new BinaryFile()
                    {
                        Guid = Guid.NewGuid(),
                        IsTemporary = true,
                        BinaryFileTypeId = binaryFileType.Id,
                        MimeType = "text/csv",
                        FileName = fileName, 
                        ContentStream = GenerateStreamFromString(csv.ToString())
                    };
                    
                    var binaryFileService = new BinaryFileService(rockContext);
                    binaryFileService.Add(binaryFile);
                    rockContext.SaveChanges();

                    string redirectUrl = $"{binaryFile.Path.Replace("~", "")}&attachment=true";

                    attributeGuid = GetAttributeValue(action, "RedirectLocationAttribute").AsGuidOrNull();
                    if (attributeGuid.HasValue)
                    {
                        var attribute = AttributeCache.Read(attributeGuid.Value, rockContext);
                        if (attribute != null)
                        {
                            if (attribute.EntityTypeId == new Rock.Model.Workflow().TypeId)
                            {
                                action.Activity.Workflow.SetAttributeValue(attribute.Key, redirectUrl);
                                action.AddLogEntry(string.Format("Set '{0}' attribute to '{1}'.", attribute.Name, redirectUrl));
                            }
                            else if (attribute.EntityTypeId == new Rock.Model.WorkflowActivity().TypeId)
                            {
                                action.Activity.SetAttributeValue(attribute.Key, redirectUrl);
                                action.AddLogEntry(string.Format("Set '{0}' attribute to '{1}'.", attribute.Name, redirectUrl));
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                action.AddLogEntry( ex.Message, true );

                if ( !GetAttributeValue( action, "ContinueOnError" ).AsBoolean() )
                {
                    errorMessages.Add( ex.Message );
                    return false;
                }
                else
                {
                    return true;
                }
            }

        }

        public Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}
