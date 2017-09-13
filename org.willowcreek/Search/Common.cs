using System;
using System.Linq;
using Rock.Data;
using Rock.Model;

namespace org.willowcreek.Search
{
    /// <summary>
    /// Class Common.
    /// </summary>
    public static class Common
    {
        public static Guid FamilyIdAttributeGuid = new Guid("EB122FAA-CE73-47F5-B56F-7DCAA4E2C42D");

        /// <summary>
        /// Finds the people by family identifier.
        /// </summary>
        /// <param name="familyId">The familyId.</param>
        /// <returns>IQueryable&lt;Person&gt;.</returns>
        public static IQueryable<Person> FindPeopleByFamilyId(string familyId)
        {
            var context = new RockContext();
            var attrService = new AttributeService(context);
            var attrId = attrService.Get(FamilyIdAttributeGuid).Id;
            var attrValues = new AttributeValueService(context).Queryable();
            var personIds = attrValues.Where(x => x.AttributeId == attrId && x.Value.Contains(familyId))
                                      .Select(x => x.EntityId);
            var people = new PersonService(context).Queryable()
                                                   .Where(x => personIds.Contains(x.Id));
            return people;
        }
    }
}
