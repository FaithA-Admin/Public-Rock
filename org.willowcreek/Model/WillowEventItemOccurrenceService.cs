using Rock.Data;

namespace org.willowcreek.Model
{
    /// <summary>
    /// WillowEventItemOccurrence Service class
    /// </summary>
    public partial class WillowEventItemOccurrenceService : Service<WillowEventItemOccurrence>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WillowEventItemOccurrenceService"/> class
        /// </summary>
        /// <param name="context">The context.</param>
        public WillowEventItemOccurrenceService(WillowContext context) : base(context) { }
    }

    /// <summary>
    /// Generated Extension Methods
    /// </summary>
    public static partial class WillowEventItemOccurrenceExtensionMethods
    {
        /// <summary>
        /// Clones this WillowEventItemOccurrence object to a new WillowEventItemOccurrence object
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="deepCopy">if set to <c>true</c> a deep copy is made. If false, only the basic entity properties are copied.</param>
        /// <returns></returns>
        public static WillowEventItemOccurrence Clone(this WillowEventItemOccurrence source, bool deepCopy)
        {
            if (deepCopy)
            {
                return source.Clone() as WillowEventItemOccurrence;
            }
            else
            {
                var target = new WillowEventItemOccurrence();
                target.CopyPropertiesFrom(source);
                return target;
            }
        }

        /// <summary>
        /// Copies the properties from another WillowEventItemOccurrence object to this WillowEventItemOccurrence object
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        public static void CopyPropertiesFrom(this WillowEventItemOccurrence target, WillowEventItemOccurrence source)
        {
            target.Id = source.Id;
            target.EventItemId = source.EventItemId;
            target.Name = source.Name;
            target.Summary = source.Summary;
            target.Description = source.Description;
            target.DetailsURL = source.DetailsURL;
            target.CampusId = source.CampusId;
            target.CampusName = source.CampusName;
            target.Location = source.Location;
            target.ContactName = source.ContactName;
            target.ContactPhone = source.ContactPhone;
            target.ContactEmail = source.ContactEmail;
            target.Photo = source.Photo;
            target.RegistrationLinkURL = source.RegistrationLinkURL;
            target.ScheduleId = source.ScheduleId;
            target.EventiCalendarContent = source.EventiCalendarContent;
            target.RegistrationId = source.RegistrationId;
            target.RegistrationStartDateTime = source.RegistrationStartDateTime;
            target.RegistrationEndDateTime = source.RegistrationEndDateTime;
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
