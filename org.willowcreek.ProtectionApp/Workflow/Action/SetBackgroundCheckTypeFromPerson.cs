using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Workflow;
using org.willowcreek.Model.Extensions;
using Rock;
using Rock.Web.Cache;

namespace org.willowcreek.ProtectionApp.Workflow.Action
{
    [ActionCategory("Protection")]
    [Description("Sets background check type based on person.")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Sets Background Check Type based on Person")]

    //IN
    [WorkflowAttribute("Applicant", "(IN) The protection applicant who's summary will be retrieved.", true, "", "", 0, null, new[] { "Rock.Field.Types.PersonFieldType" })]

    //OUT
    [WorkflowAttribute("Type", "The attribute that contains the type of background check to submit (Specific to provider).", false, "", "", 3)]

    class SetBackgroundCheckTypeFromPerson : ActionComponent
    {
        public override bool Execute(RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            try
            {
                var backgroundCheckType = "PLUS";

                var applicantGuidString = this.GetAttributeValue(action, "Applicant", rockContext);

                Guid applicantGuid = Guid.Parse(applicantGuidString);
                PersonAliasService ps = new PersonAliasService(rockContext);
                PersonAlias applicant = ps.Get(Guid.Parse(applicantGuid.ToString()));

                PersonService personService = new PersonService(rockContext);
                Person personApplicant = personService.Get(applicant.Person.Guid);

                var homelocation = personApplicant.GetHomeLocation();
                if (homelocation != null)
                {
                    //If the person has a county of COOK or no county then set to BASIC package
                    if (!string.IsNullOrEmpty(homelocation.County) && homelocation.County.ToUpper() == "COOK")
                        backgroundCheckType = "BASIC";
                }

                if (backgroundCheckType != "BASIC")
                {
                    //Check age if not basic yet
                    var over18 = personApplicant.Age > 18;
                    if (!over18) backgroundCheckType = "BASIC";
                }

                var definedType = DefinedTypeCache.Read(Rock.SystemGuid.DefinedType.PROTECT_MY_MINISTRY_PACKAGES.AsGuid());
                Guid? packageTypeAttributeGuid = GetAttributeValue(action, "Type").AsGuidOrNull();

                if (backgroundCheckType == "BASIC")
                {
                    var definedValueCache = definedType.DefinedValues.FirstOrDefault(d => d.Value == "BASIC");
                    if (definedValueCache != null && packageTypeAttributeGuid != null)
                    {
                        var basicBcType = definedValueCache.Guid;
                        SetWorkflowAttributeValue(action, packageTypeAttributeGuid.Value, basicBcType.ToString());
                    }
                }

                //rockContext.SaveChanges();

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
