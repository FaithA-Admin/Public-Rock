﻿using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Runtime.Serialization;
using Rock.Data;

namespace org.willowcreek.ProtectionApp.Model
{
    /// <summary>
    /// Willow Active Volunteers view
    /// </summary>
    [Table("wcView_WillowActiveVolunteers")]
    public class WillowActiveVolunteers : Model<WillowActiveVolunteers>
    {
        public WillowActiveVolunteers() : base() { }

        #region Entity Properties
               
        /// <summary>
        /// Gets or sets the person's first name.
        /// </summary>
        /// <value>
        /// The person's first name.
        /// </value>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the person's last name.
        /// </summary>
        /// <value>
        /// The person's last name.
        /// </value>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the person's protection status.
        /// </summary>
        /// <value>
        /// The person's protection status.
        /// </value>
        [DataMember]
        public string ProtectionStatus { get; set; }

        /// <summary>
        /// Gets or sets the person's background check date.
        /// </summary>
        /// <value>
        /// The person's background check date.
        /// </value>
        [DataMember]
        public DateTime? BackgroundCheckDate { get; set; }

        /// <summary>
        /// Gets or sets the order identifier.
        /// </summary>
        /// <value>
        /// The order identifier.
        /// </value>
        [DataMember]
        public int? OrderId { get; set; }

        #endregion

        #region Virtual Properties

        #endregion

        #region Methods

        #endregion
    }

    #region Entity Configuration
    public partial class WillowActiveVolunteersConfiguration : EntityTypeConfiguration<WillowActiveVolunteers>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WillowActiveVolunteersConfiguration"/>
        /// </summary>
        public WillowActiveVolunteersConfiguration()
        {
        }
    }
    #endregion
}
