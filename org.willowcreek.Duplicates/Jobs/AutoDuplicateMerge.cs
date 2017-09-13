using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace org.willowcreek.Duplicates.Jobs
{
    [DecimalField("Lower bound for auto-merge duplicate score", "This field will govern which dupicate records will be definitely processed based on duplicate score.", true, 100.0, "General", 0, "AutoDuplicateLowerBound")]
    [DecimalField("Lower bound for name search reconsideration", "This field will govern which dupicate records with a score less than the auto duplicate will be processed additionally based on first name alternatives.", true, 80.0, "General", 0, "NicknameLowerBound")]
    public class AutoDuplicateMerge : IJob
    {
        private const string _chronicleKey = "ChronicleEntityId";
        private const string _arenaKey = "ArenaPersonId";

        private List<string> headingKeys = new List<string> {
            "PhoneNumbers", 
            "PersonAttributes"
        };

        public void Execute(IJobExecutionContext context)
        {
            JobDataMap dataMap = context.JobDetail.JobDataMap;
            double confidenceScoreLow = dataMap.GetString("AutoDuplicateLowerBound").AsDouble();
            double nicknameScoreLow = dataMap.GetString("NicknameLowerBound").AsDouble();

            PerformAutoDuplicateMerge(confidenceScoreLow, nicknameScoreLow);
        }

        public void PerformAutoDuplicateMerge(double confidenceScoreLow, double nicknameScoreLow)
        {
            RockContext rockContext = new RockContext();

            // The altered duplicate finder should run...
            var ret = DbService.ExecuteCommand("spCrm_PersonDuplicateFinder", CommandType.StoredProcedure);

            // Get the duplicates from the database...
            var duplicates = GetDuplicatePersonFromSproc(confidenceScoreLow, nicknameScoreLow);
            var mergedCount = 0;

            // The duplicate structure contains a person and a list of duplicate possibilities for that person...
            foreach (var duplicate in duplicates)
            {
                var currRecord = duplicates.IndexOf(duplicate);
                // Before processing, make sure this is even a duplicate we want to process...some aren't...
                if (!ShouldSkipDuplicate(duplicate, rockContext))
                {
                    // In order for the family merge to happen correctly, we should pick the correct PRIMARY person...
                    var firstPerson = DetermineFirstPerson(rockContext, duplicate);

                    if (firstPerson != null)
                    {
                        // Add them to the list of data to be merged...
                        var mergeList = duplicate.Duplicates;
                        if (mergeList.Any(p => p.Id == firstPerson.Id))
                        {
                            // If the first person was one of the duplicates, add the duplicate source and reorder so that the first person is first...
                            var personService = new PersonService(rockContext);
                            var person = personService.Get(duplicate.PersonId);
                            mergeList.Add(person);
                            mergeList.Remove(firstPerson);
                            mergeList.Insert(0, firstPerson);
                        }
                        else
                        {
                            mergeList.Insert(0, firstPerson);
                        }

                        // Now spin through all the duplicates to see if any should be rejected...
                        var finalMergeList = new List<Person>();
                        for (int i = 0; i < mergeList.Count; i++)
                        {
                            // Don't skip the first one, that's the primary person, but check the others in case we need to discard them...
                            if (i == 0 || !ShouldSkipPerson(firstPerson.Id, mergeList[i].Id, rockContext))
                            {
                                finalMergeList.Add(mergeList[i]);
                            }
                        }

                        if (finalMergeList.Count >= 2)
                        {
                            // Now create the merge data and merge, unless every candidate was discarded...
                            var mergeData = new MergeData(finalMergeList, headingKeys);
                            DoMerge(mergeData, mergeList);
                            mergedCount++;
                        }
                    }
                }
            }

            ExceptionLogService.LogException(new Exception("Successfully Merged " + mergedCount + " out of " + duplicates.Count + " duplicate records."), null);
            // re-run so duplicate list is correct...
            ret = DbService.ExecuteCommand("spCrm_PersonDuplicateFinder", CommandType.StoredProcedure);
        }

        public bool ShouldSkipDuplicate(PersonDuplicateData duplicate, RockContext context)
        {
            bool skip = false;

            if (duplicate.Duplicates.Count == 1)
            {
                // If the duplicate is in the same family and has the same email, we are going to ignore it...
                skip = ShouldSkipPerson(duplicate.PersonId, duplicate.Duplicates[0].Id, context);
            }
            return skip;
        }

        public bool ShouldSkipPerson(int initialPersonId, int duplicatePersonId, RockContext context)
        {
            var familyMemberService = new GroupMemberService(context);
            var personService = new PersonService(context);

            var primary = personService.Queryable().FirstOrDefault(p => p.Id == initialPersonId);
            var duplicate = personService.Queryable().FirstOrDefault(p => p.Id == duplicatePersonId);

            // Skip record if primary and duplicate do not have the same birthdate
            if (primary != null && duplicate != null)
            {
                if (primary.BirthDate != null && duplicate.BirthDate != null && primary.BirthDate != duplicate.BirthDate)
                {
                    return true;
                }
            }
            
            // If the duplicate is in the same family and has the same email, we are going to ignore it...
            Guid familyGuid = Rock.SystemGuid.GroupType.GROUPTYPE_FAMILY.AsGuid();
            GroupMember primaryPersonFamily = null;
            GroupMember duplicateFamily = null;
            if (familyMemberService.Queryable().Any(m => m.PersonId == initialPersonId && m.Group.GroupType.Guid == familyGuid))
            {
                primaryPersonFamily = familyMemberService.Queryable().First(m => m.PersonId == initialPersonId && m.Group.GroupType.Guid == familyGuid);
            }
            if (familyMemberService.Queryable().Any(m => m.PersonId == duplicatePersonId && m.Group.GroupType.Guid == familyGuid))
            {
                duplicateFamily = familyMemberService.Queryable().First(m => m.PersonId == duplicatePersonId && m.Group.GroupType.Guid == familyGuid);
            }
            //Skip anyone with different family roles
            //(primaryPersonFamily.Group.Guid == duplicateFamily.Group.Guid) &&
            //Removed family check because role will suffice by the time the record gets here
            if ((primaryPersonFamily != null && duplicateFamily != null) && (primaryPersonFamily.GroupRoleId != duplicateFamily.GroupRoleId))
            {
                return true;
            }

            //If the two records have different marital status
            if ((duplicateFamily != null && primaryPersonFamily != null) &&
                primaryPersonFamily.Person.MaritalStatusValueId != duplicateFamily.Person.MaritalStatusValueId)
            {
                //And one is divorce/seperated/widow do not auto merge
                if ((duplicateFamily.Person.MaritalStatusValueId != null &&
                    (primaryPersonFamily.Person.MaritalStatusValueId == 895 ||
                        primaryPersonFamily.Person.MaritalStatusValueId == 896 ||
                        primaryPersonFamily.Person.MaritalStatusValueId == 897))
                    ||
                    (primaryPersonFamily.Person.MaritalStatusValueId != null && 
                    (duplicateFamily.Person.MaritalStatusValueId == 895 ||
                        duplicateFamily.Person.MaritalStatusValueId == 896 ||
                        duplicateFamily.Person.MaritalStatusValueId == 897)))
                {
                    return true;
                }
            }

            return false;
        }

        public Person DetermineFirstPerson(RockContext context, PersonDuplicateData personDuplicateData)
        {
            try
            {
                Person ret = null;

                // I guess we spin through the duplicates to see which one is the primary...the oldest record...
                var personService = new PersonService(context);
                var candidates = new List<Person>();
                var person = personService.Get(personDuplicateData.PersonId);
                if (person == null)
                {
                    throw new Exception(" Person not found with Id: " + personDuplicateData.PersonId);
                }
                candidates.Add(person);
                candidates.AddRange(personDuplicateData.Duplicates);

                // We want the oldest created person record to serve as the source... that is not 'Inactive'
                if (candidates.Any(c => c.CreatedDateTime.HasValue && c.CreatedDateTime > DateTime.MinValue))
                {
                    ret = candidates.OrderByDescending(c => c.CreatedDateTime).Last(c =>
                                    c.CreatedDateTime.HasValue && c.CreatedDateTime.Value > DateTime.MinValue && c.RecordStatusValue.Value != "Inactive");
                }
                else
                {
                    ret = candidates.First();
                }

                return ret;
            }
            catch (Exception ex)
            {
                ExceptionLogService.LogException(ex, null);
                return null;
            }
        }

        public void DoMerge(MergeData mergeData, List<Person> people)
        {
            try
            {
                bool reconfirmRequired = (mergeData.People.Select(p => p.Email).Distinct().Count() > 1 &&
                                          mergeData.People.Where(p => p.HasLogins).Any());
                GetValuesSelection(mergeData, people);

                int? primaryPersonId = null;

                var oldPhotos = new List<int>();

                var rockContext = new RockContext();

                rockContext.WrapTransaction(() =>
                {
                    var personService = new PersonService(rockContext);
                    var userLoginService = new UserLoginService(rockContext);
                    var familyService = new GroupService(rockContext);
                    var familyMemberService = new GroupMemberService(rockContext);
                    var binaryFileService = new BinaryFileService(rockContext);
                    var phoneNumberService = new PhoneNumberService(rockContext);

                    Person primaryPerson = personService.Get(mergeData.PrimaryPersonId ?? 0);
                    if (primaryPerson != null)
                    {
                        primaryPersonId = primaryPerson.Id;

                        var changes = new List<string>();

                        foreach (var p in mergeData.People.Where(p => p.Id != primaryPerson.Id))
                        {
                            changes.Add(
                                string.Format(
                                    "Merged <span class='field-value'>{0} [ID: {1}]</span> with this record.",
                                    p.FullName, p.Id));
                        }

                        // Photo Id
                        int? newPhotoId =
                            mergeData.GetSelectedValue(mergeData.GetProperty("Photo")).Value.AsIntegerOrNull();
                        if (!primaryPerson.PhotoId.Equals(newPhotoId))
                        {
                            changes.Add("Modified the photo.");
                            primaryPerson.PhotoId = newPhotoId;
                        }

                        primaryPerson.TitleValueId = GetNewIntValue(mergeData, "Title", changes);
                        primaryPerson.FirstName = GetNewStringValue(mergeData, "FirstName", changes);
                        primaryPerson.NickName = GetNewStringValue(mergeData, "NickName", changes);
                        primaryPerson.MiddleName = GetNewStringValue(mergeData, "MiddleName", changes);
                        primaryPerson.LastName = GetNewStringValue(mergeData, "LastName", changes);
                        primaryPerson.SuffixValueId = GetNewIntValue(mergeData, "Suffix", changes);
                        primaryPerson.RecordTypeValueId = GetNewIntValue(mergeData, "RecordType", changes);
                        primaryPerson.RecordStatusValueId = GetNewIntValue(mergeData, "RecordStatus", changes);
                        primaryPerson.RecordStatusReasonValueId = GetNewIntValue(mergeData, "RecordStatusReason",
                            changes);
                        primaryPerson.ConnectionStatusValueId = GetNewIntValue(mergeData, "ConnectionStatus", changes);
                        primaryPerson.IsDeceased = GetNewBoolValue(mergeData, "Deceased", changes).Value;
                        primaryPerson.Gender = (Gender) GetNewEnumValue(mergeData, "Gender", typeof (Gender), changes);
                        primaryPerson.MaritalStatusValueId = GetNewIntValue(mergeData, "MaritalStatus", changes);
                        primaryPerson.SetBirthDate(GetNewDateTimeValue(mergeData, "BirthDate", changes));
                        primaryPerson.AnniversaryDate = GetNewDateTimeValue(mergeData, "AnniversaryDate", changes);
                        primaryPerson.GraduationYear = GetNewIntValue(mergeData, "GraduationYear", changes);
                        primaryPerson.Email = GetNewStringValue(mergeData, "Email", changes);
                        primaryPerson.IsEmailActive = GetNewBoolValue(mergeData, "EmailActive", changes).Value;
                        primaryPerson.EmailNote = GetNewStringValue(mergeData, "EmailNote", changes);
                        primaryPerson.EmailPreference =
                            (EmailPreference)
                                GetNewEnumValue(mergeData, "EmailPreference", typeof (EmailPreference), changes);
                        primaryPerson.InactiveReasonNote = GetNewStringValue(mergeData, "InactiveReasonNote", changes);
                        primaryPerson.SystemNote = GetNewStringValue(mergeData, "SystemNote", changes);

                        // Update phone numbers
                        var phoneTypes =
                            DefinedTypeCache.Read(Rock.SystemGuid.DefinedType.PERSON_PHONE_TYPE.AsGuid()).DefinedValues;
                        foreach (var phoneType in phoneTypes)
                        {
                            var phoneNumber =
                                primaryPerson.PhoneNumbers.Where(p => p.NumberTypeValueId == phoneType.Id)
                                    .FirstOrDefault();
                            string oldValue = phoneNumber != null ? phoneNumber.Number : string.Empty;

                            string key = "phone_" + phoneType.Id.ToString();
                            string newValue = GetNewStringValue(mergeData, key, changes);

                            if (!oldValue.Equals(newValue, StringComparison.OrdinalIgnoreCase))
                            {
                                // New phone doesn't match old

                                if (!string.IsNullOrWhiteSpace(newValue))
                                {
                                    // New value exists
                                    if (phoneNumber == null)
                                    {
                                        // Old value didn't exist... create new phone record
                                        phoneNumber = new PhoneNumber {NumberTypeValueId = phoneType.Id};
                                        primaryPerson.PhoneNumbers.Add(phoneNumber);
                                    }

                                    // Update phone number
                                    phoneNumber.Number = newValue;
                                }
                                else
                                {
                                    // New value doesn't exist
                                    if (phoneNumber != null)
                                    {
                                        // old value existed.. delete it
                                        primaryPerson.PhoneNumbers.Remove(phoneNumber);
                                        phoneNumberService.Delete(phoneNumber);
                                    }
                                }
                            }
                        }

                        // Save the new record
                        rockContext.SaveChanges();

                        // Update the attributes
                        primaryPerson.LoadAttributes(rockContext);
                        var attributeValueService = new AttributeValueService(rockContext);
                        foreach (var property in mergeData.Properties.Where(p => p.Key.StartsWith("attr_")))
                        {
                            string attributeKey = property.Key.Substring(5);
                            string oldValue = primaryPerson.GetAttributeValue(attributeKey) ?? string.Empty;
                            string newValue = GetNewStringValue(mergeData, property.Key, changes) ?? string.Empty;

                            if (!oldValue.Equals(newValue))
                            {
                                var attribute = primaryPerson.Attributes[attributeKey];
                                Rock.Attribute.Helper.SaveAttributeValue(primaryPerson, attribute, newValue, rockContext);
                            }
                        }

                        HistoryService.SaveChanges(rockContext, typeof (Person),
                            Rock.SystemGuid.Category.HISTORY_PERSON_DEMOGRAPHIC_CHANGES.AsGuid(),
                            primaryPerson.Id, changes);

                        // Delete the unselected photos
                        string photoKeeper = primaryPerson.PhotoId.HasValue
                            ? primaryPerson.PhotoId.Value.ToString()
                            : string.Empty;
                        foreach (var photoValue in mergeData.Properties
                            .Where(p => p.Key == "Photo")
                            .SelectMany(p => p.Values)
                            .Where(v => v.Value != "" && v.Value != photoKeeper)
                            .Select(v => v.Value))
                        {
                            int photoId = 0;
                            if (int.TryParse(photoValue, out photoId))
                            {
                                var photo = binaryFileService.Get(photoId);
                                if (photo != null)
                                {
                                    string errorMessages;
                                    if (binaryFileService.CanDelete(photo, out errorMessages))
                                    {
                                        binaryFileService.Delete(photo);
                                    }
                                }
                            }
                        }
                        rockContext.SaveChanges();

                        // Delete merged person's family records and any families that would be empty after merge
                        foreach (var p in mergeData.People.Where(p => p.Id != primaryPersonId.Value))
                        {
                            // Delete the merged person's phone numbers (we've already updated the primary persons values)
                            foreach (var phoneNumber in phoneNumberService.GetByPersonId(p.Id))
                            {
                                phoneNumberService.Delete(phoneNumber);
                            }

                            if (reconfirmRequired)
                            {
                                foreach (var login in userLoginService.GetByPersonId(p.Id))
                                {
                                    login.IsConfirmed = false;
                                }
                            }

                            rockContext.SaveChanges();

                            // Delete the merged person's other family member records and the family if they were the only one in the family
                            var familyGuid = Rock.SystemGuid.GroupType.GROUPTYPE_FAMILY.AsGuid();
                            var familymembersfromquery =
                                familyMemberService.Queryable()
                                    .Where(m => m.PersonId == p.Id && m.Group.GroupType.Guid == familyGuid)
                                    .ToList();
                            foreach (var familyMember in familymembersfromquery)
                            {
                                var oldFamilyName = familyMember.Group.Name;
                                var roleGuid = familyMember.GroupRole.Guid;
                                var primaryPersonFamilies = familyMemberService.Queryable().Where(m => m.PersonId == primaryPerson.Id && m.Group.GroupType.Guid == familyGuid).ToList();

                                //before deleting the familymember lets make sure the new family has the same last name
                                var sameFamily = (oldFamilyName.ToUpper().Replace(" ", "").Replace("'", "") == primaryPersonFamilies[0].Group.Name.ToUpper().Replace(" ", "").Replace("'", ""));
                                if (sameFamily)
                                {
                                    familyMemberService.Delete(familyMember);
                                    rockContext.SaveChanges();
                                }

                                // Get the family
                                var family = familyService.Queryable("Members").FirstOrDefault(f => f.Id == familyMember.GroupId);

                                if (!family.Members.Any())
                                {
                                    // If there are not any other family members, delete the family record.
                                    DeleteFamily(family, primaryPersonId.Value, rockContext, personService,
                                        familyService);
                                }
                                //If primary is in more than 1 family, skip changing family members
                                else if (primaryPersonFamilies.Count() <= 1 && sameFamily)
                                {
                                    // If there are family members left over, we need to check to see that there is at least one adult left in the family,
                                    // otherwise we should move over children to the family of the merged person...
//                                if (!family.Members.Any(m => m.GroupRole.Guid == Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT.AsGuid()))

                                    //Don't care if there is an adult left, bring over anyone not existing in new family if this duplicate is an adult
                                    if (roleGuid != Rock.SystemGuid.GroupRole.GROUPROLE_FAMILY_MEMBER_ADULT.AsGuid())
                                    {
                                        continue;
                                    }
                                    var familyList = family.Members.ToList();
                                    foreach (var otherFamilyMember in familyList)
                                    {
                                        //Should be firstordefault
                                        var primaryPersonFamily =
                                            familyMemberService.Queryable()
                                                .Single(
                                                    m =>
                                                        m.PersonId == primaryPerson.Id &&
                                                        m.Group.GroupType.Guid == familyGuid);
                                        var primaryPersonFamilyId = primaryPersonFamily.GroupId;
                                        var existsInNew = false;

                                        foreach (var familyMemberNew in familyMemberService.Queryable().Where(
                                            m =>
                                                m.GroupId == primaryPersonFamilyId &&
                                                m.Group.GroupType.Guid == familyGuid &&
                                                m.PersonId != primaryPerson.Id))
                                        {
                                            //If the same first name and both are children
                                            if ((familyMemberNew.Person.FirstName == otherFamilyMember.Person.FirstName
                                                 ||
                                                 familyMemberNew.Person.FirstName == otherFamilyMember.Person.NickName
                                                 ||
                                                 familyMemberNew.Person.NickName == otherFamilyMember.Person.FirstName)
                                                && familyMemberNew.GroupRoleId == otherFamilyMember.GroupRoleId)
                                            {
                                                existsInNew = true;
                                            }
                                        }

                                        //If they don't exist in new family
                                        if (!existsInNew)
                                        {
                                            // Move them to the new family...
                                            GroupMember newFamilyMember = new GroupMember();
                                            newFamilyMember.Person = otherFamilyMember.Person;
                                            newFamilyMember.GroupId = primaryPersonFamilyId;
                                            newFamilyMember.GroupRoleId = otherFamilyMember.GroupRoleId;
                                            familyMemberService.Add(newFamilyMember);

                                            // Now remove them from the old family...
                                            familyMemberService.Delete(otherFamilyMember);
                                        }
                                    }
                                    rockContext.SaveChanges();

                                    family =
                                        familyService.Queryable("Members")
                                            .FirstOrDefault(f => f.Id == familyMember.GroupId);
                                    //If noone is left in the family
                                    if (!family.Members.Any())
                                    {
                                        //Delete the family
                                        DeleteFamily(family, primaryPersonId.Value, rockContext, personService,
                                            familyService);
                                    }
                                }
                            }
                        }
                    }
                });
                foreach (var p in mergeData.People.Where(p => p.Id != mergeData.PrimaryPersonId))
                {
                    // Run merge proc to merge all associated data
                    var parms = new Dictionary<string, object>();
                    parms.Add("OldId", p.Id);
                    parms.Add("NewId", mergeData.PrimaryPersonId);
                    DbService.ExecuteCommand("spCrm_PersonMerge", CommandType.StoredProcedure, parms);
                }
            }
            catch (Exception ex)
            {
                var newEx = new Exception(ex.Message + " PersonId: " + mergeData.PrimaryPersonId);
                ExceptionLogService.LogException(newEx, null);
            }
        }

        public void DeleteFamily(Group family, int primaryPersonId, RockContext rockContext, PersonService personService, GroupService familyService)
        {
            var oldFamilyChanges = new List<string>();
            History.EvaluateChange(oldFamilyChanges, "Family", family.Name, string.Empty);
            HistoryService.SaveChanges(rockContext, typeof(Person), Rock.SystemGuid.Category.HISTORY_PERSON_FAMILY_CHANGES.AsGuid(),
                primaryPersonId, oldFamilyChanges, family.Name, typeof(Group), family.Id);

            // If theres any people that have this group as a giving group, set it to null (the person being merged should be the only one)
            foreach (Person gp in personService.Queryable().Where(g => g.GivingGroupId == family.Id))
            {
                gp.GivingGroupId = null;
            }

            // Delete the family
            familyService.Delete(family);

            rockContext.SaveChanges();
        }

        public List<PersonDuplicateData> GetDuplicatePersonFromSproc(double confidenceScoreLow, double nicknameScoreLow)
        {
            RockContext rockContext = new RockContext();
            var personDuplicateService = new PersonDuplicateService(rockContext);
            int recordStatusInactiveId = DefinedValueCache.Read(Rock.SystemGuid.DefinedValue.PERSON_RECORD_STATUS_INACTIVE.AsGuid()).Id;

            // list duplicates that aren't confirmed as NotDuplicate and aren't IgnoreUntilScoreChanges. Also, don't include records where both the Person and Duplicate are inactive
            var personDuplicateQry = personDuplicateService.Queryable()
                .Where(a => !a.IsConfirmedAsNotDuplicate)
                .Where(a => !a.IgnoreUntilScoreChanges)
                .Where(a => !(a.PersonAlias.Person.RecordStatusValueId == recordStatusInactiveId && a.DuplicatePersonAlias.Person.RecordStatusValueId == recordStatusInactiveId));

            // Apply both, even though we really only need the lowest...
            personDuplicateQry = personDuplicateQry.Where(a => a.ConfidenceScore >= nicknameScoreLow);

            // Get the data from the database...
            List<PersonDuplicate> databaseDuplicates = personDuplicateQry.ToList();

            //Remove anyone under confidenceScoreLow
            //var notDuplicates = databaseDuplicates.Where(a => (a.ConfidenceScore <= confidenceScoreLow)
            //    //and not in a list of people that contain
            //    && !((
            //            (a.PersonAlias.Person.FirstName.ToUpper() == a.DuplicatePersonAlias.Person.FirstName.ToUpper()
            //            || a.PersonAlias.Person.FirstName.ToUpper() == a.DuplicatePersonAlias.Person.NickName.ToUpper()
            //            || a.PersonAlias.Person.NickName.ToUpper() == a.DuplicatePersonAlias.Person.FirstName.ToUpper())
            //            //similar first name or first/nickname match
            //            || (NameSearch.NameSearch.AreTheSameName(a.PersonAlias.Person.FirstName, a.DuplicatePersonAlias.Person.FirstName)
            //                || NameSearch.NameSearch.AreTheSameName(a.PersonAlias.Person.FirstName, a.DuplicatePersonAlias.Person.NickName)
            //                || NameSearch.NameSearch.AreTheSameName(a.PersonAlias.Person.NickName, a.DuplicatePersonAlias.Person.FirstName))
            //            )
            //        //and same birthdates if not null
            //        && ((
            //                (a.PersonAlias.Person.BirthDate != null && a.DuplicatePersonAlias.Person.BirthDate != null) 
            //                && 
            //                (a.PersonAlias.Person.BirthDate == a.DuplicatePersonAlias.Person.BirthDate)
            //            )
            //            // or if a null birthday and scored by phone email or address
            //            || 
            //            (
            //                (a.PersonAlias.Person.BirthDate == null || a.DuplicatePersonAlias.Person.BirthDate == null) 
            //                && 
            //                (a.ScoreDetail.Contains("Phone")
            //                || a.ScoreDetail.Contains("Email")
            //                || a.ScoreDetail.Contains("Address")
            //                )
            //            ))
            //        )).ToList();
            //var trueNotDups = notDuplicates;
            //foreach (var a in notDuplicates)
            //{
            //    if ((a.PersonAlias.Person.FirstName.ToUpper() == a.DuplicatePersonAlias.Person.FirstName.ToUpper()
            //         || a.PersonAlias.Person.FirstName.ToUpper() == a.DuplicatePersonAlias.Person.NickName.ToUpper()
            //         || a.PersonAlias.Person.NickName.ToUpper() == a.DuplicatePersonAlias.Person.FirstName.ToUpper())
            //        //similar first name or first/nickname match
            //        ||
            //        (NameSearch.NameSearch.AreTheSameName(a.PersonAlias.Person.FirstName,
            //            a.DuplicatePersonAlias.Person.FirstName)
            //         ||
            //         NameSearch.NameSearch.AreTheSameName(a.PersonAlias.Person.FirstName,
            //             a.DuplicatePersonAlias.Person.NickName)
            //         ||
            //         NameSearch.NameSearch.AreTheSameName(a.PersonAlias.Person.NickName,
            //             a.DuplicatePersonAlias.Person.FirstName))
            //        )
            //    {
            //        if (((a.PersonAlias.Person.BirthDate != null && a.DuplicatePersonAlias.Person.BirthDate != null)
            //             &&
            //             (a.PersonAlias.Person.BirthDate == a.DuplicatePersonAlias.Person.BirthDate)
            //            )
            //            // or if a null birthday and scored by phone email or address
            //            ||
            //            (
            //                (a.PersonAlias.Person.BirthDate == null || a.DuplicatePersonAlias.Person.BirthDate == null)
            //                &&
            //                (a.ScoreDetail.Contains("Phone")
            //                 || a.ScoreDetail.Contains("Email")
            //                 || a.ScoreDetail.Contains("Address")
            //                    )
            //                ))
            //        {
            //            var that = "this";
            //        }
            //    }
            //}


            //Remove the confirmed not duplicates from the screen
            //foreach (var duplicate in notDuplicates)
            //{
            //    duplicate.IsConfirmedAsNotDuplicate = true;
            //}

            // Now filter the results, removing items in the range between nickname and the lower bound whose names are not equal...
            databaseDuplicates.RemoveAll(a => (a.ConfidenceScore <= confidenceScoreLow)
                //and not in a list of people that contain
                && !((
                            (a.PersonAlias.Person.FirstName.ToUpper() == a.DuplicatePersonAlias.Person.FirstName.ToUpper()
                            || a.PersonAlias.Person.FirstName.ToUpper() == a.DuplicatePersonAlias.Person.NickName.ToUpper()
                            || a.PersonAlias.Person.NickName.ToUpper() == a.DuplicatePersonAlias.Person.FirstName.ToUpper())
                //similar first name or first/nickname match
                        || (NameSearch.NameSearch.AreTheSameName(a.PersonAlias.Person.FirstName, a.DuplicatePersonAlias.Person.FirstName)
                            || NameSearch.NameSearch.AreTheSameName(a.PersonAlias.Person.FirstName, a.DuplicatePersonAlias.Person.NickName)
                            || NameSearch.NameSearch.AreTheSameName(a.PersonAlias.Person.NickName, a.DuplicatePersonAlias.Person.FirstName))
                        )
                //and same birthdates if not null
                    && ((
                            (a.PersonAlias.Person.BirthDate != null && a.DuplicatePersonAlias.Person.BirthDate != null)
                            &&
                            (a.PersonAlias.Person.BirthDate == a.DuplicatePersonAlias.Person.BirthDate)
                        )
                // or if a null birthday and scored by phone email or address
                        ||
                        (
                            (a.PersonAlias.Person.BirthDate == null || a.DuplicatePersonAlias.Person.BirthDate == null)
                            &&
                            (a.ScoreDetail.Contains("Phone")
                            || a.ScoreDetail.Contains("Email")
                            || a.ScoreDetail.Contains("Address")
                            )
                        ))
                    ));

            var groupByQry = databaseDuplicates.GroupBy(a => a.PersonAlias.Person, a => a.DuplicatePersonAlias.Person);

            var qry = groupByQry.Select(a => new PersonDuplicateData
            {
                PersonId = a.Key.Id,
                LastName = a.Key.LastName,
                FirstName = a.Key.FirstName,
                MatchCount = a.Count(),
//                PersonModifiedDateTime = a.Key.ModifiedDateTime,
//                CreatedByPerson = a.Key.CreatedByPersonAlias.Person.FirstName + " " + a.Key.CreatedByPersonAlias.Person.LastName,
                Duplicates = a.ToList()
            });

            return qry.ToList();
        }

        public class PersonDuplicateData
        {
            public int PersonId { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public int MatchCount { get; set; }
            public DateTime? PersonModifiedDateTime { get; set; }
            public string CreatedByPerson { get; set; }
            public List<Person> Duplicates { get; set; }
        }

        #region MergeData Class

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class MergeData
        {

            #region Properties

            /// <summary>
            /// Gets or sets the people.
            /// </summary>
            /// <value>
            /// The people.
            /// </value>
            public List<MergePerson> People { get; set; }

            /// <summary>
            /// Gets or sets the properties.
            /// </summary>
            /// <value>
            /// The properties.
            /// </value>
            public List<PersonProperty> Properties { get; set; }

            /// <summary>
            /// Gets or sets the primary person identifier.
            /// </summary>
            /// <value>
            /// The primary person identifier.
            /// </value>
            public int? PrimaryPersonId { get; set; }

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="MergeData"/> class.
            /// </summary>
            public MergeData()
            {
                People = new List<MergePerson>();
                Properties = new List<PersonProperty>();
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="MergeData"/> class.
            /// </summary>
            /// <param name="people">The people.</param>
            public MergeData(List<Person> people, List<string> headingKeys) : this()
            {
                var phoneTypes = DefinedTypeCache.Read(Rock.SystemGuid.DefinedType.PERSON_PHONE_TYPE.AsGuid()).DefinedValues;
                foreach (var person in people)
                {
                    AddPerson(person);
                }

                foreach (var person in people)
                {
                    AddProperty("PhoneNumbers", "Phone Numbers", 0, string.Empty);

                    foreach (var phoneType in phoneTypes)
                    {
                        string key = "phone_" + phoneType.Id.ToString();
                        var phoneNumber = person.PhoneNumbers.Where(p => p.NumberTypeValueId == phoneType.Id).FirstOrDefault();
                        if (phoneNumber != null)
                        {
                            AddProperty(key, phoneType.Value, person.Id, phoneNumber.Number, phoneNumber.ToString());
                        }
                        else
                        {
                            AddProperty(key, phoneType.Value, person.Id, string.Empty, string.Empty);
                        }
                    }
                }

                foreach (var person in people)
                {
                    AddProperty("PersonAttributes", "Person Attributes", 0, string.Empty);
                    person.LoadAttributes();
                    foreach (var attribute in person.Attributes.OrderBy(a => a.Value.Order))
                    {
                        string value = person.GetAttributeValue(attribute.Key);
                        //string formattedValue = attribute.Value.FieldType.Field.FormatValue(null, value, attribute.Value.QualifierValues, false);
                        AddProperty("attr_" + attribute.Key, attribute.Value.Name, person.Id, value, value);
                    }
                }

                // Add missing values
                foreach (var property in Properties.Where(p => !headingKeys.Contains(p.Key)))
                {
                    foreach (var person in People.Where(p => !property.Values.Any(v => v.PersonId == p.Id)))
                    {
                        property.Values.Add(new PersonPropertyValue() { PersonId = person.Id });
                    }
                }

                // The primary person should have been set on the class after evaluating WC attributes...
                SetPrimary(people.First().Id);

            }

            #endregion

            #region Methods

            #region Public Methods

            /// <summary>
            /// Sets the primary.  Notice that this method will get the primary value for a property, deselect all the other values,
            /// and select the one that mattered.  Hence, if the primary is null, but a dupe record contains a value, it will use the 
            /// dupe value.
            /// </summary>
            /// <param name="primaryPersonId">The primary person identifier.</param>
            public void SetPrimary(int primaryPersonId)
            {
                PrimaryPersonId = primaryPersonId;

                foreach (var personProperty in Properties)
                {
                    PersonPropertyValue value = null;

                    if (personProperty.Values.Any(v => v.Value != null && v.Value != ""))
                    {
                        // Find primary person's non-blank value
                        value = personProperty.Values.Where(v => v.PersonId == primaryPersonId && v.Value != null && v.Value != "").FirstOrDefault();

                        // Get newest modified data
                        var newestRecordPersonId = primaryPersonId;
                        DateTime? newestRecordTime = DateTime.MinValue;
                        var attributeValueService = new AttributeValueService(new RockContext());
                        var personService = new PersonService(new RockContext());
                        foreach (var currVal in personProperty.Values)
                        {
                            var thisKey = personProperty.Key;
                            if (thisKey.StartsWith("attr_"))
                            {
                                thisKey = thisKey.Substring(5);                                
                            }
                            var thisRecord = attributeValueService.Queryable().FirstOrDefault(v => v.EntityId == currVal.PersonId && v.Attribute.Key == thisKey);
                            if (thisRecord != null)
                            {
                                DateTime? thisRecordTime;
                                //Keep created date as oldest record
                                if (thisKey == "CreatedDate")
                                {
                                    thisRecordTime = thisRecord.ValueAsDateTime;
                                    if (newestRecordTime == DateTime.MinValue)
                                    {
                                        newestRecordTime = thisRecordTime;
                                    }
                                    else if (newestRecordTime > thisRecordTime)
                                    {
                                        newestRecordTime = thisRecordTime;
                                        newestRecordPersonId = currVal.PersonId;
                                    }
                                }
                                else
                                {
                                    thisRecordTime = thisRecord.ModifiedDateTime ?? thisRecord.CreatedDateTime;
                                    if (newestRecordTime < thisRecordTime)
                                    {
                                        newestRecordTime = thisRecordTime;
                                        newestRecordPersonId = currVal.PersonId;
                                    }
                                }
                            }
                            else
                            {
                                var thisPerson = personService.Queryable().FirstOrDefault(p => p.Id == currVal.PersonId);
                                if (thisPerson != null)
                                {
                                    var thisRecordTime = thisPerson.ModifiedDateTime ?? thisPerson.CreatedDateTime;
                                    if (newestRecordTime < thisRecordTime)
                                    {
                                        newestRecordTime = thisRecordTime;
                                        newestRecordPersonId = currVal.PersonId;
                                    }
                                }
                            }
                        }

                        //If Connection Status make sure to keep highest status
                        var isConnStatus = false;
                        if (personProperty.Key == "ConnectionStatus")
                        {
                            //Member, Attendee, Visitor
                            var newValue = personProperty.Values.FirstOrDefault(v => v.FormattedValue == "Member") ??
                                           (personProperty.Values.FirstOrDefault(v => v.FormattedValue == "Attendee") ??
                                                                                                              personProperty.Values.FirstOrDefault(v => v.FormattedValue == "Visitor"));
                            if (newValue != null)
                            {
                                value = newValue;
                                isConnStatus = true;
                            }
                        }

                        //If no value or not the newest value and not connection status
                        if (value == null || (primaryPersonId != newestRecordPersonId && !isConnStatus))
                        {
                            //Find newest value
                            value = personProperty.Values.FirstOrDefault(v => v.PersonId == newestRecordPersonId && v.Value != null && v.Value != "");
                            if (value == null)
                            {
                                // Find any other selected value
                                value = personProperty.Values.FirstOrDefault(v => v.Selected);
                                if (value == null)
                                {
                                    // Find first non-blank value
                                    value = personProperty.Values.FirstOrDefault(v => v.Value != "");
                                    if (value == null)
                                    {
                                        value = personProperty.Values.FirstOrDefault();
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        value = personProperty.Values.Where(v => v.PersonId == primaryPersonId).FirstOrDefault();
                    }

                    // Unselect all the values
                    personProperty.Values.ForEach(v => v.Selected = false);

                    if (value != null)
                    {
                        value.Selected = true;
                    }
                }
            }

            /// <summary>
            /// Gets the selected value.
            /// </summary>
            /// <param name="personProperty">The person property.</param>
            /// <returns></returns>
            public PersonPropertyValue GetSelectedValue(PersonProperty personProperty)
            {
                if (personProperty != null)
                {
                    return personProperty.Values.Where(v => v.Selected).FirstOrDefault();
                }
                return null;
            }

            #endregion

            #region Private Methods

            private void AddPerson(Person person)
            {
                People.Add(new MergePerson(person));

                AddProperty("Photo", "Photo", person.Id, person.PhotoId.HasValue ? person.PhotoId.ToString() : string.Empty);
                AddProperty("Title", person.Id, person.TitleValue);
                AddProperty("FirstName", person.Id, person.FirstName);
                AddProperty("NickName", person.Id, person.NickName);
                AddProperty("MiddleName", person.Id, person.MiddleName);
                AddProperty("LastName", person.Id, person.LastName);
                AddProperty("Suffix", person.Id, person.SuffixValue);
                AddProperty("RecordType", person.Id, person.RecordTypeValue);
                AddProperty("RecordStatus", person.Id, person.RecordStatusValue);
                AddProperty("RecordStatusReason", person.Id, person.RecordStatusReasonValue);
                AddProperty("ConnectionStatus", person.Id, person.ConnectionStatusValue);
                AddProperty("Deceased", person.Id, person.IsDeceased);
                AddProperty("Gender", person.Id, person.Gender);
                AddProperty("MaritalStatus", person.Id, person.MaritalStatusValue);
                AddProperty("BirthDate", person.Id, person.BirthDate);
                AddProperty("AnniversaryDate", person.Id, person.AnniversaryDate);
                AddProperty("GraduationYear", person.Id, person.GraduationYear.HasValue ? person.GraduationYear.ToString() : string.Empty);
                AddProperty("Email", person.Id, person.Email);
                AddProperty("EmailActive", person.Id, person.IsEmailActive);
                AddProperty("EmailNote", person.Id, person.EmailNote);
                AddProperty("EmailPreference", person.Id, person.EmailPreference);
                AddProperty("InactiveReasonNote", person.Id, person.InactiveReasonNote);
                AddProperty("SystemNote", person.Id, person.SystemNote);
            }

            private void AddProperty(string key, int personId, string value, bool selected = false)
            {
                AddProperty(key, key.SplitCase(), personId, value, value, selected);
            }

            private void AddProperty(string key, string label, int personId, string value, bool selected = false)
            {
                AddProperty(key, label, personId, value, value, selected);
            }

            private void AddProperty(string key, string label, int personId, string value, string formattedValue, bool selected = false)
            {
                var property = GetProperty(key, true, label);
                var propertyValue = property.Values.Where(v => v.PersonId == personId).FirstOrDefault();
                if (propertyValue == null)
                {
                    propertyValue = new PersonPropertyValue { PersonId = personId };
                    property.Values.Add(propertyValue);
                }
                propertyValue.Value = value ?? string.Empty;
                propertyValue.FormattedValue = formattedValue ?? string.Empty;
                propertyValue.Selected = selected;
            }

            private void AddProperty(string key, int personId, DefinedValue value, bool selected = false)
            {
                AddProperty(key, key.SplitCase(), personId, value, selected);
            }

            private void AddProperty(string key, string label, int personId, DefinedValue value, bool selected = false)
            {
                var property = GetProperty(key, true, label);
                var propertyValue = property.Values.Where(v => v.PersonId == personId).FirstOrDefault();
                if (propertyValue == null)
                {
                    propertyValue = new PersonPropertyValue { PersonId = personId };
                    property.Values.Add(propertyValue);
                }
                propertyValue.Value = value != null ? value.Id.ToString() : string.Empty;
                propertyValue.FormattedValue = value != null ? value.Value : string.Empty;
                propertyValue.Selected = selected;
            }

            private void AddProperty(string key, int personId, bool? value, bool selected = false)
            {
                AddProperty(key, personId, (value ?? false).ToString(), selected);
            }

            private void AddProperty(string key, int personId, DateTime? value, bool selected = false)
            {
                AddProperty(key, personId, value.HasValue ? value.Value.ToShortDateString() : string.Empty, selected);
            }

            private void AddProperty(string key, int personId, Enum value, bool selected = false)
            {
                AddProperty(key, key.SplitCase(), personId, value.ConvertToString(false), value.ConvertToString(), selected);
            }

            public PersonProperty GetProperty(string key, bool createIfNotFound = false, string label = "")
            {
                var property = Properties.Where(p => p.Key.Equals(key, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (property == null && createIfNotFound)
                {
                    if (label == string.Empty)
                    {
                        label = key.SplitCase();
                    }

                    property = new PersonProperty(key, label);
                    Properties.Add(property);
                }

                return property;
            }

            #endregion

            #endregion

        }

        #endregion

        #region MergePerson Class

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class MergePerson
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public DateTime? ModifiedDateTime { get; set; }
            public string ModifiedBy { get; set; }
            public string Email { get; set; }
            public bool HasLogins { get; set; }

            public MergePerson(Person person)
            {
                Id = person.Id;
                FullName = person.FullName;
                ModifiedDateTime = person.ModifiedDateTime;
                Email = person.Email;
                HasLogins = person.Users.Any();

                if (person.ModifiedByPersonAlias != null &&
                    person.ModifiedByPersonAlias.Person != null)
                {
                    ModifiedBy = person.ModifiedByPersonAlias.Person.FullName;
                }
            }
        }

        #endregion

        #region PersonProperty Class

        /// <summary>
        /// 
        /// </summary>
        [Serializable]
        public class PersonProperty
        {
            public string Key { get; set; }
            public string Label { get; set; }
            public List<PersonPropertyValue> Values { get; set; }

            public PersonProperty()
            {
                Values = new List<PersonPropertyValue>();
            }

            public PersonProperty(string key)
                : this()
            {
                Key = key;
                Label = key.SplitCase();
            }

            public PersonProperty(string key, string label)
                : this()
            {
                Key = key;
                Label = label;
            }
        }

        #endregion

        #region PersonPropertyValue class

        [Serializable]
        public class PersonPropertyValue
        {
            public int PersonId { get; set; }
            public bool Selected { get; set; }
            public string Value { get; set; }
            public string FormattedValue { get; set; }
        }

        #endregion

        #region MergeData support methods (could be moved to methods on MergeData, methinks)
        /// <summary>
        /// Gets the values selection.
        /// </summary>
        private void GetValuesSelection(MergeData mergeData, List<Person> people)
        {
            foreach (var person in people)
            {
                // Get person id from the datafield that has format 'property_{0}' with {0} being the person id
                int personId = person.Id;

                // Set the correct person's value as selected
                foreach (var property in mergeData.Properties.Where(p => p.Key == personId.ToString()))
                {
                    foreach (var personValue in property.Values)
                    {
                        personValue.Selected = personValue.PersonId == personId;
                    }
                }
            }
        }

        private string GetNewStringValue(MergeData mergeData, string key, List<string> changes)
        {
            var ppValue = GetNewValue(mergeData, key, changes);
            return ppValue != null ? ppValue.Value : string.Empty;
        }

        private int? GetNewIntValue(MergeData mergeData, string key, List<string> changes)
        {
            var ppValue = GetNewValue(mergeData, key, changes);
            if (ppValue != null)
            {
                int newValue = int.MinValue;
                if (int.TryParse(ppValue.Value, out newValue))
                {
                    return newValue;
                }
            }

            return null;
        }

        private bool? GetNewBoolValue(MergeData mergeData, string key, List<string> changes)
        {
            var ppValue = GetNewValue(mergeData, key, changes);
            if (ppValue != null)
            {
                bool newValue = false;
                if (bool.TryParse(ppValue.Value, out newValue))
                {
                    return newValue;
                }
            }

            return null;
        }

        private DateTime? GetNewDateTimeValue(MergeData mergeData, string key, List<string> changes)
        {
            var ppValue = GetNewValue(mergeData, key, changes);
            if (ppValue != null)
            {
                DateTime newValue = DateTime.MinValue;
                if (DateTime.TryParse(ppValue.Value, out newValue))
                {
                    return newValue;
                }
            }

            return null;
        }

        private Enum GetNewEnumValue(MergeData mergeData, string key, Type enumType, List<string> changes)
        {
            var ppValue = GetNewValue(mergeData, key, changes);
            if (ppValue != null)
            {
                return (Enum)Enum.Parse(enumType, ppValue.Value);
            }

            return null;
        }

        private PersonPropertyValue GetNewValue(MergeData mergeData, string key, List<string> changes)
        {
            var property = mergeData.GetProperty(key);
            var primaryPersonValue = property.Values.Where(v => v.PersonId == mergeData.PrimaryPersonId).FirstOrDefault();
            var selectedPersonValue = property.Values.Where(v => v.Selected).FirstOrDefault();

            string oldValue = primaryPersonValue != null ? primaryPersonValue.Value : string.Empty;
            string newValue = selectedPersonValue != null ? selectedPersonValue.Value : string.Empty;

            if (oldValue != newValue)
            {
                string oldFormattedValue = primaryPersonValue != null ? primaryPersonValue.FormattedValue : string.Empty;
                string newFormattedValue = selectedPersonValue != null ? selectedPersonValue.FormattedValue : string.Empty;
                History.EvaluateChange(changes, property.Label, oldFormattedValue, newFormattedValue);
            }

            return selectedPersonValue;
        }
        #endregion
    }
}
