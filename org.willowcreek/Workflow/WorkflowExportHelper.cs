using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock.Data;
using Rock.Model;

namespace org.willowcreek.ProfileUpdate.Workflow
{
    public class WorkflowExportHelper
    {
        public const string SqlServerStringFormat = "yyyy-MM-dd HH:mm:ss.fff";

        public static int _ActivityInserts = 0;
        public static int _ActionInserts = 0;
        public static int _AttributeInserts = 0;
        public static int _AttributeValueInserts = 0;

        /// <summary>
        /// Generates a SQL stream of inserts for the workflow, for use in another database.
        /// The caller must free the stream memory.
        /// </summary>
        /// <param name="workflowTypeId"></param>
        /// <returns></returns>
        public static MemoryStream GenerateWorkflowSql(int workflowTypeId)
        {
            _ActivityInserts = 0;
            _ActionInserts = 0;
            _AttributeInserts = 0;
            _AttributeValueInserts = 0;

            MemoryStream outputStream = null;
            try
            {
                outputStream = new MemoryStream();
                StreamWriter writer = new StreamWriter(outputStream);

                var rockContext = new RockContext();
                var workflowType = new WorkflowTypeService(rockContext).Get(workflowTypeId);

                WriteHeader(ref writer);

                ProcessWorkflow(workflowType, rockContext, ref writer);

                writer.Flush();
            }
            catch (Exception ex)
            {
                if (outputStream != null)
                {
                    outputStream.Close();
                    outputStream.Dispose();
                    outputStream = null;
                }

                // Don't just stand there, do something!!!
                throw ex;

            }

            return outputStream;
        }

        private static void WriteHeader(ref StreamWriter writer)
        {
            // TODO Parameterize and perhaps make a dialog on the Detail form that captures the target database...
            writer.WriteLine("USE [WillowCreekRock]");
            writer.WriteLine("GO");
            writer.WriteLine();
            writer.WriteLine("DECLARE @workflowId int");
            writer.WriteLine("DECLARE @activityId int");
            writer.WriteLine("DECLARE @actionId int");
            writer.WriteLine("DECLARE @attributeId int");
            writer.WriteLine();
        }

        private static void ProcessWorkflow(WorkflowType workflowType, RockContext rockContext, ref StreamWriter writer)
        {
            // Write the workflow insert...
            var workflowInsert = GenerateWorkflowTypeInsert(workflowType);
            writer.WriteLine(workflowInsert);
            writer.WriteLine();

            // Capture the new ID...
            GenerateWorkflowIdVariable(ref writer);

            // Get the attributes that may or may not exist on the workflow...
            ProcessWorkflowAttributes(workflowType, rockContext, ref writer);

            // And now process the activities and actions...
            ProcessWorkflowActivities(workflowType, rockContext, ref writer);

            // Last go!
            writer.WriteLine("GO");

            writer.WriteLine("-- Activities:  " + _ActivityInserts);
            writer.WriteLine("-- Actions:  " + _ActionInserts);
            writer.WriteLine("-- Attributes:  " + _AttributeInserts);
            writer.WriteLine("-- Attribute Values:  " + _AttributeValueInserts);
        }

        private static void ProcessWorkflowActivities(WorkflowType workflowType, RockContext rockContext, ref StreamWriter writer)
        {
            foreach (var activityType in workflowType.ActivityTypes)
            {
                // Do the raw insert of the activity...
                var activityInsert = GenerateActivityTypeInsert(activityType);
                writer.WriteLine(activityInsert);
                _ActivityInserts++;

                // Put the variable in the SQL file that will hold the current activity ID
                GenerateActivityIdVariable(ref writer);

                // Loop through each of the attributes for the activity, if any...
                var attributeService = new AttributeService(rockContext);
                var workflowActivityTypeId = new Rock.Model.WorkflowActivity().TypeId;
                var existingAttributes = attributeService.Get(workflowActivityTypeId, "ActivityTypeId", activityType.Id.ToString()).Where(a => a.CreatedDateTime.HasValue);
                ProcessAttributeCollection(existingAttributes, "@activityId", true, rockContext, ref writer);

                // For each activity, we need to generate the actions...
                ProcessActivityActions(activityType, rockContext, ref writer);
            }
        }

