using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Rock;
using Rock.Model;
using Rock.Data;
using Rock.Web.Cache;

//using org.willowcreek.ProtectionApp.Data;
using org.willowcreek.ProtectionApp.Model;
using org.willowcreek.Model.Extensions;


namespace org.willowcreek.ProtectionApp.Logic
{
    public class ProtectionEvaluator
    {
        /* TODO:
         *       We aren't going to call this on every workflow or during a form save.
         *       This is going to be called from the ProtectionReviewer job.
        */

        public void EvaluateAll()
        {
            //TODO: evalute every person that has a "P" badge in any status... and anyone who has any of the protection fields populated.

        }

        public void EvaluateRecent()
        {
            //TODO: evaluate only the applicants currently going through the protection process (acitve workflow, recent workflows, recent field updates, etc.)

        }

        //public bool InProtectionProcess(Guid applicantGuid)
        //{
        //    //TODO: evalute person for a "P" badge in any status... and anyone who has any of the protection fields populated.

        //    RockContext rockContext = new RockContext();
            
        //    PersonAliasService ps = new PersonAliasService(rockContext);

        //    PersonAlias thisInitiatorPersonAlias = ps.Get(Guid.Parse(applicantGuid.ToString()));
        //    ProtectionBadge? badges = thisInitiatorPersonAlias.Person.GetProtectionBadges();

        //    bool protectBadge = false;

        //    if (badges.HasValue)
        //    {
        //        var badge = badges.Value;
        //        //protection
        //        if (badge.HasFlag(ProtectionBadge.ProtectionInProcess) || badge.HasFlag(ProtectionBadge.ProtectionNeedsReview) || badge.HasFlag(ProtectionBadge.ProtectionApproved)
        //            || badge.HasFlag(ProtectionBadge.ProtectionNeedsReview) || badge.HasFlag(ProtectionBadge.ProtectionLimited) || badge.HasFlag(ProtectionBadge.ProtectionRestricted)
        //            || badge.HasFlag(ProtectionBadge.ProtectionUnknown))
        //        { protectBadge = true; }

        //    }

        //    return protectBadge;
        //}

        /// <summary>
        /// Determine if the applicant has two completed references already
        /// </summary>
        /// <param name="rockContext"></param>
        /// <param name="applicantGuid"></param>
        /// <returns></returns>
        public bool HasTwoCompletedReferences(RockContext rockContext, Guid applicantGuid)
        {
            PersonAliasService ps = new PersonAliasService(rockContext);
            PersonAlias applicantPersonAlias = ps.Get(applicantGuid);
            Person applicant = applicantPersonAlias.Person;

            applicant.LoadAttributes();
            //Get all 3 reference date attributes
            var ref1CompDate = applicant.AttributeValues["ProtectionReference1Date"].Value;
            var ref2CompDate = applicant.AttributeValues["ProtectionReference2Date"].Value;
            var ref3CompDate = applicant.AttributeValues["ProtectionReference3Date"].Value;
            var refCount = 0;

            refCount += (!string.IsNullOrEmpty(ref1CompDate)) ? 1 : 0;
            refCount += (!string.IsNullOrEmpty(ref2CompDate)) ? 1 : 0;
            refCount += (!string.IsNullOrEmpty(ref3CompDate)) ? 1 : 0;

            return refCount >= 2;
        }

