using org.willowcreek.ProtectionApp.Data;
using org.willowcreek.ProtectionApp.Model;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace org.willowcreek.ProtectionApp.Logic
{
    public class ProtectionRerunBackgroundChecks
    {
        /// <summary>
        /// Rerun a list of people for background checks
        /// </summary>
        /// <param name="peopleIds"></param>
        public static string RerunAll(ProtectionAppContext rockContext, List<WillowActiveVolunteers> volunteers)
        {
            try
            {
                int rerunBC = 0, newBC = 0, exceptions = 0;
                bool isRerun;
                List<string> errorMessages = new List<string>();

                //For every volunteer
                foreach (var volunteer in volunteers)
                {
                    using (var newContext = new RockContext())
                    {
                        var workTypeService = new WorkflowTypeService(newContext);
                        var personAliasService = new PersonAliasService(newContext);
                        //Get background check workflow
                        var bcWorkType = workTypeService.Get(ProtectionAppWorkflowHelper.PROTECTION_APP_WORKFLOW_BACKGROUND_CHECK);
                        if (bcWorkType != null)
                        {
                            try
                            {
                                isRerun = false;
                                Rock.Model.Workflow nextWorkflow = Rock.Model.Workflow.Activate(bcWorkType, "Activated by Batch BC Rerun");

                                //load the attributes for the next workflow
                                nextWorkflow.LoadAttributes(newContext);

                                //Set Requester
                                if (nextWorkflow.Attributes.ContainsKey("Requester"))
                                {
                                    nextWorkflow.SetAttributeValue("Requester", "6E90CAA2-7C84-4084-BE4B-5AC149EE0475");
                                    //Hard-coded as rock admin for now
                                }
                                //Set Applicant
                                if (nextWorkflow.Attributes.ContainsKey("Applicant"))
                                {
                                    nextWorkflow.SetAttributeValue("Applicant", volunteer.Guid);
                                }


                                //Set last report id if exists
                                if (volunteer.OrderId.HasValue & volunteer.OrderId > 0)
                                {
                                    //If attribute exists
                                    if (nextWorkflow.Attributes.ContainsKey("ReportID"))
                                    {
                                        var nextAttr = nextWorkflow.Attributes["ReportID"];
                                        nextWorkflow.SetAttributeValue("ReportID", volunteer.OrderId.Value);

                                        nextWorkflow.AddLogEntry("Set attribute ReportID from existing report");
                                        isRerun = true;
                                    }
                                }
                                else
                                {
                                    if (nextWorkflow.Attributes.ContainsKey("SendApplicationForm"))
                                    {
                                        var nextAttr = nextWorkflow.Attributes["SendApplicationForm"];
                                        nextWorkflow.SetAttributeValue("SendApplicationForm", "false");
                                        nextWorkflow.AddLogEntry("Set attribute SendApplicationForm to false");
                                        isRerun = false;
                                    }

                                }

                                //TODO: may need to pass in an object instead of entity.  entity loses context after awhile, this often passes NULL.
                                if (ProcessWorkflow(newContext, nextWorkflow, bcWorkType, ref errorMessages))
                                {
                                    if (isRerun)
                                    {
                                        rerunBC++;
                                    }
                                    else
                                    {
                                        newBC++;
                                    }
                                    continue;
                                }
                                else
                                {
                                    foreach (string error in errorMessages)
                                        ExceptionLogService.LogException(new Exception(error), null, volunteer.Id);
                                    //the next workflow did not process correctly, we should log the errors
                                    throw new Exception(string.Format("Error processing next workflow {0}", nextWorkflow.Name));
                                }
                            }
                            catch (Exception ex)
                            {
                                while (ex.InnerException != null)
                                    ex = ex.InnerException;
                                ExceptionLogService.LogException(ex, null, volunteer.Id);
                                exceptions++;
                            }
                        }
                        else
                        {
                            //workflow type no longer exists
                            return string.Format("No workflow type with Guid {0} exists", ProtectionAppWorkflowHelper.PROTECTION_APP_WORKFLOW_BACKGROUND_CHECK);
                        }
                    }
                }

                return rerunBC + " rerun BCs, " + newBC + " new BCs, " + exceptions + " exceptions";
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                return ex.Message;
            }
        }

        /// <summary>
        /// Get volunteers who expire next week
        /// </summary>
        /// <param name="paContext"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static List<WillowActiveVolunteers> GetExpiringVolunteersByBC(ProtectionAppContext paContext)
        {
            var volunteersService = new WillowActiveVolunteersService(paContext);

            var monday = Extensions.GetNextWeekday(DateTime.Today.AddDays(1), DayOfWeek.Monday);
            var sunday = Extensions.GetNextWeekday(monday, DayOfWeek.Sunday);

            //Get global variables
            var rockContext = new RockContext();
            var globalAttributes = GlobalAttributesCache.Read(rockContext);
            int bcExpirationYears = Convert.ToInt32(globalAttributes.GetValue("ProtectionBackgroundCheckExpiration"));

            //Get all voluneers who are expired
            DateTime bcExpirationDate = DateTime.Now.Date.AddYears(-1);
            var volunteers = volunteersService.Queryable().AsNoTracking().Where(v => v.BackgroundCheckDate.HasValue
                                                        //Has an existing BC date
                                                        //And BC requested a year or more ago
                                                        && v.BackgroundCheckDate <= bcExpirationDate);

            return volunteers.ToList();
        }

        /// <summary>
        /// ProcessWorkflow
        /// </summary>
        /// <param name="rockContext"></param>
        /// <param name="workflow"></param>
        /// <param name="workType"></param>
        /// <param name="errorMessages"></param>
        /// <returns></returns>
        public static bool ProcessWorkflow(RockContext rockContext, Rock.Model.Workflow workflow, WorkflowType workType, ref List<string> errorMessages)
        {
            var workflowService = new WorkflowService(rockContext);
            workflow.IsPersisted = true;
            if (workflow.Id == 0)
            {
                workflowService.Add(workflow);
            }
            rockContext.SaveChanges();
            if (workflowService.Process(workflow, out errorMessages))
            {
                rockContext.WrapTransaction(() =>
                {
                    rockContext.SaveChanges();
                    workflow.SaveAttributeValues(rockContext);
                    foreach (var activity in workflow.Activities)
                    {
                        activity.SaveAttributeValues(rockContext);
                    }
                });

                return true;
            }
            return false;
        }
    }
}