        private static void ProcessActivityActions(WorkflowActivityType activityType, RockContext rockContext, ref StreamWriter writer)
        {
            foreach (var action in activityType.ActionTypes)
            {
                // Do the raw insert of the action...
                var actionInsert = GenerateActionTypeInsert(action);
                writer.WriteLine(actionInsert);
                _ActionInserts++;

                // Set the variable to the ID of the last inserted item...well, generate the variable to sit in the SQL file...
                GenerateActionIdVariable(ref writer);

                // Now attempt to process all the attributes for the action...
                var attributeService = new AttributeService(rockContext);
                var workflowActionTypeId = new Rock.Model.WorkflowActionType().TypeId;
                var existingAttributes = attributeService.Get(workflowActionTypeId, "EntityTypeId", action.EntityTypeId.ToString()).Where(a => a.CreatedDateTime.HasValue);
                ProcessAttributeCollection(existingAttributes, action.EntityTypeId.ToString(), false, rockContext, ref writer);

                // Right now, I only see attribute values for actions that are not accounted for with attributes _created_ when the workflow was created...
                ProcessAttributeValues(action.Id, workflowActionTypeId, rockContext, ref writer);
            }
        }

        private static void ProcessWorkflowAttributes(WorkflowType workflowType, RockContext rockContext, ref StreamWriter writer)
        {
            // Get the existing attributes for this entity type and qualifier value
            var attributeService = new AttributeService(rockContext);
            var workflowTypeId = new Rock.Model.Workflow().TypeId;
            var existingAttributes = attributeService.Get(workflowTypeId, "WorkflowTypeId", workflowType.Id.ToString());
            ProcessAttributeCollection(existingAttributes, "@workflowId", true, rockContext, ref writer);
        }

        private static void ProcessAttributeCollection(IQueryable<Rock.Model.Attribute> attributes, string parentId, bool processValues, RockContext rockContext, ref StreamWriter writer)
        {
            var attributeValueService = new AttributeValueService(rockContext);
            foreach (var attribute in attributes)
            {
                var attributeInsert = GenerateAttributeInsert(attribute, parentId);
                writer.WriteLine(attributeInsert);
                _AttributeInserts++;

                GenerateAttributeIdVariable(ref writer);

                if (processValues)
                {
                    // How to determine whether this is an attribute that requires a value...
                    var attributeValues = attributeValueService.GetByAttributeId(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueInsert = GenerateAttributeValueInsert(attributeValue, true);
                        writer.WriteLine(attributeValueInsert);
                        _AttributeValueInserts++;
                    }
                }
            }

        }

        private static void ProcessAttributeValues(int entityId, int entityTypeId, RockContext rockContext, ref StreamWriter writer)
        {
            var attributeValueService = new AttributeValueService(rockContext);

            // In order to get attribute values for attributes _not_ created by the new workflow, go get attribute values whose entity IDs
            // match actions (items) just created on the workflow.  Then make sure the attribute values are truly for those values by 
            // checking the parent attribute to make sure it's an action (item) type...
            var values = attributeValueService.GetByEntityId(entityId).Where(v => v.Attribute.EntityTypeId == entityTypeId);

            foreach (var attributeValue in values)
            {
                var attributeValueInsert = GenerateAttributeValueInsert(attributeValue, false);
                writer.WriteLine(attributeValueInsert);
                _AttributeValueInserts++;
            }
        }

        private static void GenerateWorkflowIdVariable(ref StreamWriter writer)
        {
            writer.WriteLine("SET @workflowId = SCOPE_IDENTITY()");
            writer.WriteLine();
        }

        private static void GenerateActivityIdVariable(ref StreamWriter writer)
        {
            writer.WriteLine("SET @activityId = SCOPE_IDENTITY()");
            writer.WriteLine();
        }

        private static void GenerateActionIdVariable(ref StreamWriter writer)
        {
            writer.WriteLine("SET @actionId = SCOPE_IDENTITY()");
            writer.WriteLine();
        }

