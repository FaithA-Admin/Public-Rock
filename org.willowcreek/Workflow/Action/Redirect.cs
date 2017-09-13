using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Rock.Attribute;
using Rock.Data;
using Rock.Workflow;
using Rock.Model;

namespace org.willowcreek.Workflow.Action
{
    [ActionCategory("Utility")]
    [Description("Redirects the end-user to the url specified")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Redirect")]

    [WorkflowAttribute("Redirect Url", "The attribute that contains the Url the end-user should be redirected to.  This attribute must exist on the Workflow.")]
    public class Redirect : ActionComponent
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
                var url = GetRedirectUrl(rockContext, action, ref errorMessages);
                if (url == null)
                {
                    return false;
                }
                else
                {
                    //redirect the response, passing in false to NOT terminate the response otherwise the workflow state does not get persisted
                    if (System.Web.HttpContext.Current != null)
                        System.Web.HttpContext.Current.Response.Redirect(url, false);
                }

                return true;
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

        protected string GetRedirectUrl(Rock.Data.RockContext rockContext, Rock.Model.WorkflowAction action, ref List<string> errorMessages)
        {
            var attrGuid = GetAttributeValue(action, "RedirectUrl");
            if (String.IsNullOrEmpty(attrGuid))
            {
                errorMessages.Add("Redirect found no attribute holding an Url.  Please set it before executing this action.");
            }
            var attributeService = new AttributeService(rockContext);
            var attr = attributeService.Get(Guid.Parse(attrGuid));
            if (attr == null)
            {
                errorMessages.Add("Redirect could not find the specified Redirect Url attribute.  Please choose a valid initial date attribute for the workflow.");
            }
            var url = action.Activity.Workflow.GetAttributeValue(attr.Key);
            if (url == null)
            {
                url = action.Activity.GetAttributeValue(attr.Key);
                if (url == null)
                {
                    errorMessages.Add("Redirect found an empty Date for the Redirect Url.  Please assign a valid initial date attribute for the workflow.");
                }
            }

            return url.ToString();
        }

    }
}

