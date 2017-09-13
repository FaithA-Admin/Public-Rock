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
using Rock.Workflow;

namespace org.willowcreek.Workflow.Action
{
    [ActionCategory("Workflow Control")]
    [Description("Starts another workflow")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Workflow Continuation")]

    //IN
    [WorkflowTypeField("Workflow", "(IN) The workflow to activate by this action", false, true)]

    //OUT
    [WorkflowAttribute("Workflow Id", "(OUT) The workflow Id of the workflow being started.", false, fieldTypeClassNames: new string[] { "Rock.Field.Types.IntegerFieldType" })]
    [WorkflowAttribute("Workflow Guid", "(OUT) The workflow Guid of the workflow being started.", false, fieldTypeClassNames: new string[] { "Rock.Field.Types.TextFieldType" } )]
    public class WorkflowContinuation : ActionComponent
    {
        /// <summary>
        /// Executes the specified rock context.
        /// </summary>
        /// <param name="rockContext">The rock context.</param>
        /// <param name="action">The action.</param>
        /// <param name="entity">The entity.</param>
        /// <param name="errorMessages">The error messages.</param>
        /// <returns></returns>
        public override bool Execute(RockContext rockContext, Rock.Model.WorkflowAction action, object entity, out List<string> errorMessages)
        {
            errorMessages = new List<string>();
            try
            {
                var currentWorkflow = action.Activity.Workflow;
                //get this actions Workflow attribute defined by the [WorkflowTypeField()] on this class
                string workflowId = this.GetAttributeValue(action, "Workflow");
                Guid workflowGuid;
                if (Guid.TryParse(workflowId, out workflowGuid))
                {
                    var newContext = new RockContext();
                    var workTypeService = new WorkflowTypeService(newContext);
                    var nextWorkType = workTypeService.Get(workflowGuid);
                    if (nextWorkType != null)
                    {
                        Rock.Model.Workflow nextWorkflow = Rock.Model.Workflow.Activate(nextWorkType, string.Format("Activated by workflow {0}", currentWorkflow.Name));

                        //load the attributes for the next workflow
                        nextWorkflow.LoadAttributes(newContext);

                        //loop through each of the current workflow attributes and copy the values to the next workflow if the same key and type exists
                        foreach (var currentAttr in currentWorkflow.Attributes)
                        {
                            if (nextWorkflow.Attributes.ContainsKey(currentAttr.Key))
                            {
                                //we have matching keys
                                var nextAttr = nextWorkflow.Attributes[currentAttr.Key];
                                if (currentAttr.Value.FieldTypeId == nextAttr.FieldTypeId)
                                {
                                    //field types match, lets get the value from the current workflow and set the value on the next workflow
                                    string attributeValue = currentWorkflow.GetAttributeValue(currentAttr.Key);
                                    nextWorkflow.SetAttributeValue(currentAttr.Key, attributeValue);

                                    currentWorkflow.AddLogEntry(string.Format("Copying attribute {0}", currentAttr.Key));
                                    nextWorkflow.AddLogEntry(string.Format("Copied attribute {0} from workflow {1}", currentAttr.Key, currentWorkflow.Name));
                                }
                            }
                        }

                        //TODO: may need to pass in an object instead of entity.  entity loses context after awhile, this often passes NULL.
                        if (ProcessWorkflow(newContext, nextWorkflow, nextWorkType, entity, ref errorMessages))
                        {
                            //save nextWorkflow.Id to currentWorkflow
                            SetNewWorkflowIdValue(nextWorkflow.Id, rockContext, action, ref errorMessages);
                            SetNewWorkflowGuidValue( nextWorkflow.Guid, rockContext, action, ref errorMessages );
                            return true;
                        }
                        else
                        {
                            //the next workflow did not process correctly, we should log the errors
                            currentWorkflow.AddLogEntry(string.Format("Error processing next workflow {0}", nextWorkflow.Name));
                            foreach (string error in errorMessages)
                                currentWorkflow.AddLogEntry(error);

                            return false;
                        }
                    }
                    else
                    {
                        //workflow type no longer exists
                        currentWorkflow.AddLogEntry(string.Format("No workflow type with Guid {0} exists", workflowGuid));
                        return false;
                    }
                }
                else
                {
                    return false;
                }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rockContext"></param>
        /// <param name="workflow"></param>
        /// <param name="workType"></param>
        /// <param name="entity"></param>
        /// <param name="errorMessages"></param>
        /// <returns></returns>
        private bool ProcessWorkflow(RockContext rockContext, Rock.Model.Workflow workflow, WorkflowType workType, object entity, ref List<string> errorMessages)
        {
            var workflowService = new WorkflowService(rockContext);
            if (workflowService.Process(workflow, entity, out errorMessages))
            {
                workflow.IsPersisted = true;

                if (workflow.Id == 0)
                {
                    workflowService.Add(workflow);
                }

                rockContext.WrapTransaction(() =>
                {
                    rockContext.SaveChanges();
                    workflow.SaveAttributeValues(rockContext);
                    foreach (var activity in workflow.Activities)
                    {
                        activity.SaveAttributeValues(rockContext);
                    }
                });

                return true;
            }
            return false;
        }

        private void SetNewWorkflowIdValue(int newWorkflowId, Rock.Data.RockContext rockContext, Rock.Model.WorkflowAction action, ref List<string> errorMessages)
        {
            Guid? attributeGuid = GetAttributeValue(action, "WorkflowId").AsGuidOrNull();
            if (attributeGuid.HasValue)
            {
                var attribute = AttributeCache.Read(attributeGuid.Value, rockContext);
                if (attribute != null)
                {
                    string value = newWorkflowId.ToString();

                    if (attribute.EntityTypeId == new Rock.Model.Workflow().TypeId)
                    {
                        action.Activity.Workflow.SetAttributeValue(attribute.Key, value);
                        action.AddLogEntry(string.Format("Set '{0}' attribute to '{1}'.", attribute.Name, value));
                    }
                    else if (attribute.EntityTypeId == new Rock.Model.WorkflowActivity().TypeId)
                    {
                        action.Activity.SetAttributeValue(attribute.Key, value);
                        action.AddLogEntry(string.Format("Set '{0}' attribute to '{1}'.", attribute.Name, value));
                    }
                }
            }
        }

        private void SetNewWorkflowGuidValue( Guid newWorkflowGuid, Rock.Data.RockContext rockContext, Rock.Model.WorkflowAction action, ref List<string> errorMessages )
        {
            Guid? attributeGuid = GetAttributeValue( action, "WorkflowGuid" ).AsGuidOrNull();
            if ( attributeGuid.HasValue )
            {
                var attribute = AttributeCache.Read( attributeGuid.Value, rockContext );
                if ( attribute != null )
                {
                    string value = newWorkflowGuid.ToString();

                    if ( attribute.EntityTypeId == new Rock.Model.Workflow().TypeId )
                    {
                        action.Activity.Workflow.SetAttributeValue( attribute.Key, value );
                        action.AddLogEntry( string.Format( "Set '{0}' attribute to '{1}'.", attribute.Name, value ) );
                    }
                    else if ( attribute.EntityTypeId == new Rock.Model.WorkflowActivity().TypeId )
                    {
                        action.Activity.SetAttributeValue( attribute.Key, value );
                        action.AddLogEntry( string.Format( "Set '{0}' attribute to '{1}'.", attribute.Name, value ) );
                    }
                }
            }
        }

    }
}