        private static void GenerateAttributeIdVariable(ref StreamWriter writer)
        {
            writer.WriteLine("SET @attributeId = SCOPE_IDENTITY()");
            writer.WriteLine();
        }

        /// <summary>
        /// This will generate the insert.  
        /// It will not attempt to carry over created or modified alias ID.
        /// It will, however, try to carry over the category ID.  That means this will break for custom categories whose IDs
        /// are different across environments.  So much for Guids.
        /// </summary>
        /// <param name="workflowType"></param>
        /// <returns></returns>
        private static string GenerateWorkflowTypeInsert(WorkflowType workflowType)
        {
            string insertFormat = 
            @"INSERT [dbo].[WorkflowType] 
	                ([IsSystem], 
	                [IsActive], 
	                [Name], 
	                [Description], 
	                [CategoryId], 
	                [Order], 
	                [WorkTerm], 
	                [ProcessingIntervalSeconds], 
	                [IsPersisted], 
	                [LoggingLevel], 
	                [Guid], 
	                [CreatedDateTime], 
	                [ModifiedDateTime], 
	                [CreatedByPersonAliasId], 
	                [ModifiedByPersonAliasId], 
	                [ForeignId], 
	                [IconCssClass]) 
                VALUES 
	                (0, 
	                {0}, 
	                N'{1}', 
	                N'{2}', 
	                {3}, 
	                {4}, 
	                N'{5}', 
	                {6}, 
	                {7}, 
	                {8}, 
                    N'{9}', 
	                {10}, 
	                {11}, 
	                NULL, 
	                NULL, 
	                NULL, 
	                N'{12}')";
            
            
            string ret = String.Format(insertFormat,
                workflowType.IsActive.HasValue ? (workflowType.IsActive.Value ? "1" : "0") : "0",
                workflowType.Name.Replace("'", "''"),
                workflowType.Description.Replace("'", "''"),
                workflowType.CategoryId.HasValue ? workflowType.CategoryId.Value.ToString() : "NULL",
                workflowType.Order,
                workflowType.WorkTerm.Replace("'", "''"),
                workflowType.ProcessingIntervalSeconds.HasValue ? workflowType.ProcessingIntervalSeconds.Value.ToString() : "NULL",
                workflowType.IsPersisted ? "1" : "0",
                (int)workflowType.LoggingLevel,
                workflowType.Guid.ToString("D"),
                workflowType.CreatedDateTime.HasValue ? String.Format("CAST(N'{0}' AS DateTime)", workflowType.CreatedDateTime.Value.ToString(SqlServerStringFormat)) : "NULL",
                workflowType.ModifiedDateTime.HasValue ? String.Format("CAST(N'{0}' AS DateTime)", workflowType.ModifiedDateTime.Value.ToString(SqlServerStringFormat)) : "NULL",
                workflowType.IconCssClass);

            return ret;
        }

