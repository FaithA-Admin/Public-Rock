using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Rest;
using Rock.Rest.Filters;
using Rock.Web.Cache;
using org.willowcreek;
using org.willowcreek.ProtectionApp.Data;
using org.willowcreek.ProtectionApp.Logic;
using org.willowcreek.ProtectionApp.Model;
using Location = Rock.Model.Location;
using org.willowcreek.Workflow;

namespace org.willowcreek.ProtectionApp.Rest
{
    /// <summary>
    /// REST API for Referral Agencies
    /// </summary>

    public class QuestionnaireController : ApiController<Questionnaire>
    {
        private readonly ProtectionAppContext protectionContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionnaireController"/> class.
        /// </summary>
        public QuestionnaireController()
            : base(new QuestionnaireService(new Data.ProtectionAppContext()))
        {
            this.protectionContext = Service.Context as Data.ProtectionAppContext;
        }

        // POST api/<controller>
        [System.Web.Http.Route("api/questionnaire/references")]
        [System.Web.Http.HttpPost]
        public async Task<JToken> References(Questionnaire app)//[FromBody]
        {
            return GetReferences( app, false );
        }

        // POST api/<controller>
        [System.Web.Http.Route( "api/questionnaire/references_Spanish" )]
        [System.Web.Http.HttpPost]
        public async Task<JToken> References_Spanish( Questionnaire app )//[FromBody]
        {
            return GetReferences( app, true );
        }

        private JToken GetReferences( Questionnaire app, bool spanishVersion )
        {
            JArray errors = new JArray();
            string url = "";

            //Initiate the rock person, family and workflows
            try
            {
                string workflowId = app.WorkflowId;
                Guid workflowGuid = Guid.Parse( workflowId );
                Person ref1 = null;
                Person ref2 = null;
                Person ref3 = null;
                RockContext rockContext = new RockContext();
                WorkflowService workflowService = new WorkflowService( rockContext );
                //load the workflow so we can log activity against it
                Rock.Model.Workflow workflowInstance;
                workflowInstance = workflowService.Get( workflowGuid );
                //Get Parent Workflow because this will either be Application or References
                workflowInstance.LoadAttributes();
                var wkAttributes = workflowInstance.AttributeValues;
                var parentWorkflowId = Convert.ToInt32( wkAttributes["RequestWorkflowId"].Value );

                if ( !System.Web.HttpContext.Current.Items.Contains( "CurrentPerson" ) )
                {
                    System.Web.HttpContext.Current.Items.Add( "CurrentPerson", GetPerson() );
                }
                Service.Context.SaveChanges();

                rockContext.WrapTransaction( () =>
                {
                    PersonService personService = new PersonService( rockContext );

                    //TH - pass workflow guid to method, Pass questionnaire to method instead of just id
                    if ( string.IsNullOrEmpty( app.Reference1PersonAliasGuid ) )
                        ref1 = CreateReference( rockContext, personService, workflowInstance, app.ApplicantPersonAliasGuid, app.Reference1Name, string.Empty, app.Reference1Email, workflowGuid.ToString(), 1, app.Reference1NatureOfAssociation );
                    else
                        SetReferenceWorkflowAttribute( rockContext, workflowInstance, app.Reference1PersonAliasGuid, 1 );

                    if ( string.IsNullOrEmpty( app.Reference2PersonAliasGuid ) )
                        ref2 = CreateReference( rockContext, personService, workflowInstance, app.ApplicantPersonAliasGuid, app.Reference2Name, string.Empty, app.Reference2Email, workflowGuid.ToString(), 2, app.Reference2NatureOfAssociation );
                    else
                        SetReferenceWorkflowAttribute( rockContext, workflowInstance, app.Reference2PersonAliasGuid, 2 );

                    if ( string.IsNullOrEmpty( app.Reference3PersonAliasGuid ) )
                        ref3 = CreateReference( rockContext, personService, workflowInstance, app.ApplicantPersonAliasGuid, app.Reference3Name, string.Empty, app.Reference3Email, workflowGuid.ToString(), 3, app.Reference3NatureOfAssociation );
                    else
                        SetReferenceWorkflowAttribute( rockContext, workflowInstance, app.Reference3PersonAliasGuid, 3 );
                } );
                CreateReferenceListDocument( app, spanishVersion );
                var parentWorkflowInstance = workflowService.Get( parentWorkflowId );
                parentWorkflowInstance.LoadAttributes();
                wkAttributes = parentWorkflowInstance.AttributeValues;
                if ( !spanishVersion )
                {
                    url = ProtectionRouter.GetNextWorkflowUrl( parentWorkflowInstance, wkAttributes["RequestWorkflowId"].Value, spanishVersion );
                }
                else
                {
                    url = ProtectionRouter.GetNextWorkflowUrl( parentWorkflowInstance, wkAttributes["RequestWorkflowId"].Value, spanishVersion );
                }               

                // Process this workflow now to send emails to the references
                workflowInstance.ProcessAsync( workflowService );
            }
            catch ( Exception ex )
            {
                errors.Add( new JValue( ex.Message ) );
                var inner = ex.InnerException;
                while ( inner != null )
                {
                    errors.Add( new JValue( inner.Message ) );

                    inner = inner.InnerException;
                }
                ExceptionLogService.LogException( ex, System.Web.HttpContext.Current );
                throw;
            }

            JObject json = new JObject();
            json.Add( "success", new JValue( errors.Count == 0 ) );
            json.Add( "errors", errors );
            json.Add( "redirectUrl", new JValue( url ) );

            return json;
        }