        /// <summary>
        /// Validate protection steps are valid against expiration times
        /// </summary>
        /// <param name="rockContext"></param>
        /// <param name="attributeKey"></param>
        /// <param name="applicantGuid"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public bool CheckProtectionStepStatus(RockContext rockContext, string attributeKey, Guid applicantGuid, out string date)
        {

            date = "n/a";

            AttributeService attService = new AttributeService(rockContext);
            PersonAliasService ps = new PersonAliasService(rockContext);
            var globalAttributes = GlobalAttributesCache.Read(rockContext);

            string globalAtt = "none";

            if (attributeKey == "ProtectionApplicationDate")
                globalAtt = "ProtectionApplicationExpiration";
            else if (attributeKey == "ProtectionReference1Date" || attributeKey == "ProtectionReference2Date" || attributeKey == "ProtectionReference3Date")
                globalAtt = "ProtectionReferenceExpiration";
            else if (attributeKey == "BackgroundCheckDate")
                globalAtt = "ProtectionBackgroundCheckExpiration";
            else if (attributeKey == "PolicyAcknowledgmentDate")
                globalAtt = "ProtectionPolicyAcknowledgmentExpiration";

            int globalEvaluationBuffer = Convert.ToInt16(globalAttributes.GetValue("ProtectionEvaluationBuffer"));

            PersonAlias applicantPersonAlias = ps.Get(Guid.Parse(applicantGuid.ToString()));
           
            var thisAttributeCheck = attService.Queryable().Join(rockContext.AttributeValues, w => w.Id, av1 => av1.AttributeId, (w, av1) => new { w, av1 })
                .Where(r => r.w.Key == attributeKey)
                .Where(t => t.av1.EntityId == applicantPersonAlias.PersonId)
                .OrderByDescending(t => t.av1.CreatedDateTime).FirstOrDefault();

            if (thisAttributeCheck != null && !string.IsNullOrEmpty(thisAttributeCheck.av1.Value))
            {
                date = DateTime.Parse(thisAttributeCheck.av1.Value).ToString("M/d/yy");
                if (Convert.ToDateTime(thisAttributeCheck.av1.Value) > (DateTime.Today.AddYears(Convert.ToInt16(globalAttributes.GetValue(globalAtt)) * -1).AddDays(globalEvaluationBuffer)))
                {
                    return false;
                }
            }

            return true;

        }

        /// <summary>
        /// Get all workflows with an applicant with protection status given
        /// </summary>
        /// <param name="rockContext"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static List<Rock.Model.Workflow> GetWorkflowsNotInProtectionStatus(RockContext rockContext, Guid status = new Guid())
        {
            var workflowService = new WorkflowService(rockContext);
            
            // Get protection workflows that are NOT completed
            var protectionWorkflows = workflowService.Queryable().Where(w => w.WorkflowType.Category.Name == "Protection" && !w.CompletedDateTime.HasValue).ToList();
            
            // Get the Applicants
            protectionWorkflows.ForEach(w => w.LoadAttributes());
            var workflowPersons = protectionWorkflows.Where(w => w.AttributeValues["Applicant"].ValueAsType != null)
                                  .Join(rockContext.PersonAliases,
                                  w => w.AttributeValues["Applicant"].ValueAsType.ToString(), pa => pa.Guid.ToString(),
                                  (w, pa) => new { Workflow = w, Person = pa.Person }).ToList();
            
            workflowPersons.ForEach(wp => wp.Person.LoadAttributes());
            List<Rock.Model.Workflow> filtered = null;
            
            if (status != new Guid()) 
            {
                // Only get workflows where the Applicant's protection status = status parameter
                // TODO: Update this to use Guid instead of ValueFormatted
                filtered = workflowPersons
                    .Where(wpa => wpa.Person.AttributeValues["ProtectionStatus"].Value.AsGuid() != status)
                    .Select(wpa => wpa.Workflow).ToList();
            }
            else
            {
                // Get all workflows where the Applicant's protection status is not one of these
                var excludedStatuses = new [] {
                    ProtectionAppWorkflowHelper.PROTECTION_STATUS_PROCESS_INITIATED,
                    ProtectionAppWorkflowHelper.PROTECTION_STATUS_IN_PROGRESS,
                    ProtectionAppWorkflowHelper.PROTECTION_STATUS_NEEDS_REVIEW,
                    // This represents applicants that have no protection status, 
                    // because we don't yet know how or why someone would have no protection status and should not expire their workflow
                    Guid.Empty 
                };
                filtered = workflowPersons
                    .Where(wpa => !excludedStatuses.Contains(wpa.Person.AttributeValues["ProtectionStatus"].Value.AsGuid()))
                    .Select(wpa => wpa.Workflow).ToList();
            }

            return filtered;
        }
        
