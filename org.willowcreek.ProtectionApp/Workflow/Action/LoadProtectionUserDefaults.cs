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

using org.willowcreek;
using org.willowcreek.Model.Extensions;
using org.willowcreek.ProtectionApp.Data;
using org.willowcreek.ProtectionApp.Model;


namespace org.willowcreek.ProtectionApp.Workflow.Action
{
    [ActionCategory("Protection")]
    [Description("Retrieve the user defaults for this person (initiator of protection request).")]
    [Export(typeof(ActionComponent))]
    [ExportMetadata("ComponentName", "Load Protection User Defaults")]

    //IN
    [BooleanField("Requester Only", "(IN) Load only the default requester.", false, "", 0)]
    [WorkflowAttribute("Initiator", "(IN) Person who started the protection request.", false, "", "", 1, null, new string[] { "Rock.Field.Types.PersonFieldType" })]
    [WorkflowAttribute("Applicant", "(IN) The person applying for protection.", false, "", "", 2, null, new string[] { "Rock.Field.Types.PersonFieldType" })]
    [WorkflowAttribute("Ministry", "(IN) The ministry to which applicant is applying", false, "", "", 2, null, new string[] { "Rock.Field.Types.DefinedValueFieldType" })]

    //IN & OUT
    [WorkflowAttribute("Requester", "(IN/OUT) Person responsible for the protection request (often the initiator).", false, "", "", 3, null, new string[] { "Rock.Field.Types.PersonFieldType" })]

    //OUT
    [WorkflowAttribute("Requester Email", "(OUT) Email of the reqester.", false, "", "", 4, null, new string[] { "Rock.Field.Types.EmailFieldType" })]
    [WorkflowAttribute("Applicant Email", "(OUT) Email of the applicant.", false, "", "", 5, null, new string[] { "Rock.Field.Types.EmailFieldType" })]
    [WorkflowAttribute("Reply To Name", "(OUT) The name of the person or ministry that the applicant can reply to.", false, "", "", 6, null, new string[] { "Rock.Field.Types.TextFieldType" })]
    [WorkflowAttribute("Reply To Email", "(OUT) The email of the person or ministry that the applicant can reply to.", false, "", "", 7, null, new string[] { "Rock.Field.Types.EmailFieldType" })]
    [WorkflowAttribute("Personal Message", "(OUT) The personal message to add to the email template.", false, "", "", 8, null, new string[] { "Rock.Field.Types.TextFieldType" })]
    [WorkflowAttribute("Email Template", "(OUT) The email template for the protection application email.", false, "", "", 9, null, new string[] { "Rock.Field.Types.EmailTemplateFieldType" })]

    class LoadProtectionUserDefaults : ActionComponent
    {

        public override bool Execute(RockContext rockContext, WorkflowAction action, Object entity, out List<string> errorMessages)
        {
            errorMessages = new List<string>();

            try
            {
                ProtectionAppContext pac = new ProtectionAppContext();
                PersonAliasService ps = new PersonAliasService(rockContext);

                //Requester Only
                var requesterOnly = this.GetAttributeValue(action, "RequesterOnly");
                if (Convert.ToBoolean(requesterOnly))
                {
                    var initiatorGuid = this.GetAttributeValue(action, "Initiator", rockContext).AsGuidOrNull();
                    PersonAlias thisInitiatorPersonAlias = ps.Get(Guid.Parse(initiatorGuid.ToString()));
                    var userDefaults = new UserDefaultsService(pac).Queryable().FirstOrDefault(a => a.PersonAliasGuid == thisInitiatorPersonAlias.AliasPersonGuid);
                    if (userDefaults != null)
                    {
                        PersonAlias requesterPersonAlias = ps.GetByAliasGuid(Guid.Parse(userDefaults.ProtectionRequesterPersonAliasGuid.ToString()));
                        this.SetAttributeValue(action, "Requester", rockContext, requesterPersonAlias.Guid.ToString());
                    }
                    else
                        this.SetAttributeValue(action, "Requester", rockContext, thisInitiatorPersonAlias.Guid.ToString());

                    return true;
                }

                //If NOT, Requester Only

                var applicantGuid = this.GetAttributeValue(action, "Applicant", rockContext);
                var requesterGuid = this.GetAttributeValue(action, "Requester", rockContext);
                var ministryGuid = this.GetAttributeValue(action, "Ministry", rockContext);

                // Start by setting the Reply values to the requester's name and email
                var requester = ps.GetPerson( requesterGuid.AsGuid() );
                this.SetAttributeValue( action, "ReplyToName", rockContext, requester.FullName );
                this.SetAttributeValue( action, "ReplyToEmail", rockContext, requester.Email );

                if (requesterGuid.Length > 0 && applicantGuid.Length > 0)
                {

                    PersonAlias thisRequesterPersonAlias = ps.Get(Guid.Parse(requesterGuid.ToString()));
                    PersonAlias thisApplicantPersonAlias = ps.Get(Guid.Parse(applicantGuid.ToString()));

                    //Retrieve Request & Applicant Email
                    this.SetAttributeValue(action, "RequesterEmail", rockContext, thisRequesterPersonAlias.Person.Email);
                    this.SetAttributeValue(action, "ApplicantEmail", rockContext, thisApplicantPersonAlias.Person.Email);

                    //Retrieve User Defaults
                    var userDefaults = new UserDefaultsService(pac).Queryable().FirstOrDefault(a => a.PersonAliasGuid == thisRequesterPersonAlias.Guid);

                    if (userDefaults != null)
                    {
                        if ( userDefaults.ProtectionReplyToName.Length > 0 )
                        {
                            this.SetAttributeValue( action, "ReplyToName", rockContext, userDefaults.ProtectionReplyToName );
                        }
                        if ( userDefaults.ProtectionReplyToEmail.Length > 0 )
                        {
                            this.SetAttributeValue( action, "ReplyToEmail", rockContext, userDefaults.ProtectionReplyToEmail );
                        }
                        this.SetAttributeValue(action, "PersonalMessage", rockContext, userDefaults.ProtectionEmailPersonalMessage);
                    }

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