        /// <summary>
        /// Construct the attribute insert.
        /// The qualifier value will likely be a sequence id captured by a variable in the script.
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="entityTypeQualifierValue"></param>
        /// <returns></returns>
        private static string GenerateAttributeInsert(Rock.Model.Attribute attribute, string entityTypeQualifierValue)
        {
            string insertFormat =
            @"INSERT [dbo].[Attribute] 
	                ([IsSystem], 
	                [FieldTypeId], 
	                [EntityTypeId], 
	                [EntityTypeQualifierColumn], 
	                [EntityTypeQualifierValue], 
	                [Key], 
	                [Name], 
	                [Description], 
	                [Order], 
	                [IsGridColumn], 
	                [DefaultValue], 
	                [IsMultiValue], 
	                [IsRequired], 
	                [Guid], 
	                [CreatedDateTime], 
	                [ModifiedDateTime], 
	                [CreatedByPersonAliasId], 
	                [ModifiedByPersonAliasId], 
	                [ForeignId], 
	                [IconCssClass]) 
                VALUES 
	                (0, 
	                {0}, 
	                {1}, 
	                N'{2}', 
	                {3}, 
	                N'{4}', 
	                N'{5}', 
	                N'{6}', 
	                {7}, 
	                {8}, 
                    N'{9}',
	                {10}, 
                    {11},
                    N'{12}',
	                {13}, 
	                {14}, 
	                NULL, 
	                NULL, 
                    NULL,
	                N'{15}')";


            string ret = String.Format(insertFormat,
                attribute.FieldTypeId,
                attribute.EntityTypeId.HasValue ? attribute.EntityTypeId.Value.ToString() : "NULL",
                attribute.EntityTypeQualifierColumn,
                entityTypeQualifierValue,
                attribute.Key,
                attribute.Name.Replace("'", "''"),
                attribute.Description.Replace("'", "''"),
                attribute.Order,
                attribute.IsGridColumn ? "1" : "0",
                String.IsNullOrEmpty(attribute.DefaultValue) ? "" : attribute.DefaultValue,
                attribute.IsMultiValue ? "1" : "0",
                attribute.IsRequired ? "1" :"0",
                attribute.Guid.ToString("D"),
                attribute.CreatedDateTime.HasValue ? String.Format("CAST(N'{0}' AS DateTime)", attribute.CreatedDateTime.Value.ToString(SqlServerStringFormat)) : "NULL",
                attribute.ModifiedDateTime.HasValue ? String.Format("CAST(N'{0}' AS DateTime)", attribute.ModifiedDateTime.Value.ToString(SqlServerStringFormat)) : "NULL",
                attribute.IconCssClass);

            return ret;

        }

        /// <summary>
        /// Construct the attribute insert.
        /// The qualifier value will likely be a sequence id captured by a variable in the script.
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="entityTypeQualifierValue"></param>
        /// <returns></returns>
        private static string GenerateAttributeValueInsert(Rock.Model.AttributeValue attributeValue, bool useNewParent)
        {
            string ret = "";
            if (useNewParent)
            {
                string insertFormat =
                @"INSERT [dbo].[AttributeValue] 
                   ([IsSystem],
                   [AttributeId],
                   [EntityId],
                   [Value],
                   [Guid],
                   [CreatedDateTime],
                   [ModifiedDateTime],
                   [CreatedByPersonAliasId],
                   [ModifiedByPersonAliasId],
                   [ForeignId])
                VALUES 
	                (0, 
	                @attributeId, 
	                {0}, 
	                N'{1}', 
	                N'{2}', 
	                {3}, 
	                {4}, 
	                NULL, 
	                NULL,
                    NULL)";


                ret = String.Format(insertFormat,
                    attributeValue.EntityId.HasValue ? attributeValue.EntityId.Value.ToString() : "NULL",
                    attributeValue.Value.Replace("'", "''"),
                    attributeValue.Guid.ToString("D"),
                    attributeValue.CreatedDateTime.HasValue ? String.Format("CAST(N'{0}' AS DateTime)", attributeValue.CreatedDateTime.Value.ToString(SqlServerStringFormat)) : "NULL",
                    attributeValue.ModifiedDateTime.HasValue ? String.Format("CAST(N'{0}' AS DateTime)", attributeValue.ModifiedDateTime.Value.ToString(SqlServerStringFormat)) : "NULL");

            }
            else
            {
                string insertFormat =
                @"INSERT [dbo].[AttributeValue] 
                   ([IsSystem],
                   [AttributeId],
                   [EntityId],
                   [Value],
                   [Guid],
                   [CreatedDateTime],
                   [ModifiedDateTime],
                   [CreatedByPersonAliasId],
                   [ModifiedByPersonAliasId],
                   [ForeignId])
                VALUES 
	                (0, 
	                {0}, 
	                {1}, 
	                N'{2}', 
	                N'{3}', 
	                {4}, 
	                {5}, 
	                NULL, 
	                NULL,
                    NULL)";


                ret = String.Format(insertFormat,
                    attributeValue.AttributeId,
                    attributeValue.EntityId.HasValue ? attributeValue.EntityId.Value.ToString() : "NULL",
                    attributeValue.Value.Replace("'", "''"),
                    attributeValue.Guid.ToString("D"),
                    attributeValue.CreatedDateTime.HasValue ? String.Format("CAST(N'{0}' AS DateTime)", attributeValue.CreatedDateTime.Value.ToString(SqlServerStringFormat)) : "NULL",
                    attributeValue.ModifiedDateTime.HasValue ? String.Format("CAST(N'{0}' AS DateTime)", attributeValue.ModifiedDateTime.Value.ToString(SqlServerStringFormat)) : "NULL");

            }

            return ret;

        }

