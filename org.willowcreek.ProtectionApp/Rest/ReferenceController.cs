using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Linq;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Rest;
using Rock.Rest.Filters;
using org.willowcreek;
using org.willowcreek.ProtectionApp.Data;
using org.willowcreek.ProtectionApp.Model;
using org.willowcreek.Workflow;

namespace org.willowcreek.ProtectionApp.Rest
{
    /// <summary>
    /// REST API for Referral Agencies
    /// </summary>

    public class ReferenceController : ApiController<Reference>
    {
        private readonly ProtectionAppContext protectionContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceController"/> class.
        /// </summary>
        public ReferenceController()
            : base(new ReferenceService(new Data.ProtectionAppContext()))
        {
            this.protectionContext = Service.Context as Data.ProtectionAppContext;
        }

        // GET api/<controller>
        [Authenticate, Secured]
        [System.Web.Http.Route("api/reference")]
        public new JToken Get()
        {

            JToken json = JObject.Parse("{ 'firstname' : 'Jason', 'lastname' : 'Voorhees' }");
            return json;

            //string queryString = Request.RequestUri.Query;
            //string type = System.Web.HttpUtility.ParseQueryString( queryString ).Get( "type" );
            //string term = System.Web.HttpUtility.ParseQueryString( queryString ).Get( "term" );

            //int key = int.MinValue;
            //if (int.TryParse(type, out key))
            //{
            //    var searchComponents = Rock.Search.SearchContainer.Instance.Components;
            //    if (searchComponents.ContainsKey(key))
            //    {
            //        var component = searchComponents[key];
            //        return component.Value.Search( term );
            //    }
            //}

            //throw new HttpResponseException(HttpStatusCode.BadRequest);
        }

        // GET api/<controller>/{id}
        [Authenticate, Secured]
        [System.Web.Http.Route("api/reference/{id}")]
        public new JToken Get(int id)
        {

            JToken json = JObject.Parse("{ 'firstname' : 'Jason', 'lastname' : 'Voorhees' }");
            return json;

            //string queryString = Request.RequestUri.Query;
            //string type = System.Web.HttpUtility.ParseQueryString( queryString ).Get( "type" );
            //string term = System.Web.HttpUtility.ParseQueryString( queryString ).Get( "term" );

            //int key = int.MinValue;
            //if (int.TryParse(type, out key))
            //{
            //    var searchComponents = Rock.Search.SearchContainer.Instance.Components;
            //    if (searchComponents.ContainsKey(key))
            //    {
            //        var component = searchComponents[key];
            //        return component.Value.Search( term );
            //    }
            //}

            //throw new HttpResponseException(HttpStatusCode.BadRequest);
        }

        // POST api/<controller>
        [Authenticate]
        [System.Web.Http.Route("api/reference")]
        [System.Web.Http.HttpPost]
        public new JToken Post([FromBody]Reference reference)
        {
            return GetReference( reference , false);
        }

        // POST api/<controller>
        [Authenticate]
        [System.Web.Http.Route( "api/reference_Spanish" )]
        [System.Web.Http.HttpPost]
        public new JToken Post_Spanish( [FromBody]Reference reference )
        {
            return GetReference( reference, true );
        }

