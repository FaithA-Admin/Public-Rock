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

using org.willowcreek.ProtectionApp.Data;
using org.willowcreek.ProtectionApp.Model;
using org.willowcreek.Model.Extensions;
using org.willowcreek.Model;

namespace org.willowcreek.ProtectionApp.Workflow.Action
{
    [ActionCategory("Protection")]
    [Description("Save the user defaults for this person (initiator of protection request).")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Save Protection User Defaults")]

    //IN
    [WorkflowAttribute("Initiator", "(IN) Person initiating the protection request.", true, "", "", 1, null, new string[] { "Rock.Field.Types.PersonFieldType" })]
    [WorkflowAttribute("Requester", "(IN) Person responsible for the protection request (often the initiator).", true, "", "", 2, null, new string[] { "Rock.Field.Types.PersonFieldType" })]
    [WorkflowAttribute("Requester Email", "(IN) Email of the reqester.", true, "", "", 3, null, new string[] { "Rock.Field.Types.EmailFieldType" })]
    [WorkflowAttribute("Applicant", "(IN) The person applying for protection.", true, "", "", 4, null, new string[] { "Rock.Field.Types.PersonFieldType" })]
    [WorkflowAttribute("Applicant Email", "(IN) Email of the applicant.", true, "", "", 5, null, new string[] { "Rock.Field.Types.EmailFieldType" })]
    [WorkflowAttribute("Reply To Name", "(IN) The name of the person or ministry that the applicant can reply to.", true, "", "", 6, null, new string[] { "Rock.Field.Types.TextFieldType" })]
    [WorkflowAttribute("Reply To Email", "(IN) The email of the person or ministry that the applicant can reply to.", true, "", "", 7, null, new string[] { "Rock.Field.Types.EmailFieldType" } )]
    [WorkflowAttribute("Personal Message", "(IN) The personal message to add to the email template.", true, "", "", 8, null, new string[] { "Rock.Field.Types.TextFieldType" })]

    class SaveProtectionUserDefaults : ActionComponent
    {

        public override bool Execute(RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            try
            {
                ProtectionAppContext pac = new ProtectionAppContext();
                PersonAliasService ps = new PersonAliasService(rockContext);

                var initiatorGuid = this.GetAttributeValue(action, "Initiator", rockContext);
                var applicantGuid = this.GetAttributeValue(action, "Applicant", rockContext);
                var requesterGuid = this.GetAttributeValue(action, "Requester", rockContext);

                var requesterEmail = this.GetAttributeValue(action, "RequesterEmail", rockContext);
                var applicantEmail = this.GetAttributeValue(action, "ApplicantEmail", rockContext);

                var replyToName = this.GetAttributeValue(action, "ReplyToName", rockContext);
                var replyToEmail = this.GetAttributeValue(action, "ReplyToEmail", rockContext);
                var personalMessage = this.GetAttributeValue(action, "PersonalMessage", rockContext);

                PersonAlias thisRequesterPersonAlias = ps.Get(Guid.Parse(requesterGuid));
                PersonAlias thisApplicantPersonAlias = ps.Get(Guid.Parse(applicantGuid));
                PersonAlias thisInitiatorPersonAlias = ps.Get(Guid.Parse(initiatorGuid));

                //retrieve people
                PersonService personService = new PersonService(rockContext);
                Person personRequester = personService.Get(thisRequesterPersonAlias.Person.Guid);
                Person personApplicant = personService.Get(thisApplicantPersonAlias.Person.Guid);

                var userDefaultsService = new UserDefaultsService(pac);

                if ((initiatorGuid.Length > 0 && requesterGuid.Length > 0))
                {

                    var userDefaultsInitiator = userDefaultsService.Queryable().FirstOrDefault(a => a.PersonAliasGuid == thisInitiatorPersonAlias.Guid);

                    //update user defaults for initiator

                    if (userDefaultsInitiator != null)
                    {
                        if (initiatorGuid != requesterGuid)
                        {
                            userDefaultsInitiator.ProtectionRequesterPersonAliasGuid = Guid.Parse(requesterGuid.ToString());
                        }

                        userDefaultsInitiator.PersonAliasGuid = thisInitiatorPersonAlias.Guid;
                        //TODO - Update to check if this is == current global default, if so don't save
                        userDefaultsInitiator.ProtectionReplyToName = replyToName;
                        userDefaultsInitiator.ProtectionReplyToEmail = replyToEmail;
                        userDefaultsInitiator.ProtectionRequesterPersonAliasGuid = thisRequesterPersonAlias.Guid;
                        userDefaultsInitiator.ProtectionEmailPersonalMessage = personalMessage;

                    }
                    else
                    {
                        var addDefaults = new UserDefaults
                        {
                            PersonAliasGuid = thisInitiatorPersonAlias.Guid,
                            ProtectionReplyToName = replyToName,
                            ProtectionReplyToEmail = replyToEmail,
                            ProtectionRequesterPersonAliasGuid = thisRequesterPersonAlias.Guid,
                            ProtectionEmailPersonalMessage = personalMessage
                        };

                        pac.UsersDefaults.Add(addDefaults);

                    }

                    //update requester & applicant email

                    var demographicChanges = new List<string>();
                    demographicChanges.Add("Modified");

                    personRequester.Email = requesterEmail;
                    personApplicant.Email = applicantEmail;

                    History.EvaluateChange(demographicChanges, "Email", personRequester.Email, requesterEmail);
                    History.EvaluateChange(demographicChanges, "Email", personApplicant.Email, applicantEmail);

                    pac.SaveChanges();
                    rockContext.SaveChanges();

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