        private static string GenerateActivityTypeInsert(Rock.Model.WorkflowActivityType activity)
        {
            string insertFormat =
            @"INSERT [dbo].[WorkflowActivityType] 
	                ([IsActive], 
	                [WorkflowTypeId], 
	                [Name], 
	                [Description], 
	                [IsActivatedWithWorkflow], 
	                [Order], 
	                [Guid], 
	                [CreatedDateTime], 
	                [ModifiedDateTime], 
	                [CreatedByPersonAliasId], 
	                [ModifiedByPersonAliasId], 
	                [ForeignId]) 
                VALUES 
	                (1, 
	                @workFlowId, 
	                N'{0}', 
	                N'{1}', 
	                {2}, 
	                {3}, 
	                N'{4}', 
	                {5}, 
	                {6}, 
	                NULL, 
                    NULL,
	                NULL)";


            string ret = String.Format(insertFormat,
                activity.Name.Replace("'", "''"),
                activity.Description.Replace("'", "''"),
                activity.IsActivatedWithWorkflow ? "1" : "0",
                activity.Order,
                activity.Guid.ToString("D"),
                activity.CreatedDateTime.HasValue ? String.Format("CAST(N'{0}' AS DateTime)", activity.CreatedDateTime.Value.ToString(SqlServerStringFormat)) : "NULL",
                activity.ModifiedDateTime.HasValue ? String.Format("CAST(N'{0}' AS DateTime)", activity.ModifiedDateTime.Value.ToString(SqlServerStringFormat)) : "NULL");

            return ret;


        }

        private static string GenerateActionTypeInsert(Rock.Model.WorkflowActionType action)
        {
            // Get the EntityId for which the attribute is linked...
            // Workflow form id
            string insertFormat =
            @"INSERT [dbo].[WorkflowActionType] 
		            ([ActivityTypeId], 
		            [Name], 
		            [Order], 
		            [EntityTypeId], 
		            [IsActionCompletedOnSuccess], 
		            [IsActivityCompletedOnSuccess], 
		            [Guid], 
		            [CreatedDateTime], 
		            [ModifiedDateTime], 
		            [CreatedByPersonAliasId], 
		            [ModifiedByPersonAliasId], 
		            [ForeignId], 
		            [WorkflowFormId], 
		            [CriteriaAttributeGuid], 
		            [CriteriaComparisonType], 
		            [CriteriaValue]) 
                VALUES 
	                (@activityId, 
	                N'{0}', 
	                {1}, 
	                {2}, 
	                {3}, 
	                {4}, 
	                N'{5}', 
	                {6}, 
	                {7}, 
                    NULL,
	                NULL, 
                    NULL,
                    {8},
	                {9}, 
	                {10}, 
	                N'{11}')";


            string ret = String.Format(insertFormat,
                action.Name.Replace("'", "''"),
                action.Order,
                action.EntityTypeId,
                action.IsActionCompletedOnSuccess ? "1" : "0",
                action.IsActivityCompletedOnSuccess ? "1" : "0",
                action.Guid.ToString("D"),
                action.CreatedDateTime.HasValue ? String.Format("CAST(N'{0}' AS DateTime)", action.CreatedDateTime.Value.ToString(SqlServerStringFormat)) : "NULL",
                action.ModifiedDateTime.HasValue ? String.Format("CAST(N'{0}' AS DateTime)", action.ModifiedDateTime.Value.ToString(SqlServerStringFormat)) : "NULL",
                action.WorkflowFormId.HasValue ? action.WorkflowFormId.Value.ToString() : "NULL",
                action.CriteriaAttributeGuid.HasValue ? "N'" + action.CriteriaAttributeGuid.Value.ToString("D") + "'" : "NULL",
                (int)action.CriteriaComparisonType,
                action.CriteriaValue);

            return ret;


        }
    }
}
