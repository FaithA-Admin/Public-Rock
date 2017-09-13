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
using org.willowcreek.Model.Extensions;

namespace org.willowcreek.Workflow.Action
{
    [ActionCategory("Organization Chart")]
    [Description("Sets the default campus and ministry from person.")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Set Campus And Ministry From Person")]

    //IN
    [WorkflowAttribute("Person", "(IN) The attribute that contains the person who's campus & ministry default will be retrieved.", true, "", "", 0, null, new string[] { "Rock.Field.Types.PersonFieldType" })]

    //OUT
    [WorkflowAttribute("Campus", "(OUT) The campus attribute to set the value of.", false, "", "", 1, null, new string[] { "Rock.Field.Types.CampusFieldType" })]
    [WorkflowAttribute("Ministry", "(OUT) The ministry attribute to set the value of.", false, "", "", 2, null, new string[] { "Rock.Field.Types.DefinedValueFieldType" })]

    class SetCampusAndMinistryFromPerson : ActionComponent
    {

        public override bool Execute(RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            try
            {
                var personGuid = this.GetAttributeValue(action, "Person", rockContext);

                if (personGuid.Length > 0)
                {

                    Guid? campusGuid = Guid.Empty;
                    Guid? ministryGuid = Guid.Empty;

                    //Get Campus and/or Ministry from defaults table

                    WillowContext wc = new WillowContext();
                    var campusMinistryPerson = new CampusMinistryPersonService(wc).Queryable().OrderByDescending(a => a.ModifiedDateTime).FirstOrDefault(a => a.PersonAliasGuid.ToString() == personGuid.ToString());
                    if (campusMinistryPerson != null)
                    {
                        if (!campusMinistryPerson.Campus.IsNullOrEmpty())
                            campusGuid = campusMinistryPerson.Campus;
                        if (!campusMinistryPerson.Ministry.IsNullOrEmpty())
                            ministryGuid = campusMinistryPerson.Ministry;
                    }

                    //Get Campus from person's family record, if there is no default already

                    if (campusGuid.IsNullOrEmpty())
                    {
                        PersonAliasService ps = new PersonAliasService(rockContext);
                        var thisPersonAlias = ps.Get(personGuid.AsGuid());
                        if (thisPersonAlias != null)
                        {
                            var thisPerson = thisPersonAlias.Person;
                            if (thisPerson != null)
                            {
                                var thisFamily = thisPerson.GetFamilies(rockContext).FirstOrDefault();
                                if (thisFamily != null && thisFamily.Campus != null)
                                {
                                    campusGuid = thisFamily.Campus.Guid;
                                }

                            }
                        }
                    }

                    //Set Attribute if they now have a value

                    if (!campusGuid.IsNullOrEmpty())
                        this.SetAttributeValue(action, "Campus", rockContext, campusGuid.ToString());

                    if (!ministryGuid.IsNullOrEmpty())
                        this.SetAttributeValue(action, "Ministry", rockContext, ministryGuid.ToString());

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
