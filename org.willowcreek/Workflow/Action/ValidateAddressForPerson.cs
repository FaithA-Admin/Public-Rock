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

namespace org.willowcreek.Workflow.Action
{
    [ActionCategory("Person Update")]
    [Description("Sets flag on if Person's address is verified.")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Sets flag on if Person's address is verified")]

    //IN
    [WorkflowAttribute("Person", "(IN) The Person who's address will be retrieved.", true, "", "", 0, null, new[] { "Rock.Field.Types.PersonFieldType" })]

    //OUT
    [WorkflowAttribute("AddressVerified", "The attribute that contains the flag if the address was verified.", false, "", "", 3)]

    class ValidateAddressForPerson : ActionComponent
    {
        public override bool Execute(RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages)
        {
            errorMessages = new List<string>();
            try
            {
                var personGuidString = this.GetAttributeValue(action, "Person", rockContext);
                //Default invalid
                var validAddress = false;

                //Get Person Alias
                Guid personGuid = Guid.Parse(personGuidString);
                PersonAliasService ps = new PersonAliasService(rockContext);
                PersonAlias person = ps.Get(Guid.Parse(personGuid.ToString()));

                //Get Person
                PersonService personService = new PersonService(rockContext);
                Person personApplicant = personService.Get(person.Person.Guid);

                //Get Home Location
                var homelocation = personApplicant.GetHomeLocation();
                if (homelocation != null)
                {
                    //If the person has a county set validation true
                    if (!string.IsNullOrEmpty(homelocation.County))
                        validAddress = true;
                }

                //Get attribute guid
                Guid? addressVerifiedAttributeGuid = GetAttributeValue(action, "AddressVerified").AsGuidOrNull();
                //Set workflow attribute
                SetWorkflowAttributeValue(action, addressVerifiedAttributeGuid.Value, validAddress.ToString());

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
