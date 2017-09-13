using Rock.Data;
using org.willowcreek.ProtectionApp.Data;

namespace org.willowcreek.ProtectionApp.Model
{
    /// <summary>
    /// WillowActiveVolunteers Service class
    /// </summary>
    public partial class WillowActiveVolunteersInGroupsService : Service<WillowActiveVolunteersInGroups>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WillowActiveVolunteersService"/> class
        /// </summary>
        /// <param name="context">The context.</param>
        public WillowActiveVolunteersInGroupsService(ProtectionAppContext context) : base(context) { }
    }

    /// <summary>
    /// Generated Extension Methods
    /// </summary>
    public static partial class WillowActiveVolunteersInGroupsExtensionMethods
    {
        /// <summary>
        /// Clones this WillowActiveVolunteers object to a new WillowActiveVolunteers object
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="deepCopy">if set to <c>true</c> a deep copy is made. If false, only the basic entity properties are copied.</param>
        /// <returns></returns>
        public static WillowActiveVolunteersInGroups Clone(this WillowActiveVolunteersInGroups source, bool deepCopy)
        {
            if (deepCopy)
            {
                return source.Clone() as WillowActiveVolunteersInGroups;
            }
            else
            {
                var target = new WillowActiveVolunteersInGroups();
                target.CopyPropertiesFrom(source);
                return target;
            }
        }

        /// <summary>
        /// Copies the properties from another WillowEventItemOccurrence object to this WillowEventItemOccurrence object
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        public static void CopyPropertiesFrom(this WillowActiveVolunteersInGroups target, WillowActiveVolunteersInGroups source)
        {
            target.Id = source.Id;
            target.FirstName = source.FirstName;
            target.LastName = source.LastName;
            target.GroupId = source.GroupId;
            target.GroupName = source.GroupName;
            target.ProtectionStatus = source.ProtectionStatus;
            target.BackgroundCheckDate = source.BackgroundCheckDate;
            target.OrderId = source.OrderId;
            target.CreatedDateTime = source.CreatedDateTime;
            target.ModifiedDateTime = source.ModifiedDateTime;
            target.CreatedByPersonAliasId = source.CreatedByPersonAliasId;
            target.ModifiedByPersonAliasId = source.ModifiedByPersonAliasId;
            target.Guid = source.Guid;
            target.ForeignId = source.ForeignId;
            target.ForeignGuid = source.ForeignGuid;
            target.ForeignKey = source.ForeignKey;
        }
    }
}
