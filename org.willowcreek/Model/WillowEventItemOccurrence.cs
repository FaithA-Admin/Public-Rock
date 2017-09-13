using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization;
using System.Collections.Generic;
using Rock.Data;
using Rock.Security;

namespace org.willowcreek.Model
{
    /// <summary>
    /// Represents a Willow event item occurrence.
    /// </summary>
    [Table("wcView_WillowEventItemOccurrence")]
    public class WillowEventItemOccurrence : Model<WillowEventItemOccurrence>
    {
        public WillowEventItemOccurrence() : base() { }

        #region Entity Properties

        /// <summary>
        /// Gets or sets the event item identifier.
        /// </summary>
        /// <value>
        /// The event item identifier.
        /// </value>        
        [DataMember]
        public int EventItemId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the summary.
        /// </summary>
        /// <value>
        /// The summary.
        /// </value>
        [DataMember]
        public string Summary { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the details URL.
        /// </summary>
        /// <value>
        /// The details URL.
        /// </value>
        [DataMember]
        public string DetailsURL { get; set; }

        /// <summary>
        /// Gets or sets the campus identifier.
        /// </summary>
        /// <value>
        /// The campus identifier.
        /// </value>
        [DataMember]
        public int? CampusId { get; set; }

        /// <summary>
        /// Gets or sets the campus.
        /// </summary>
        /// <value>
        /// The campus.
        /// </value>
        [DataMember]
        public string CampusName { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        [DataMember]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the contact name.
        /// </summary>
        /// <value>
        /// The contact name.
        /// </value>
        [DataMember]
        public string ContactName { get; set; }

        /// <summary>
        /// Gets or sets the contact phone.
        /// </summary>
        /// <value>
        /// The contact phone.
        /// </value>
        [DataMember]
        public string ContactPhone { get; set; }

        /// <summary>
        /// Gets or sets the contact email.
        /// </summary>
        /// <value>
        /// The contact email.
        /// </value>
        [DataMember]
        public string ContactEmail { get; set; }

        /// <summary>
        /// Gets or sets the photo.
        /// </summary>
        /// <value>
        /// The photo.
        /// </value>
        [DataMember]
        public string Photo { get; set; }

        /// <summary>
        /// Gets or sets the registration link URL.
        /// </summary>
        /// <value>
        /// The registration link URL.
        /// </value>
        [DataMember]
        public string RegistrationLinkURL { get; set; }

        /// <summary>
        /// Gets or sets the schedule identifier.
        /// </summary>
        /// <value>
        /// The schedule identifier.
        /// </value>
        [DataMember]
        public int ScheduleId { get; set; }

        /// <summary>
        /// Gets or sets the event iCalendar content.
        /// </summary>
        /// <value>
        /// The event iCalendar content.
        /// </value>
        [DataMember]
        public string EventiCalendarContent { get; set; }

        /// <summary>
        /// Gets or sets the event start date time.
        /// </summary>
        /// <value>
        /// The event start date time.
        /// </value>
        [DataMember]
        public DateTime? EventStartDateTime { get; set; }

        /// <summary>
        /// Gets or sets the event end date time.
        /// </summary>
        /// <value>
        /// The event end date time.
        /// </value>
        [DataMember]
        public DateTime? EventEndDateTime { get; set; }
        /// <summary>
        /// Gets or sets the registration identifier.
        /// </summary>
        /// <value>
        /// The registration identifier.
        /// </value>
        [DataMember]
        public int? RegistrationId { get; set; }

        /// <summary>
        /// Gets or sets the registration start date time.
        /// </summary>
        /// <value>
        /// The registration start date time.
        /// </value>
        [DataMember]
        public DateTime? RegistrationStartDateTime { get; set; }

        /// <summary>
        /// Gets or sets the registration end date time.
        /// </summary>
        /// <value>
        /// The registration end date time.
        /// </value>
        [DataMember]
        public DateTime? RegistrationEndDateTime { get; set; }
               
        #endregion


        #region Virtual Properties

        #endregion

        #region Methods

        #endregion
    }

    #region Entity Configuration

    /// <summary>
    /// WillowEventItemOccurrence Configuration class.
    /// </summary>
    public partial class WillowEventItemOccurrenceConfiguration : EntityTypeConfiguration<WillowEventItemOccurrence>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WillowEventItemOccurrenceConfiguration" /> class.
        /// </summary>
        public WillowEventItemOccurrenceConfiguration()
        {
        }
    }

    #endregion
}