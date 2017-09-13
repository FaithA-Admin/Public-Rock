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

using org.willowcreek.Model;
using org.willowcreek;
using org.willowcreek.Model.Extensions;

namespace org.willowcreek.Workflow.Action
{
    [ActionCategory("Organization Chart")]
    [Description("Sets the default campus and ministry for a person.")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Save Campus And Ministry For Person")]

    //IN
    [WorkflowAttribute("Person", "(IN) The attribute that contains the person who's campus & ministry default will be saved.", true, "", "", 0, null, new string[] { "Rock.Field.Types.PersonFieldType" })]
    [WorkflowAttribute("Campus", "(IN) The campus attribute to save.", false, "", "", 1, null, new string[] { "Rock.Field.Types.CampusFieldType" })]
    [WorkflowAttribute("Ministry", "(IN) The ministry attribute to save.", false, "", "", 2, null, new string[] { "Rock.Field.Types.DefinedValueFieldType" })]

    class SaveCampusAndMinistryForPerson : ActionComponent
    {

        public override bool Execute(RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            try
            {
                var personGuid = this.GetAttributeValue(action, "Person", rockContext).AsGuidOrNull();
                var campusGuid = this.GetAttributeValue(action, "Campus", rockContext).AsGuidOrNull();
                var ministryGuid = this.GetAttributeValue(action, "Ministry", rockContext).AsGuidOrNull();

                WillowContext wc = new WillowContext();

                if (!personGuid.IsNullOrEmpty())
                {
                    // look for full match - update modified date when exact match
                    // if no exact match or partial match create a new record
                    var campusMinistryPerson = new CampusMinistryPersonService(wc).Queryable().OrderByDescending(x => x.ModifiedDateTime).FirstOrDefault(a => a.PersonAliasGuid == personGuid && a.Ministry == ministryGuid && a.Campus == campusGuid);

                    if (campusMinistryPerson != null)
                    {
                        campusMinistryPerson.Ministry = ministryGuid;
                        campusMinistryPerson.Campus = campusGuid;
                        campusMinistryPerson.ModifiedDateTime = DateTime.Now;
                    }
                    else
                    {
                        var newCampusMinistryPerson = new CampusMinistryPerson
                        {
                            Campus = campusGuid,
                            Ministry = ministryGuid,
                            PersonAliasGuid = Guid.Parse(personGuid.ToString())
                        };

                        wc.CampusMinistryPersons.Add(newCampusMinistryPerson);
                    }

                    wc.SaveChanges();
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

    }
}
