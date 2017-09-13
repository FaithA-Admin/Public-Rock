using Rock.Data;
using org.willowcreek.ProtectionApp.Data;

namespace org.willowcreek.ProtectionApp.Model
{
    /// <summary>
    /// WillowActiveVolunteers Service class
    /// </summary>
   public partial class WillowActiveVolunteersService : Service<WillowActiveVolunteers>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WillowActiveVolunteersService"/> class
        /// </summary>
        /// <param name="context">The context.</param>
        public WillowActiveVolunteersService(ProtectionAppContext context) : base(context) { }
    }

    /// <summary>
    /// Generated Extension Methods
    /// </summary>
    public static partial class WillowActiveVolunteersExtensionMethods
    {
        /// <summary>
        /// Clones this WillowActiveVolunteers object to a new WillowActiveVolunteers object
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="deepCopy">if set to <c>true</c> a deep copy is made. If false, only the basic entity properties are copied.</param>
        /// <returns></returns>
        public static WillowActiveVolunteers Clone(this WillowActiveVolunteers source, bool deepCopy)
        {
            if (deepCopy)
            {
                return source.Clone() as WillowActiveVolunteers;
            }
            else
            {
                var target = new WillowActiveVolunteers();
                target.CopyPropertiesFrom(source);
                return target;
            }
        }

        /// <summary>
        /// Copies the properties from another WillowEventItemOccurrence object to this WillowEventItemOccurrence object
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        public static void CopyPropertiesFrom(this WillowActiveVolunteers target, WillowActiveVolunteers source)
        {
            target.Id = source.Id;
            target.FirstName = source.FirstName;
            target.LastName = source.LastName;
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
