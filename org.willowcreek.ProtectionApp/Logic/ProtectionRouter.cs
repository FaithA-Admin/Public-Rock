using System;
using System.Linq;
using Rock;
using Rock.Model;
using Rock.Data;
using Rock.Security;
using System.Web;

namespace org.willowcreek.ProtectionApp.Logic
{
    public class ProtectionRouter
    {

        /*
         * TODO:
         *       method called from app save. do you continue on to reference list?  and what's the workflow id/guid?
         *       this is an extremely rough placeholder for:
         *              - when each protection step is completed, to determine what's next in the process. Like:  App to Ref List to Policy
        */

        public string Next(string TaskCompleted)
        {
            //...
            return null;
        }

        public class ValidatedWorkflow
        {
            public Rock.Model.Workflow Workflow;
            public bool ValidApplicant;
        }

        // TODO: Is this the best class to put this function in?
        public static ValidatedWorkflow GetValidatedWorkflow(RockContext context, WorkflowService workflowService, Person currentPerson, string workflowId, int workflowTypeId)
        {
            var result = new ValidatedWorkflow { Workflow = null, ValidApplicant = false };

            // Confirm that a WorkflowId was passed in
            if (string.IsNullOrEmpty(workflowId))
            {
                return result;
            }

            // Confirm that the WorkflowId is a valid Guid
            Guid workflowGuid;
            if (!Guid.TryParse(workflowId, out workflowGuid))
            {
                return result;
            }

            // Confirm that the WorkflowId refers to a real workflow
            var workflow = workflowService.Get(workflowGuid);
            if (workflow == null)
            {
                return result;
            }

            // Confirm that the specified workflow is a Protection Application
            if (workflow.WorkflowTypeId != workflowTypeId)
            {
                return result;
            }

            result.Workflow = workflow;

            // Confirm that a user is logged in
            if (currentPerson == null)
            {
                return result;
            }

            // Confirm that the current user is the applicant
            if (workflow.AttributeValues == null)
            {
                workflow.LoadAttributes(context);
            }
            var applicantAttribute = workflow.AttributeValues["Applicant"].Value;
            Guid applicantGuid;
            if (!Guid.TryParse(applicantAttribute, out applicantGuid))
            {
                return result;
            }
            var personAliasService = new PersonAliasService(context);
            var applicant = personAliasService.GetPerson(applicantGuid);
            if (applicant == null)
            {
                var personAlias = personAliasService.Get(applicantGuid);
                ExceptionLogService.LogException(new NullReferenceException("The workflow's Applicant attribute cannot be found."), HttpContext.Current, personAlias: personAlias);
                return result;
            }
            if (applicant.Id != currentPerson.Id)
            {
                return result;
            }

            result.ValidApplicant = true;
            return result;
        }

        /// <summary>
        /// Set BC workflow SSN attribute if exists
        /// </summary>
        /// <param name="currentWorkflow"></param>
        /// <param name="applicantSSN"></param>
        public static void SetWorkflowSSN(Rock.Model.Workflow currentWorkflow, string parentWorkflowId, string applicantSSN)
        {
            try
            {
                RockContext rockContext = new RockContext();
                WorkflowService workflowService = new WorkflowService(rockContext);
                Rock.Model.Workflow workflowInstance;
                //Validate we have the request workflow
                if (!string.IsNullOrEmpty(parentWorkflowId))
                {
                    var workflowId = Convert.ToInt32(parentWorkflowId);
                    workflowInstance = workflowService.Get(workflowId);
                }
                else
                {
                    //Currently never gets hit because Protection 01 Request has RequestWorkflowId attribute of itself
                    workflowInstance = currentWorkflow;
                }
                workflowInstance.LoadAttributes();
                //Get Request Workflow 'Start Other Workflows' activity
                var startWorkflowsActivity = workflowInstance.Activities.LastOrDefault(a => a.ActivityType.Name.ToLower().Contains("workflow"));
                startWorkflowsActivity.LoadAttributes();

                //Grab child workflow from activity attributes
                var backgroundKey = startWorkflowsActivity.AttributeValues.Keys.FirstOrDefault(k => k.ToLower().Contains("background"));
                var backgroundWorkflow = workflowService.Get(Convert.ToInt32(startWorkflowsActivity.AttributeValues[backgroundKey].ValueAsType));
                if (backgroundWorkflow != null)
                {
                    //Load attributes and set SSN attribute
                    backgroundWorkflow.LoadAttributes();
                    backgroundWorkflow.AttributeValues["SSN"].Value = Encryption.EncryptString(applicantSSN);
                    backgroundWorkflow.SaveAttributeValues(rockContext);
                }
            }
            //Exception should only be if we don't find the attributes or parent workflow
            catch (Exception ex)
            {
                //TODO Log error
            }
        }
        public static void GoToNextIncompleteStep(RockContext context, Rock.Model.Workflow workflowInstance, System.Web.HttpResponse response, bool spanishVersion)
        {
            var url = GetNextWorkflowUrl(context, workflowInstance, spanishVersion);
            if (!string.IsNullOrEmpty(url)) response.Redirect(url);
        }