        /// <summary>
        /// Method to generate and save reference list document
        /// </summary>
        private void CreateReferenceListDocument(Questionnaire app, bool spanishVersion)
        {
            RockContext rockContext = new RockContext();

            //Get person alias for applicant
            PersonAliasService aliasService = new PersonAliasService(rockContext);
            PersonAlias applicantAlias;

            applicantAlias = aliasService.Get(app.ApplicantPersonAliasGuid);
            //Fix full legal name for saved document
            app.FullLegalName = applicantAlias.Person.FullLegalName();

            string path = string.Empty;
            if (System.Web.HttpContext.Current != null)
            {
                if(!spanishVersion)
                {
                    path = System.Web.HttpContext.Current.Server.MapPath("~/Plugins/org_willowcreek/ProtectionApp/Templates/ReferenceListDetail.html");
                }else
                {
                    path = System.Web.HttpContext.Current.Server.MapPath( "~/Plugins/org_willowcreek/ProtectionApp_Spanish/Templates/ReferenceListDetail_Spanish.html" );
                }
            }

            try
            {
                if (!string.IsNullOrEmpty(path))
                {

                    string fileDescription = "Reference List";
                    string fileName = "ReferenceList.html";
                    Guid fileTypeGuid = ProtectionAppWorkflowHelper.PROTECTION_APP_REFERENCE_LIST_FILE_TYPE_ID;
                    Guid attributeGuid = ProtectionAppWorkflowHelper.PROTECTION_APP_REFERENCE_LIST_ATTRIBUTE_ID;
                    var html = string.Empty;
                    using (var docStream = new System.IO.MemoryStream())
                        html = Utility.GenerateDocument(rockContext, applicantAlias, path, app, fileTypeGuid,
                            fileName, fileDescription, attributeGuid, docStream);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogService.LogException(ex, System.Web.HttpContext.Current);
                throw;
            }
        }


        // POST api/<controller>
        [Authenticate]
        [System.Web.Http.Route("api/questionnaire")]
        [System.Web.Http.HttpPost]
        public new async Task<JToken> Questionnaire([FromBody]Questionnaire app)
        {
            return GetQuestionnaire( app, false );
        }

        // POST api/<controller>
        [Authenticate]
        [System.Web.Http.Route( "api/questionnaire_Spanish" )]
        [System.Web.Http.HttpPost]
        public new async Task<JToken> Questionnaire_Spanish( [FromBody]Questionnaire app )
        {
            return GetQuestionnaire( app, true );
        }

        private JToken GetQuestionnaire( Questionnaire app, bool spanishVersion )
        {
            JArray errors = new JArray();
            string url = "";
            string appSsn = "";

            //make sure the SSN is encrypted 
            if ( !string.IsNullOrEmpty( app.ApplicantSsn ) )
            {
                //    app.ApplicantSsn = Rock.Security.Encryption.EncryptString(app.ApplicantSsn);
                appSsn = app.ApplicantSsn;
                app.ApplicantSsn = "xxx-xx-xxxx";
            }

            SetProxyCreation( true );
            CheckCanEdit( app );
            Service.Add( app );

            if ( !System.Web.HttpContext.Current.Items.Contains( "CurrentPerson" ) )
            {
                System.Web.HttpContext.Current.Items.Add( "CurrentPerson", GetPerson() );
            }
            Service.Context.SaveChanges();

            if ( !app.IsValid )
            {
                foreach ( ValidationResult error in app.ValidationResults )
                    errors.Add( new JValue( error.ErrorMessage ) );
            }
            else
            {
                //Initiate the rock person, family and workflows
                try
                {
                    string workflowId = app.WorkflowId;
                    Guid workflowGuid = Guid.Parse( workflowId );
                    string path = string.Empty;
                    if ( System.Web.HttpContext.Current != null )
                    {
                        if ( !spanishVersion )
                        {
                            path = System.Web.HttpContext.Current.Server.MapPath(
                                                           "~/Plugins/org_willowcreek/ProtectionApp/Templates/ProtectionAppDetail.html" );
                        }else
                        {
                            path = System.Web.HttpContext.Current.Server.MapPath(
                                                           "~/Plugins/org_willowcreek/ProtectionApp_Spanish/Templates/ProtectionAppDetail_Spanish.html" );
                        }                       
                    }
                    PersonAlias alias = null;
                    Person ref1 = null;
                    Person ref2 = null;
                    Person ref3 = null;
                    RockContext rockContext = new RockContext();
                    WorkflowService workflowService = new WorkflowService( rockContext );
                    Rock.Model.Workflow workflowInstance;

                    //load the workflow so we can log activity against it
                    workflowInstance = workflowService.Get( workflowGuid );

                    PersonService personService = new PersonService( rockContext );
                    //Create the person
                    alias = CreatePerson( rockContext, personService, app, workflowInstance );
                    //TH - Set appropriate Guid
                    app.ApplicantPersonAliasGuid = alias.Guid;

                    int questionnaireId = app.Id;

                    if ( alias != null )
                    {
                        try
                        {
                            workflowInstance = workflowService.Get( workflowGuid );
                            InitiateWorkflow( rockContext, workflowInstance, app, ref1, ref2, ref3, alias );

                            if ( !string.IsNullOrEmpty( path ) )
                            {
                                string fileDescription = "Protection Application";
                                string fileName = "ProtectionApplication.html";
                                Guid fileTypeGuid = ProtectionAppWorkflowHelper.PROTECTION_APP_FILE_TYPE_ID;
                                Guid attributeGuid = ProtectionAppWorkflowHelper.PROTECTION_APP_ATTRIBUTE_ID;
                                var html = string.Empty;
                                using ( var docStream = new System.IO.MemoryStream() )
                                    html = Utility.GenerateDocument( rockContext, alias, path, app, fileTypeGuid,
                                        fileName, fileDescription, attributeGuid, docStream );
                            }
                        }
                        catch ( Exception ex )
                        {
                            ExceptionLogService.LogException( ex, System.Web.HttpContext.Current );
                            throw;
                        }
                    }
                    var wkAttributes = workflowInstance.AttributeValues;
                    url = ProtectionRouter.GetNextWorkflowUrl( workflowInstance, wkAttributes["RequestWorkflowId"].Value, spanishVersion);
                    ProtectionRouter.SetWorkflowSSN( workflowInstance, wkAttributes["RequestWorkflowId"].Value, appSsn );
                    Service.Context.SaveChanges();

                    workflowInstance.ProcessAsync( workflowService );
                }
                catch ( Exception ex )
                {
                    errors.Add( new JValue( ex.Message ) );
                    var inner = ex.InnerException;
                    while ( inner != null )
                    {
                        errors.Add( new JValue( inner.Message ) );

                        inner = inner.InnerException;
                    }
                    ExceptionLogService.LogException( ex, System.Web.HttpContext.Current );
                    throw;
                }
            }

            JObject json = new JObject();
            json.Add( "success", new JValue( errors.Count == 0 ) );
            json.Add( "errors", errors );

            json.Add( "redirectUrl", new JValue( url ) );
            return json;
        }



        /// <summary>
        /// Creates the reference.
        /// </summary>
        /// <param name="personService"></param>
        /// <param name="workflowInstance">Current workflow instance</param>
        /// <param name="applicantPersonAliasGuid"></param>
        /// <param name="name">The name.</param>
        /// <param name="phone">The phone.</param>
        /// <param name="email">The email.</param>
        /// <param name="workflowGuid">The current workflow</param>
        /// <param name="rockContext"></param>
        /// <param name="refNumber"></param>
        /// <param name="association"></param>
        /// //TH - added to keep references in sync with their workflow
        private Person CreateReference(RockContext rockContext, PersonService personService, Rock.Model.Workflow workflowInstance, Guid applicantPersonAliasGuid, string name, string phone, string email, string workflowGuid, int refNumber, string association)
        {
            char[] split = { ' ' };
            string[] names = name.Split(split, StringSplitOptions.RemoveEmptyEntries);
            string firstName;
            string lastName;

            if (names.Length >= 1)
            {
                firstName = names[0];
                lastName = names[names.Length - 1];
            }
            else
            {
                firstName = name;
                lastName = string.Empty;
            }

            //Create the person

            var person = new Person();
            workflowInstance.AddLogEntry(string.Format("Created new reference person"));
            workflowInstance.AddLogEntry(string.Format("Set reference person's Email to {0}", email));
            workflowInstance.AddLogEntry(string.Format("Set reference person's FirstName to {0}", firstName));
            workflowInstance.AddLogEntry(string.Format("Set reference person's LastName to {0}", lastName));

            person.Email = email;
            person.FirstName = firstName;
            person.LastName = lastName;
            //Inactive
            person.RecordStatusValueId = 4;
            //Reference GUID 
            DefinedValueCache status = DefinedValueCache.Read("0FA8D32F-492A-478F-8692-012BBC0B004C");  //Reference
            if (status != null)
                person.ConnectionStatusValueId = status.Id;

            if (!string.IsNullOrEmpty(phone))
            {
                person.PhoneNumbers = new List<PhoneNumber>
                {
                    new PhoneNumber
                    {
                        Number = phone,
                        NumberTypeValueId = 13,
                        PersonId = person.Id
                    }
                };
            }
            personService.Add(person);
            //Get proper person id for alias id
            rockContext.SaveChanges();

            //Get alias just created by rock
            var alias = rockContext.PersonAliases.Local.LastOrDefault();
            //Update name
            alias.Name = string.Join(" ", person.FirstName, person.LastName);

            rockContext.SaveChanges();

            //create the reference
            Reference reference = new Reference
            {
                //TH No longer care about questionnaire
                //QuestionnaireId = questionnaire.Id,
                WorkflowId = workflowGuid,
                //TH - Added to fix multiple rows added for 1 reference
                ReferencePersonAliasGuid = alias.Guid,
                FirstName = firstName,
                LastName = lastName,
                RefNumber = refNumber,
                ApplicantPersonAliasGuid = applicantPersonAliasGuid,
                Email = email,
                NatureOfRelationshipApplicant = association
            };

            //TH - need to fix this, reason why existing users dont get added to reference table.  Should add them anyway
            //            if (isNewPerson)
            protectionContext.References.Add(reference);
            protectionContext.SaveChanges();

            SetReferenceWorkflowAttribute(rockContext, workflowInstance, reference.ReferencePersonAliasGuid.ToString(), refNumber);

            return person;
        }

        /// <summary>
        /// Set workflow attribute for current reference
        /// </summary>
        /// <param name="rockContext"></param>
        /// <param name="referencePersonAliasGuid"></param>
        /// <param name="refNumber"></param>
        private void SetReferenceWorkflowAttribute(RockContext rockContext, Rock.Model.Workflow workflowInstance, string referencePersonAliasGuid, int refNumber)
        {
            //Set Workflow attributes to reference
            var workflowReferences = workflowInstance.AttributeValues.Where(a => a.Key.Contains("Reference"));
            var thisReference = workflowReferences.FirstOrDefault(r => r.Key.Contains(refNumber.ToString()));
            workflowInstance.SetAttributeValue(thisReference.Key, referencePersonAliasGuid.ToString());
            workflowInstance.SaveAttributeValues(rockContext);
        }

        /// <summary>
        /// Creates the person.
        /// </summary>
        /// <param name="personService"></param>
        /// <param name="app">The application.</param>
        /// <param name="workflowInstance">The workflow instance.</param>
        /// <param name="rockContext"></param>
        private PersonAlias CreatePerson(RockContext rockContext, PersonService personService, Questionnaire app, Rock.Model.Workflow workflowInstance)
        {
            PersonAliasService aliasService = new PersonAliasService(rockContext);
            //Create the person
            Person person;
            PersonAlias alias;

            bool isNewPerson = false; //app.ApplicantPersonAliasGuid;
            if (!isNewPerson)
            {
                var personAliasService = new PersonAliasService(rockContext);
                var existingAlias = personAliasService.Get(app.ApplicantPersonAliasGuid);
                person = personService.Get(existingAlias.PersonId);                                    //TODO: wrong? or correct now that retrieve is fixed?
            }
            else
                person = new Person();

            UpdatePersonDetails(rockContext, isNewPerson, workflowInstance, person, app);

            if (isNewPerson)
                personService.Add(person);
            rockContext.SaveChanges();//so we have a proper family Id

            if (isNewPerson)
            {
                alias = new PersonAlias
                {
                    AliasPersonId = person.Id,
                    AliasPersonGuid = person.Guid,
                    Name = string.Join(" ", person.FirstName, person.LastName),
                    PersonId = person.Id
                };
                aliasService.Add(alias);
                rockContext.SaveChanges();
            }
            else
            {
                alias = person.PrimaryAlias;
            }
            return alias;
        }

        private void UpdatePersonDetails(RockContext rockContext, bool isNewPerson, Rock.Model.Workflow workflowInstance, Person person, Questionnaire app)
        {
            if (isNewPerson)
            {
                workflowInstance.AddLogEntry(string.Format("Created new person"));
            }
            var demographicChanges = new List<string>();

            demographicChanges.Add("Protection Application");

            if (!isNewPerson && person.BirthDate != app.DateOfBirth)
                History.EvaluateChange(demographicChanges, "Birth Date", person.BirthDate, app.DateOfBirth);

            if (!isNewPerson && person.Email != app.EmailAddress)
                History.EvaluateChange(demographicChanges, "Email", person.Email, app.EmailAddress);

            if (!isNewPerson && person.FirstName != app.LegalFirstName)
                History.EvaluateChange(demographicChanges, "First Name", person.FirstName, app.LegalFirstName);

            if (!isNewPerson && person.MiddleName != app.MiddleName)
                History.EvaluateChange(demographicChanges, "Middle Name", person.MiddleName, app.MiddleName);

            if (!isNewPerson && person.LastName != app.LastName)
                History.EvaluateChange(demographicChanges, "Last Name", person.LastName, app.LastName);

            if (!isNewPerson && person.SuffixValueId != app.Suffix)
                History.EvaluateChange(demographicChanges, "Suffix", person.SuffixValueId, app.Suffix);

            if (!isNewPerson && person.NickName != app.Nickname)
                History.EvaluateChange(demographicChanges, "Nick Name", person.NickName, app.Nickname);

            if (!isNewPerson && person.Gender != app.Gender)
                History.EvaluateChange(demographicChanges, "Gender", person.Gender, app.Gender);

            if (!isNewPerson && person.MaritalStatusValueId != (int)app.MaritalStatus)
                History.EvaluateChange(demographicChanges, "Marital Status", person.MaritalStatusValueId.HasValue ? DefinedValueCache.GetName(person.MaritalStatusValueId) : string.Empty, DefinedValueCache.GetName((int)app.MaritalStatus));

            person.BirthMonth = app.DateOfBirth.Month;
            person.BirthDay = app.DateOfBirth.Day;
            person.BirthYear = app.DateOfBirth.Year;
            person.Email = app.EmailAddress;
            person.FirstName = app.LegalFirstName;
            person.MiddleName = app.MiddleName;
            person.LastName = app.LastName;
            person.SuffixValueId = app.Suffix;
            person.MaritalStatusValueId = (int)app.MaritalStatus;
            person.Gender = app.Gender;

            if (!string.IsNullOrEmpty(app.Nickname))
                person.NickName = app.Nickname;

            if (person.PhoneNumbers == null)
                person.PhoneNumbers = new List<PhoneNumber>();

            if (!string.IsNullOrEmpty(app.HomePhone))
            {
                var homePhone = person.PhoneNumbers.FirstOrDefault(x => x.NumberTypeValueId == 13);
                if (homePhone == null || string.IsNullOrEmpty(homePhone.Number))
                {
                    History.EvaluateChange(demographicChanges, "Home Phone", null, app.HomePhone);
                    homePhone = new PhoneNumber
                    {
                        Number = app.HomePhone,
                        //NumberTypeValue  = "Home"
                        NumberTypeValueId = 13,
                        PersonId = person.Id
                    };
                    person.PhoneNumbers.Add(homePhone);
                }
                else if (homePhone.NumberFormatted != app.HomePhone)
                {
                    History.EvaluateChange(demographicChanges, "Home Phone", homePhone.NumberFormatted, app.HomePhone);
                    homePhone.Number = app.HomePhone;
                }
            }

            if (!string.IsNullOrEmpty(app.MobilePhone))
            {
                var mobilePhone = person.PhoneNumbers.FirstOrDefault(x => x.NumberTypeValueId == 12);
                if (mobilePhone == null || string.IsNullOrEmpty(mobilePhone.Number))
                {
                    History.EvaluateChange(demographicChanges, "Mobile Phone", null, app.MobilePhone);
                    mobilePhone = new PhoneNumber
                    {
                        Number = app.MobilePhone,
                        //NumberTypeValue  = "Mobile"
                        NumberTypeValueId = 12,
                        PersonId = person.Id
                    };
                    person.PhoneNumbers.Add(mobilePhone);
                }
                else if (mobilePhone.NumberFormatted != app.MobilePhone)
                {
                    History.EvaluateChange(demographicChanges, "Mobile Phone", mobilePhone.NumberFormatted, app.MobilePhone);
                    mobilePhone.Number = app.MobilePhone;
                }
            }

            HistoryService.SaveChanges(rockContext, typeof(Person), Rock.SystemGuid.Category.HISTORY_PERSON_DEMOGRAPHIC_CHANGES.AsGuid(),
                person.Id, demographicChanges, null, typeof(Person), person.Id);

            GroupService.AddNewGroupAddress( rockContext, person.GetFamily(rockContext), Rock.SystemGuid.DefinedValue.GROUP_LOCATION_TYPE_HOME
                , app.CurrentAddressStreet, null, app.CurrentAddressCity, app.CurrentAddressState, app.CurrentAddressZip, "US"
                , true, "Protection Application", true, true );

            rockContext.SaveChanges();
        }

        /// <summary>
        /// Initiates the rock.
        /// </summary>
        /// <param name="workflowInstance"></param>
        /// <param name="app">The app.</param>
        /// <param name="ref1"></param>
        /// <param name="ref2"></param>
        /// <param name="ref3"></param>
        private void InitiateWorkflow(RockContext rockContext, Rock.Model.Workflow workflowInstance, Questionnaire app, Person ref1, Person ref2, Person ref3, PersonAlias alias)
        {
            //trigger workflow           
            // To save an attribute on a workflow, one must load them all first, set a value, then save again (or use the attribute service directly)...
            workflowInstance.LoadAttributes(rockContext);
            workflowInstance.SetAttributeValue("ApplicationComplete", "True");
            workflowInstance.SaveAttributeValues(rockContext);

            //final commit
            rockContext.SaveChanges();
        }
    }
}
