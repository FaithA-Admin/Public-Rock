using Rock.Data;

namespace org.willowcreek.Model
{
    /// <summary>
    /// WillowEventItemMinistry Service class
    /// </summary>
    public partial class WillowEventItemMinistryService : Service<WillowEventItemMinistry>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WillowEventItemMinistryService"/> class
        /// </summary>
        /// <param name="context"></param>
        public WillowEventItemMinistryService(WillowContext context) : base(context) { }
    }

    public static partial class WillowEventItemMinistryExtensionMethods
    {
        /// <summary>
        /// Clones this WillowEventItemMinistry object to a new WillowEventItemMinistry object
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="deepCopy">if set to <c>true</c> a deep copy is made. If false, only the basic entity properties are copied.</param>
        /// <returns></returns>
        public static WillowEventItemMinistry Clone(this WillowEventItemMinistry source, bool deepCopy)
        {
            if (deepCopy)
            {
                return source.Clone() as WillowEventItemMinistry;
            }
            else
            {
                var target = new WillowEventItemMinistry();
                target.CopyPropertiesFrom(source);
                return target;
            }
        }

        /// <summary>
        /// Copies the properties from another WillowEventItemOccurrence object to this WillowEventItemOccurrence object
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        public static void CopyPropertiesFrom(this WillowEventItemMinistry target, WillowEventItemMinistry source)
        {
            target.Id = source.Id;
            target.EventItemId = source.EventItemId;
            target.Ministry = source.Ministry;
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