        /// <summary>
        /// Get every person with the current protection status
        /// </summary>
        /// <param name="rockContext"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public static List<Rock.Model.Person> GetPeopleByProtectionStatus(RockContext rockContext, Guid? statusGuid = null)
        {
            var personService = new PersonService(rockContext);

            //Get all people with attribute 'ProtectionStatus' in current status
            var avService = new AttributeValueService(rockContext);
            var attrValues = avService.Queryable().Where(av => av.Attribute.Key.Contains("ProtectionStatus")
                                                            && !string.IsNullOrEmpty(av.Value)
                                                            // Comparing strings because AsGuid() does not work in LINQ to Entity
                                                            && av.Value != ProtectionAppWorkflowHelper.PROTECTION_STATUS_EXPIRED.ToString()
                                                            && av.Value != ProtectionAppWorkflowHelper.PROTECTION_STATUS_DECLINED.ToString());

            if(statusGuid.HasValue)
                attrValues = attrValues.Where(av => av.Value.ToLower() == statusGuid.ToString().ToLower());

            var personIds = attrValues.Select(av => av.EntityId).ToList();
            var people = personService.Queryable().Where(p => personIds.Contains(p.Id)).ToList();
            return people;
        }

        /// <summary>
        /// Evaluate list of people for protection status
        /// </summary>
        /// <param name="people"></param>
        public static string EvaluateAll(RockContext rockContext, List<Person> people)
        {
            try
            {
                //Get Status'
                var definedType = rockContext.DefinedTypes.Where(dt => dt.Name == "Protection Status").FirstOrDefault();
                var statuses = rockContext.DefinedValues.Where(dv => dv.DefinedTypeId == definedType.Id);
                var DECLINED = statuses.FirstOrDefault(s => s.Value == "Declined");
                var APPROVED = statuses.FirstOrDefault(s => s.Value == "Approved");
                var EXPIRED = statuses.FirstOrDefault(s => s.Value == "Expired");
                var APPROVEDWITHREST = statuses.FirstOrDefault(s => s.Value == "Approved with restrictions");

                //Get global variables
                var globalAttributes = GlobalAttributesCache.Read(rockContext);
                int appExpirationYears = Convert.ToInt32(globalAttributes.GetValue("ProtectionApplicationExpiration"));
                int bcExpirationYears = Convert.ToInt32(globalAttributes.GetValue("ProtectionBackgroundCheckExpiration"));
                int polExpirationYears = Convert.ToInt32(globalAttributes.GetValue("ProtectionPolicyAcknowledgmentExpiration"));
                int refExpirationYears = Convert.ToInt32(globalAttributes.GetValue("ProtectionReferenceExpiration"));
                int noActExpirationDays = Convert.ToInt32(globalAttributes.GetValue("ProtectionWorkflowExpirationNoActivity"));
                int actExpirationDays = Convert.ToInt32(globalAttributes.GetValue("ProtectionWorkflowExpirationSomeActivity"));

                DateTime bcDate, appDate, polDate, r1Date, r2Date, r3Date, protInitDate;
                string bcResult, restriction, protStatus;
                int validDates, validReferences, approvedPeople = 0, declinedPeople = 0, expiredPeople = 0, exceptions = 0;

                //For every person
                foreach (var person in people)
                {
                    try
                    {
                        validDates = 0;
                        validReferences = 0;
                        var expirePerson = false;

                        person.LoadAttributes();

                        //Get all protection values needed
                        protStatus = person.AttributeValues["ProtectionStatus"].ValueFormatted;
                        protInitDate = (DateTime)(person.AttributeValues["ProtectionProcessInitiated"].ValueAsType ?? new DateTime());
                        appDate = (DateTime)(person.AttributeValues["ProtectionApplicationDate"].ValueAsType ?? new DateTime());
                        bcDate = (DateTime)(person.AttributeValues["BackgroundCheckDate"].ValueAsType ?? new DateTime());
                        polDate = (DateTime)(person.AttributeValues["PolicyAcknowledgmentDate"].ValueAsType ?? new DateTime());
                        r1Date = (DateTime)(person.AttributeValues["ProtectionReference1Date"].ValueAsType ?? new DateTime());
                        r2Date = (DateTime)(person.AttributeValues["ProtectionReference2Date"].ValueAsType ?? new DateTime());
                        r3Date = (DateTime)(person.AttributeValues["ProtectionReference3Date"].ValueAsType ?? new DateTime());
                        bcResult = person.AttributeValues["BackgroundCheckResult"].ValueFormatted;
                        restriction = person.AttributeValues["RestrictedFrom"].ValueFormatted;

                        //If restriction is volunteering or bc result is fail and no date set to declined and move on
                        if ((!string.IsNullOrEmpty(restriction) && restriction == "Volunteering") || (!string.IsNullOrEmpty(bcResult) && bcResult.ToLower() == "fail" && bcDate != new DateTime()))
                        {
                            if (!protStatus.Contains("Declined"))
                            {
                                UpdateProtectionStatus(person, DECLINED, true);
                                declinedPeople++;
                            }

                            continue;
                        }

                        //Don't set either of these in case the below cases get hit and they get another status
                        //Expired no activity or with activity
                        if (!string.IsNullOrEmpty(protStatus) && protStatus.Contains("Initiated") && protInitDate.AddDays(noActExpirationDays) < DateTime.Now)
                        {
                            expirePerson = true;
                        }
                        //Expired activity
                        if (!string.IsNullOrEmpty(protStatus) && protStatus.Contains("Progress") && protInitDate.AddDays(actExpirationDays) < DateTime.Now)
                        {
                            expirePerson = true;
                        }

                        //Validate application
                        if (appDate.AddYears(appExpirationYears) > DateTime.Now)
                            validDates++;
                        //Validate Background Check
                        if (bcDate.AddYears(bcExpirationYears) > DateTime.Now) //&& (!string.IsNullOrEmpty(bcResult) && bcResult.ToLower() == "pass"))
                            validDates++;
                        //Validate Policy Acknowledgement
                        if (polDate.AddYears(polExpirationYears) > DateTime.Now)
                            validDates++;
                        //Validate Ref1
                        if (r1Date.AddYears(refExpirationYears) > DateTime.Now)
                            validReferences++;
                        //Validate Ref2
                        if (r2Date.AddYears(refExpirationYears) > DateTime.Now)
                            validReferences++;
                        //Validate Ref3
                        if (r3Date.AddYears(refExpirationYears) > DateTime.Now)
                            validReferences++;

                        if (validReferences >= 2)
                            validDates += 2;

                        //Need 5 valid dates, unable to get to 5 without 2 references
                        if (validDates >= 5)
                        {
                            if (!string.IsNullOrEmpty(restriction))
                            {
                                if (!protStatus.ToLower().Contains("restrict"))
                                {
                                    UpdateProtectionStatus(person, APPROVEDWITHREST, true);
                                    approvedPeople++;
                                }
                                continue;
                            }
                            else
                            {
                                //Not currently approved
                                if (!protStatus.Contains("Approved"))
                                {
                                    UpdateProtectionStatus(person, APPROVED, true);
                                    approvedPeople++;
                                }
                                continue;
                            }
                        }
                        //Dont have enough valid dates
                        else
                        {
                            //And currently approved, expire
                            if (!string.IsNullOrEmpty(protStatus) && protStatus.Contains("Approved"))
                            {
                                if (!protStatus.Contains("Expired"))
                                {
                                    UpdateProtectionStatus(person, EXPIRED, true);
                                    expiredPeople++;
                                }
                                continue;
                            }
                        }

                        //If made it here set to the expired status otherwise leave it alone
                        if (expirePerson)
                        {
                            if (!protStatus.Contains("Expired"))
                            {
                                UpdateProtectionStatus(person, EXPIRED, true);
                                expiredPeople++;
                            }
                            continue;
                        }
                    }
                    catch(Exception ex)
                    {
                        while (ex.InnerException != null)
                            ex = ex.InnerException;
                        ExceptionLogService.LogException(ex, null, person.Id);
                        exceptions++;
                        if(exceptions >= 25)
                        {
                            throw ex;
                        }
                    }
                }

                return approvedPeople + " volunteers approved, " + expiredPeople + " volunteers expired, " + declinedPeople + " volunteers declined, " + exceptions + " exceptions";
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
                return ex.Message;
            }
        }