        private JToken GetReference( Reference reference, bool spanishVersion)
        {
            JArray errors = new JArray();
            SetProxyCreation( true );
            CheckCanEdit( reference );

            var referenceId = reference.Id;
            var refService = new ReferenceService( protectionContext );
            var rQry = refService.Queryable();

            //get the reference
            var existingRef = rQry.FirstOrDefault( x => x.Id == referenceId );
            // if we have a completed reference, they are done
            if ( existingRef != null )
            {
                if ( !reference.IsValid )
                {
                    foreach ( ValidationResult error in reference.ValidationResults )
                        errors.Add( new JValue( error.ErrorMessage ) );
                }
                else
                {
                    //Update Existing reference record
                    existingRef.SubmissionDate = reference.SubmissionDate;
                    existingRef.KnownMoreThanOneYear = reference.KnownMoreThanOneYear;
                    existingRef.IsReference18 = reference.IsReference18;
                    existingRef.NatureOfRelationship = reference.NatureOfRelationship;
                    existingRef.MaintainRelationships = reference.MaintainRelationships;
                    existingRef.MaintainRelationshipsExplain = reference.MaintainRelationshipsExplain;
                    existingRef.RespectHealthyRelationalBoundaries = reference.RespectHealthyRelationalBoundaries;
                    existingRef.RespectHealthyRelationalBoundariesExplain = reference.RespectHealthyRelationalBoundariesExplain;
                    existingRef.CriminalOffenses = reference.CriminalOffenses;
                    existingRef.CriminalOffensesExplain = reference.CriminalOffensesExplain;
                    existingRef.ManipulativeBehavior = reference.ManipulativeBehavior;
                    existingRef.ManipulativeBehaviorExplain = reference.ManipulativeBehaviorExplain;
                    existingRef.InflictedEmotionalHarm = reference.InflictedEmotionalHarm;
                    existingRef.InflictedEmotionalHarmExplain = reference.InflictedEmotionalHarmExplain;
                    existingRef.TrustInChildCare = reference.TrustInChildCare;
                    existingRef.TrustInChildCareExplain = reference.TrustInChildCareExplain;
                    existingRef.WouldRecommend = reference.WouldRecommend;
                    existingRef.WouldRecommendExplain = reference.WouldRecommendExplain;
                    existingRef.Signature = reference.Signature;
                    existingRef.SignatureDate = reference.SignatureDate;
                    existingRef.ModifiedDateTime = reference.ModifiedDateTime;
                    existingRef.ModifiedByPersonAliasId = reference.ModifiedByPersonAliasId;

                    string path = string.Empty;
                    if ( System.Web.HttpContext.Current != null )
                    {
                        if ( reference.KnownMoreThanOneYear.HasValue && reference.KnownMoreThanOneYear.Value &&
                            reference.IsReference18.HasValue && reference.IsReference18.Value &&
                            reference.NatureOfRelationship != "Family" )
                        {
                            if ( !spanishVersion )
                            {
                                path = "~/Plugins/org_willowcreek/ProtectionApp/Templates/ReferenceDetail.html";
                            }else
                            {
                                path = "~/Plugins/org_willowcreek/ProtectionApp_Spanish/Templates/ReferenceDetail_Spanish.html";
                            }                           
                        }
                        else
                        {
                            if ( !spanishVersion )
                            {
                                path = "~/Plugins/org_willowcreek/ProtectionApp/Templates/ReferenceDetail_Invalid.html";
                            }
                            else
                            {
                                path = "~/Plugins/org_willowcreek/ProtectionApp_Spanish/Templates/ReferenceDetail_Invalid_Spanish.html";
                            }                           
                        }
                            
                        path = System.Web.HttpContext.Current.Server.MapPath( path );
                    }

                    try
                    {
                        var referenceAlias = protectionContext.Aliases.FirstOrDefault( x => x.Guid == reference.ReferencePersonAliasGuid );
                        var referencePerson = referenceAlias.Person;
                        Service.Context.SaveChanges();

                        var workflowId = reference.WorkflowId;
                        Guid workflowGuid = Guid.Parse( workflowId );
                        var rockContext = new RockContext();
                        var workflowService = new WorkflowService( rockContext );

                        //load the workflow so we can log activity against it
                        var workflowInstance = workflowService.Get( workflowGuid );
                        SaveReferenceDocument( rockContext, workflowInstance, reference, referencePerson, path );
                    }
                    catch ( Exception ex )
                    {
                        errors.Add( new JValue( ex.Message ) );
                        ExceptionLogService.LogException( ex, System.Web.HttpContext.Current );
                    }
                }
            }
            JObject json = new JObject();
            json.Add( "success", new JValue( errors.Count == 0 ) );
            json.Add( "errors", errors );
            json.Add( "data", JObject.FromObject( reference ) );
            return json;
        }

