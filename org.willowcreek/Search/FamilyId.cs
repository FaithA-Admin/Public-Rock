using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using Rock;
using Rock.Attribute;
using Rock.Search;

namespace org.willowcreek.Search
{
    [Description("Family Id Search (Willow)")]
    [Export(typeof(SearchComponent))]
    [ExportMetadata("ComponentName", "Family Id")]
    [BooleanField("Quick List Enabled", "Enable results while typing in the search box", false, "", 4, "QuickListEnabled")]
    public class FamilyId : SearchComponent
    {
        /// <summary>
        /// Gets the attribute value defaults.
        /// </summary>
        /// <value>
        /// The attribute defaults.
        /// </value>
        public override Dictionary<string, string> AttributeValueDefaults
        {
            get
            {
                var defaults = new Dictionary<string, string>();
                defaults.Add("SearchLabel", "Family Id");
                return defaults;
            }
        }

        /// <summary>
        /// Returns a list of value/label results matching the searchterm
        /// </summary>
        /// <param name="searchterm">The searchterm.</param>
        /// <returns></returns>
        /// <remarks>
        /// This method is only called via a api\SearchController Call (e.g. when a term is typed into search box),
        /// it does not get run on non-api searchs
        /// </remarks>
        public override IQueryable<string> Search(string searchterm)
        {
            bool quickListEnabled = false;
            if (bool.TryParse(GetAttributeValue("QuickListEnabled"), out quickListEnabled) && quickListEnabled)
            {
                var people = Common.FindPeopleByFamilyId(searchterm)
                                        .OrderBy(x => x.LastName)
                                        .ToList()
                                        .Select(x =>
                                        {
                                            x.LoadAttributes();
                                            return string.Format("{0}, {1} - {2}",
                                                x.LastName,
                                                x.NickName,
                                                x.GetAttributeValue("FamilyIdCard"));
                                        });

                return people.AsQueryable();
            }
            else
            {
                return null;
            }
        }
    }
}