        private static void UpdateProtectionStatus(Person person, DefinedValue newProtectionStatus, bool clearInitiatedDate)
        {
            var context = new RockContext(); // Saves faster when using a fresh context
            var personChanges = new List<string>();

            // Update Protection Status
            var oldProtectionStatus = DefinedValueCache.Read(person.GetAttributeValue("ProtectionStatus"));
            History.EvaluateChange(personChanges, "Protection Status", oldProtectionStatus.Value, newProtectionStatus.Value);
            Rock.Attribute.Helper.SaveAttributeValue(person, person.Attributes["ProtectionStatus"], newProtectionStatus.Guid.ToString(), context);
            
            // Update Process Initiated Date
            if (clearInitiatedDate)
            {
                var oldInitiatedDate = person.GetAttributeValueAsType("ProtectionProcessInitiated") as DateTime?;
                if (oldInitiatedDate != null)
                {
                    History.EvaluateChange(personChanges, "Process Initiated", oldInitiatedDate, null);
                    Rock.Attribute.Helper.SaveAttributeValue(person, person.Attributes["ProtectionProcessInitiated"], null, context);
                }
            }

            HistoryService.SaveChanges(context, typeof(Person), Rock.SystemGuid.Category.HISTORY_PERSON_DEMOGRAPHIC_CHANGES.AsGuid(), person.Id, personChanges, true);
        }
    
