using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization;
using Rock.Data;
using Rock.Security;

namespace org.willowcreek.Model
{
    /// <summary>
    /// Represents a Willow event item ministry.
    /// </summary>
    [Table("wcView_WillowEventItemMinistry")]
    public class WillowEventItemMinistry : Model<WillowEventItemMinistry>
    {
        public WillowEventItemMinistry() : base() { }

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
        /// Gets or sets the ministry.
        /// </summary>
        /// <value>
        /// The ministry.
        /// </value>
        [DataMember]
        public string Ministry { get; set; }
        #endregion

        #region Virtual Properties

        #endregion

        #region Methods

        #endregion
    }

    #region Entity Configuration

    public partial class WillowEventItemMinistryConfiguration : EntityTypeConfiguration<WillowEventItemMinistry>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WillowEventItemMinistryConfiguration"/>
        /// </summary>
        public WillowEventItemMinistryConfiguration()
        {
        }
    }
    #endregion
}
