using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Workflow;

using System;

namespace org.willowcreek.Model.Extensions
{

    public static class ActionComponentExtension
    {

        /// <summary>
        /// Retrieves an attribute Id
        /// </summary>
        /// <param name="action">The workflow action.</param>
        /// <param name="key">The attribute key.</param>
        /// <param name="rockContext">The Rock context.</param>
        /// <returns>
        ///   Attribute Id
        /// </returns>
        public static int? GetAttributeId(this ActionComponent actionComponent, WorkflowAction action, string key, RockContext rockContext)
        {
            int? ret = 0;
            Guid? guid = GetAttributeValue(action, key).AsGuidOrNull();
            if (guid.HasValue)
            {
                var attribute = AttributeCache.Read(guid.Value, rockContext);
                if (attribute != null)
                {
                    ret = attribute.Id;
                }
            }
            return ret;
        }

        /// <summary>
        /// Retrieves an attribute value from activity or workflow
        /// </summary>
        /// <param name="action">The workflow action.</param>
        /// <param name="key">The attribute key.</param>
        /// <param name="rockContext">The Rock context.</param>
        /// <returns>
        ///   Value as a string
        /// </returns>
        public static string GetAttributeValue(this ActionComponent actionComponent, WorkflowAction action, string key, RockContext rockContext)
        {
            string ret = null;
            Guid? guid = GetAttributeValue(action, key).AsGuidOrNull();
            if (guid.HasValue)
            {
                var attribute = AttributeCache.Read(guid.Value, rockContext);
                if (attribute != null)
                {
                    if (attribute.EntityTypeId == new Rock.Model.Workflow().TypeId)
                    {
                        ret = action.Activity.Workflow.GetAttributeValue(attribute.Key).ToStringSafe();
                    }
                    else if (attribute.EntityTypeId == new Rock.Model.WorkflowActivity().TypeId)
                    {
                        ret = action.Activity.GetAttributeValue(attribute.Key).ToStringSafe();
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Saves an attribute value for activity or workflow
        /// </summary>
        /// <param name="action">The workflow action.</param>
        /// <param name="key">The attribute key.</param>
        /// <param name="rockContext">The Rock context.</param>
        /// <returns></returns>
        public static void SetAttributeValue(this ActionComponent actionComponent, WorkflowAction action, string key, RockContext rockContext, string value)
        {
            Guid? guid = GetAttributeValue(action, key).AsGuidOrNull();
            if (guid.HasValue)
            {
                var attribute = AttributeCache.Read(guid.Value, rockContext);
                if (attribute != null)
                {
                    if (attribute.EntityTypeId == new Rock.Model.Workflow().TypeId)
                    {
                        action.Activity.Workflow.SetAttributeValue(attribute.Key, value);
                    }
                    else if (attribute.EntityTypeId == new Rock.Model.WorkflowActivity().TypeId)
                    {
                        action.Activity.SetAttributeValue(attribute.Key, value);
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves an attribute value. This has been duplicated from the ActionComponent's protected method.
        /// </summary>
        /// <param name="action">The workflow action.</param>
        /// <param name="key">The attribute key.</param>
        /// <returns>
        ///   Value as a string
        /// </returns>
        private static string GetAttributeValue(WorkflowAction action, string key)
        {
            var actionType = action.ActionTypeCache;

            var values = actionType.AttributeValues;
            if (values.ContainsKey(key))
            {
                var keyValues = values[key];
                if (keyValues != null)
                {
                    return keyValues.Value;
                }
            }

            if (actionType.Attributes != null &&
                actionType.Attributes.ContainsKey(key))
            {
                return actionType.Attributes[key].DefaultValue;
            }

            return string.Empty;
        }

    }

}
