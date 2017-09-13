using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.willowcreek.Workflow.Model;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using Rock.Workflow;

namespace org.willowcreek.Workflow.Action
{
    [ActionCategory("Utility")]
    [Description("Calculates passage of a specified time amount")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Elapse")]

    [WorkflowAttribute("Initial Date Attribute", "The attribute that contains the start of the interval for which elapsed time will be calculated.  This attribute must exist on the Workflow.")]
    [WorkflowAttribute("Destination Elapsed Attribute", "The attribute that will contain the value of the elapsed interval.", true, "", "", 1)]
    [CustomRadioListField("Interval Type", "Select an interval with which to measure elapsed time.", ElapseIntervalTypeHelper.ElapseIntervalOptions, true, "Days", Order = 2)]
    public class Elapse : ActionComponent
    {
        public override bool Execute(RockContext rockContext, WorkflowAction action, object entity, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            try
            {
                var date = GetInitialDateValue(rockContext, action, ref errorMessages);
                if (date == null)
                {
                    return false;
                }

                // Figure out what sort of timespan the user wanted...
                var intervalType = GetAttributeValue(action, "IntervalType");
                var elapseIntervalType = (ElapseIntervalType)Enum.Parse(typeof(ElapseIntervalType), intervalType);

                // How long has time elapsed...
                var intervalCount = DetermineIntervalCount(elapseIntervalType, date);

                // And set the new value on the Activity destination property...
                SetIntervalValue(intervalCount, rockContext, action, ref errorMessages);

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

        protected void SetIntervalValue(int intervalCount, Rock.Data.RockContext rockContext, Rock.Model.WorkflowAction action, ref List<string> errorMessages)
        {
            Guid guid = GetAttributeValue(action, "DestinationElapsedAttribute").AsGuid();
            if (!guid.IsEmpty())
            {
                // Get the property configured for the resulting date arithmetic...
                var attribute = AttributeCache.Read(guid, rockContext);
                if (attribute != null)
                {
                    action.Activity.SetAttributeValue(attribute.Key, intervalCount.ToString());
                    action.AddLogEntry(string.Format("Set '{0}' attribute to '{1}'.", attribute.Name, intervalCount));
                }
            }
        }

        protected DateTime? GetInitialDateValue(Rock.Data.RockContext rockContext, Rock.Model.WorkflowAction action, ref List<string> errorMessages)
        {
            // Get the initial date and time value, and keep in mind, the property stores an attribute, so we need to get its key and get its value...
            var initialDate = GetAttributeValue(action, "InitialDateAttribute");
            if (String.IsNullOrEmpty(initialDate))
            {
                errorMessages.Add("Elapse found no attribute holding an initial date.  Please set it before executing this action.");
            }

            // Gotta love metadata based system, all I got back was a darn Guid that tells me which attribute I got, so now I need a key, and then I can actually get the value...
            var attributeService = new AttributeService(rockContext);
            var initialDateAttribute = attributeService.Get(Guid.Parse(initialDate));
            if (initialDateAttribute == null)
            {
                errorMessages.Add("Elapse could not find the specified date attribute.  Please choose a valid initial date attribute for the workflow.");
            }
            var date = action.Activity.Workflow.GetAttributeValue(initialDateAttribute.Key).AsDateTime();
            if (date == null)
            {
                errorMessages.Add("Elapse found an empty Date for the initial date.  Please assign a valid initial date attribute for the workflow.");
            }

            return date;
        }

        protected int DetermineIntervalCount(ElapseIntervalType intervalType, DateTime? date)
        {
            int ret = 0;
            if (date.HasValue)
            {
                var span = DateTime.Now - date;
                if (span.HasValue)
                {
                    switch (intervalType)
                    {
                        case ElapseIntervalType.Days:
                            ret = Convert.ToInt32(span.Value.TotalDays);
                            break;
                        case ElapseIntervalType.Hours:
                            ret = Convert.ToInt32(span.Value.TotalHours);
                            break;
                        case ElapseIntervalType.Minutes:
                            ret = Convert.ToInt32(span.Value.TotalMinutes);
                            break;
                        case ElapseIntervalType.Seconds:
                            ret = Convert.ToInt32(span.Value.TotalSeconds);
                            break;
                    }
                }
            }
            return ret;
        }
    }
}