        /// <summary>
        /// Method to save reference input to the appropriate reference property on person and workflow
        /// </summary>
        /// <param name="rockContext"></param>
        /// <param name="workflowInstance"></param>
        /// <param name="reference"></param>
        /// <param name="referencePerson"></param>
        /// <param name="documentPath"></param>
        private void SaveReferenceDocument(RockContext rockContext, Rock.Model.Workflow workflowInstance,Reference reference,Person referencePerson,string documentPath)
        {
            try
            {
                //trigger workflow
                workflowInstance.LoadAttributes(rockContext);
                var applicantId = workflowInstance.AttributeValues["Applicant"].Value;
                Guid applicantGuid;
                Guid.TryParse(applicantId, out applicantGuid);

                var applicantAlias = protectionContext.Aliases.FirstOrDefault(x => x.Guid == applicantGuid);
                var referenceNumber = Convert.ToInt32(workflowInstance.AttributeValues["ReferenceNumber"].Value);

                // Now process the workflow...
                Guid fileTypeGuid = ProtectionAppWorkflowHelper.PROTECTION_APP_REFERENCE_FILE_TYPE_ID;
                Guid attributeGuid = new Guid();
                string fileName = "ProtectionApplication_Reference.html";
                string fileDescription = "Protection Application - Reference";
                // Time to activate other activities in the workflow
                var workflowActivityTypeService = new WorkflowActivityTypeService(rockContext);
                using (var docStream = new System.IO.MemoryStream())
                {
                    applicantAlias.Person.LoadAttributes(rockContext);
                    switch (referenceNumber)
                    {
                        case 1:
                            workflowInstance.SetAttributeValue("ReferenceComplete", "True");
                            workflowInstance.AddLogEntry(string.Format("Setting Reference 1"));
                            attributeGuid = ProtectionAppWorkflowHelper.PROTECTION_APP_REFERENCE_1_ATTRIBUTE_ID;

                            //If reference approved set date on person
                            if (IsReferenceApproved(reference))
                            {
                                applicantAlias.Person.SetAttributeValue("ProtectionReference1Date", DateTime.Now);
                            }
                            else
                            {
                                //Set to needs review
                                applicantAlias.Person.SetAttributeValue("ProtectionStatus", ProtectionAppWorkflowHelper.PROTECTION_STATUS_NEEDS_REVIEW);
                            }
                            break;
                        case 2:
                            workflowInstance.SetAttributeValue("ReferenceComplete", "True");
                            workflowInstance.AddLogEntry(string.Format("Setting Reference 2"));
                            attributeGuid = ProtectionAppWorkflowHelper.PROTECTION_APP_REFERENCE_2_ATTRIBUTE_ID;

                            //If reference approved set date on person
                            if (IsReferenceApproved(reference))
                            {
                                applicantAlias.Person.SetAttributeValue("ProtectionReference2Date", DateTime.Now);
                            }
                            else
                            {
                                //Set to needs review
                                applicantAlias.Person.SetAttributeValue("ProtectionStatus", ProtectionAppWorkflowHelper.PROTECTION_STATUS_NEEDS_REVIEW);
                            }
                            break;
                        case 3:
                            workflowInstance.SetAttributeValue("ReferenceComplete", "True");
                            workflowInstance.AddLogEntry(string.Format("Setting Reference 3"));
                            attributeGuid = ProtectionAppWorkflowHelper.PROTECTION_APP_REFERENCE_3_ATTRIBUTE_ID;

                            //If reference approved set date on person
                            if (IsReferenceApproved(reference))
                            {
                                applicantAlias.Person.SetAttributeValue("ProtectionReference3Date", DateTime.Now);
                            }
                            else
                            {
                                //Set to needs review
                                applicantAlias.Person.SetAttributeValue("ProtectionStatus", ProtectionAppWorkflowHelper.PROTECTION_STATUS_NEEDS_REVIEW);
                            }
                            break;
                    }
                    applicantAlias.Person.SaveAttributeValues(rockContext);

                    //Must save document after attribute values otherwise gets overwritten with null
                    if (!string.IsNullOrEmpty(documentPath))
                        Utility.GenerateDocument(rockContext, applicantAlias, documentPath, reference, fileTypeGuid, fileName, fileDescription, attributeGuid, docStream);

                    workflowInstance.SaveAttributeValues(rockContext);

                    if (System.Web.HttpContext.Current.Items["CurrentPerson"] == null)
                    {
                        var personAlias =
                            protectionContext.Aliases.FirstOrDefault(x => x.Guid == reference.ReferencePersonAliasGuid);
                        if (personAlias != null)
                        {
                            System.Web.HttpContext.Current.Items.Add("CurrentPerson",
                                protectionContext.People.FirstOrDefault(x => x.Id == personAlias.PersonId));
                        }
                    }

                    //final commit
                    rockContext.SaveChanges();

                    // Process this workflow now
                    workflowInstance.ProcessAsync(rockContext);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogService.LogException(ex, System.Web.HttpContext.Current);
            }
        }

        //private bool ProcessActivity(RockContext rockContext, Rock.Model.Workflow workflowInstance, WorkflowActivityTypeService workflowActivityTypeService, Guid activityID, object entity, out List<string> errorMessages)
        //{
        //    //var activityType = workflowActivityTypeService.Get(activityID);
        //    //workflowInstance.AddLogEntry(string.Format("Activating the '{0}' activity", activityType.ToString()));
        //    //var activity = WorkflowActivity.Activate(activityType, workflowInstance);
        //    return workflowInstance.Process(rockContext, entity, out errorMessages);
        //}

        public bool IsValidReference(Reference reference)
        {
            if (reference.KnownMoreThanOneYear.HasValue && 
                reference.KnownMoreThanOneYear.Value &&
                reference.IsReference18.HasValue && 
                reference.IsReference18.Value &&
                reference.NatureOfRelationship != "Family" &&
                reference.NatureOfRelationshipApplicant != "Family")
                return true;

            return false;
        }

        /// <summary>
        /// Method to determine if reference was approved or not
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public bool IsReferenceApproved(Reference reference)
        {
            if (reference.SubmissionDate.HasValue &&
                reference.KnownMoreThanOneYear.HasValue && reference.KnownMoreThanOneYear.Value &&
                reference.IsReference18.HasValue && reference.IsReference18.Value &&
                reference.NatureOfRelationship != "Family" &&
                reference.MaintainRelationships.HasValue && reference.MaintainRelationships.Value &&
                reference.RespectHealthyRelationalBoundaries.HasValue && !reference.RespectHealthyRelationalBoundaries.Value &&
                reference.CriminalOffenses.HasValue && !reference.CriminalOffenses.Value &&
                reference.ManipulativeBehavior.HasValue && !reference.ManipulativeBehavior.Value &&
                reference.InflictedEmotionalHarm.HasValue && !reference.InflictedEmotionalHarm.Value &&
                reference.TrustInChildCare.HasValue && reference.TrustInChildCare.Value &&
                reference.WouldRecommend.HasValue && reference.WouldRecommend == 1)
                return true;

            return false;
        }
    }
}