        public void ExpireWorkflows()
        {
            //TODO: this is a more precise version of WorkflowExtension.Expire(...).
            //      read through all 8 protection workflows to determine which can expire (or complete).
        }

        public void CompleteRequestWorkflow()
        {
            //TODO: the request workflow will stay active until all the protection workflows are complete.
            //      this method will complete the request workflow, when everything else is in place.
        }

        /// <summary>
        /// Method to return if person has all dates expired or empty
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public static bool IsProtectionFullyExpired(RockContext rockContext, Person person)
        {
            int expiredDates = 0;

            //Get global variables
            var globalAttributes = GlobalAttributesCache.Read(rockContext);
            int appExpirationYears = Convert.ToInt32(globalAttributes.GetValue("ProtectionApplicationExpiration"));
            int bcExpirationYears = Convert.ToInt32(globalAttributes.GetValue("ProtectionBackgroundCheckExpiration"));
            int polExpirationYears = Convert.ToInt32(globalAttributes.GetValue("ProtectionPolicyAcknowledgmentExpiration"));
            int refExpirationYears = Convert.ToInt32(globalAttributes.GetValue("ProtectionReferenceExpiration"));
            int noActExpirationDays = Convert.ToInt32(globalAttributes.GetValue("ProtectionWorkflowExpirationNoActivity"));
            int actExpirationDays = Convert.ToInt32(globalAttributes.GetValue("ProtectionWorkflowExpirationSomeActivity"));

            DateTime appDate = (DateTime)(person.AttributeValues["ProtectionApplicationDate"].ValueAsType ?? new DateTime());
            DateTime bcDate = (DateTime)(person.AttributeValues["BackgroundCheckDate"].ValueAsType ?? new DateTime());
            DateTime polDate = (DateTime)(person.AttributeValues["PolicyAcknowledgmentDate"].ValueAsType ?? new DateTime());
            DateTime r1Date = (DateTime)(person.AttributeValues["ProtectionReference1Date"].ValueAsType ?? new DateTime());
            DateTime r2Date = (DateTime)(person.AttributeValues["ProtectionReference2Date"].ValueAsType ?? new DateTime());
            DateTime r3Date = (DateTime)(person.AttributeValues["ProtectionReference3Date"].ValueAsType ?? new DateTime());

            //Validate application
            if (appDate.AddYears(appExpirationYears) < DateTime.Now)
                expiredDates++;
            //Validate Background Check
            if (bcDate.AddYears(bcExpirationYears) < DateTime.Now)
                expiredDates++;
            //Validate Policy Acknowledgement
            if (polDate.AddYears(polExpirationYears) < DateTime.Now)
                expiredDates++;
            //Validate Ref1
            if (r1Date.AddYears(refExpirationYears) < DateTime.Now)
                expiredDates++;
            //Validate Ref2
            if (r2Date.AddYears(refExpirationYears) < DateTime.Now)
                expiredDates++;
            //Validate Ref3
            if (r3Date.AddYears(refExpirationYears) < DateTime.Now)
                expiredDates++;

            if (expiredDates == 6)
                return true;
            else
                return false;
        }

    }
}
