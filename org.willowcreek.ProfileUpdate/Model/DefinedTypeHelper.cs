using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock.Data;

namespace org.willowcreek.ProfileUpdate.Model
{
    public class DefinedTypeHelper
    {
        // These two strings must be the names of the defined type in the database
        protected const string _sectionCommunityTypeName = "Sections";
        protected const string _childGradeTypeName = "Grade";

        public static Guid SECTION_COMMUNITY_GUID
        {
            get
            {
                RockContext context = new RockContext();

                Guid ret = Guid.Empty;

                if (context.DefinedTypes.Any(dt => dt.Name == _sectionCommunityTypeName))
                {
                    ret = context.DefinedTypes.Single(dt => dt.Name == _sectionCommunityTypeName).Guid;
                }

                return ret;
            }
        }
        public static Guid CHILD_GRADE_GUID
        {
            get
            {
                RockContext context = new RockContext();

                Guid ret = Guid.Empty;

                if (context.DefinedTypes.Any(dt => dt.Name == _childGradeTypeName))
                {
                    ret = context.DefinedTypes.Single(dt => dt.Name == _childGradeTypeName).Guid;
                }

                return ret;
            }
        }
    }
}
