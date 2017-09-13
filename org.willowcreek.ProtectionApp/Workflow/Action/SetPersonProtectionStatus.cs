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

namespace org.willowcreek.ProtectionApp.Workflow.Action
{
    /// <summary>
    /// Sets an attribute's value to the selected person 
    /// </summary>
    [ActionCategory("Protection")]
    [Description("Sets a person attribute of a given person.")]
    [Export( typeof( ActionComponent ) )]
    [ExportMetadata( "ComponentName", "Set Person Attribute" )]

    [WorkflowAttribute("Person", "Workflow attribute that contains the person to update.", true, "", "", 0, null, new string[] { "Rock.Field.Types.PersonFieldType" } )]
    [WorkflowTextOrAttribute("Status", "New Protection Status", "The new status to set the person attribute to. <span class='tip tip-lava'></span>", false, "", "", 1, "Value", new string[] { "Rock.Field.Types.DefinedValueFieldType" })]
    
    public class SetPersonAttribute : ActionComponent
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

            string updateValue = GetAttributeValue(action, "Value");
            Guid? valueGuid = updateValue.AsGuidOrNull();
            if (valueGuid.HasValue)
            {
                updateValue = action.GetWorklowAttributeValue( valueGuid.Value );
            }
            //else
            //{
            //    updateValue = updateValue.ResolveMergeFields( GetMergeFields( action ) );
            //}

            string value = GetAttributeValue(action, "Person");
            var guid = value.AsGuid();
            if (!guid.IsEmpty())
            {
                var attributePerson = AttributeCache.Read(guid, rockContext);
                if (attributePerson != null)
                {
                    string attributePersonValue = action.GetWorklowAttributeValue(guid);
                    if (!string.IsNullOrWhiteSpace(attributePersonValue))
                    {
                        if (attributePerson.FieldType.Class == "Rock.Field.Types.PersonFieldType")
                        {
                            Guid personAliasGuid = attributePersonValue.AsGuid();
                            if (!personAliasGuid.IsEmpty())
                            {
                                var person = new PersonAliasService(rockContext).Queryable()
                                    .Where(a => a.Guid.Equals(personAliasGuid))
                                    .Select(a => a.Person)
                                    .FirstOrDefault();

                                string personAttribute = "B73BA93E-2D18-4EFC-A18B-AD68ACC91902";

                                guid = personAttribute.AsGuid();
                                if (!guid.IsEmpty())
                                {
                                    var attribute = AttributeCache.Read(guid, rockContext);

                                    if (person != null)
                                    {
                                        // update person attribute
                                        Rock.Attribute.Helper.SaveAttributeValue(person, attribute, updateValue, rockContext);
                                        action.AddLogEntry(string.Format("Set '{0}' attribute to '{1}'.", attribute.Name, person.FullName));
                                        return true;
                                    }
                                    else
                                    {
                                        errorMessages.Add(string.Format("Person could not be found for selected value ('{0}')!", guid.ToString()));
                                    }
                                }
                            }
                        }
                        else
                        {
                            errorMessages.Add("The attribute used to provide the person was not of type 'Person'.");
                        }
                    }
                }
            }
            errorMessages.ForEach(m => action.AddLogEntry(m, true));

            return true;
        }
    }
}