        public static string GetNextWorkflowUrl(RockContext context, Rock.Model.Workflow currentWorkflow, bool spanishVersion)
        {
            if (currentWorkflow.AttributeValues == null)
            {
                currentWorkflow.LoadAttributes(context);
            }
            Rock.Web.Cache.AttributeValueCache value;
            if (currentWorkflow.AttributeValues.TryGetValue("RequestWorkflowId", out value))
            {
                return GetNextWorkflowUrl(currentWorkflow, value.Value, spanishVersion);
            }
            return string.Empty;
        }

        /// <summary>
        /// Method to return redirect url for next workflow based on current workflow and parent
        /// </summary>
        /// <param name="currentWorkflow"></param>
        /// <param name="parentWorkflowId"></param>
        /// <returns></returns>
        public static string GetNextWorkflowUrl(Rock.Model.Workflow currentWorkflow, string parentWorkflowId, bool spanishVersion)
        {
            try
            {
                RockContext rockContext = new RockContext();
                WorkflowService workflowService = new WorkflowService(rockContext);
                PersonAliasService paService = new PersonAliasService(rockContext);
                Rock.Model.Workflow workflowInstance;
                //Validate we have the request workflow
                if (!string.IsNullOrEmpty(parentWorkflowId))
                {
                    var workflowId = Convert.ToInt32(parentWorkflowId);
                    workflowInstance = workflowService.Get(workflowId);
                }
                else
                {
                    //Currently never gets hit because Protection 01 Request has RequestWorkflowId attribute of itself
                    workflowInstance = currentWorkflow;
                }
                workflowInstance.LoadAttributes();

                var returnUrl = string.Empty;

                if ( !spanishVersion )
                {
                    returnUrl = "/MyAccount/ProtectionComplete";
                }else
                {
                    returnUrl = "/MyAccount/ProtectionComplete_Spanish";
                }
               
                bool redirectToReferences = false;
                var applicantGuid = new Guid(workflowInstance.AttributeValues["Applicant"].Value);
                var applicant = paService.GetPerson(applicantGuid);

                //Get Request Workflow 'Start Other Workflows' activity
                // TODO: This is a terrible way to get the right activity, find a better way. - CM
                var startWorkflowsActivity = workflowInstance.Activities.LastOrDefault(a => a.ActivityType.Name.ToLower().Contains("workflow"));
                startWorkflowsActivity.LoadAttributes();

                if (currentWorkflow.WorkflowType.Name.Contains("Application"))
                {
                    //Check Reference Attribute
                    redirectToReferences = Convert.ToBoolean(workflowInstance.AttributeValues["RequestReferences"].Value);
                    if (redirectToReferences)
                    {
                        //Grab child workflow from activity attributes
                        // TODO: This is a terrible way to get the right activity, find a better way. - CM
                        var referenceKey = startWorkflowsActivity.AttributeValues.Keys.FirstOrDefault(k => k.ToLower().Contains("reference"));
                        var referenceWorkflow = workflowService.Get(Convert.ToInt32(startWorkflowsActivity.AttributeValues[referenceKey].ValueAsType));
                        if (referenceWorkflow.CompletedDateTime != null)
                        {
                            redirectToReferences = false;
                        }
                        else
                        {
                            if ( !spanishVersion )
                            {
                                returnUrl = "/MyAccount/ProtectionAppReferences/" + applicant.UrlEncodedKey + "?WorkflowId=" + referenceWorkflow.Guid;
                            }
                            else
                            {
                                returnUrl = "/MyAccount/ProtectionAppReferences_Spanish/" + applicant.UrlEncodedKey + "?WorkflowId=" + referenceWorkflow.Guid;
                            }                            
                        }
                    }

                }
                if (!redirectToReferences)
                {
                    var redirectToPolicy = Convert.ToBoolean(workflowInstance.AttributeValues["SendPolicyAcknowledgmentForm"].Value);
                    if (redirectToPolicy)
                    {
                        //Grab child workflow from activity attributes
                        // TODO: This is a terrible way to get the right activity, find a better way. - CM
                        var policyKey = startWorkflowsActivity.AttributeValues.Keys.FirstOrDefault(k => k.ToLower().Contains("policy"));
                        var policyWorkflow = workflowService.Get(Convert.ToInt32(startWorkflowsActivity.AttributeValues[policyKey].ValueAsType));
                        policyWorkflow.LoadAttributes();

                        var policyUrl = policyWorkflow.AttributeValues["LinkURL"].Value;
                        //Use the generated URL on the workflow otherwise create it
                        if (!String.IsNullOrEmpty(policyUrl))
                        {
                            var urlSplit = policyUrl.Split('/');
                            if ( !spanishVersion )
                            {
                                returnUrl = "/MyAccount/PolicyAcknowledgement/" + urlSplit.LastOrDefault();
                            }else
                            {
                                returnUrl = "/MyAccount/PolicyAcknowledgement_Spanish/" + urlSplit.LastOrDefault();
                            }                           
                        }
                        else
                        {
                            if ( !spanishVersion )
                            {
                                returnUrl = "/MyAccount/PolicyAcknowledgement/" + applicant.UrlEncodedKey + "?WorkflowId=" + policyWorkflow.Guid;
                            }else
                            {
                                returnUrl = "/MyAccount/PolicyAcknowledgement_Spanish/" + applicant.UrlEncodedKey + "?WorkflowId=" + policyWorkflow.Guid;
                            }                           
                        }
                    }
                }
                return returnUrl;
            }
            //Exception should only be if we don't find the attributes or parent workflow
            catch (Exception ex)
            {
                //TODO Log error
                return "";
            }
        }
    }
}
