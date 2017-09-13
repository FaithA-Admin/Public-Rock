using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Runtime.Serialization;

using Rock.Attribute;
using Rock.Model;

namespace org.willowcreek.ProtectionApp.Model
{

    /// <summary>
    /// A Protection App Queue
    /// </summary>
    [Table("_org_willowcreek_ProtectionApp_Archive")]
    public class Archive : Rock.Data.Model<Archive>, Rock.Security.ISecured
    {
        public Archive()
            : base()
        {

        }

    #region Entity Properties
        /// <summary>
        /// Gets or sets the timestamp of when application was created
        /// </summary>
        /// <value>The application time of creation.</value>
        [DataMember]
        public DateTime ArchiveDateTime { get; set; }

        /// <summary>
        /// Gets or sets the status of the protection application
        /// </summary>
        /// <value>The protection application status.</value>
        [DataMember]
        public Guid ProtectionStatus { get; set; }

        /// <summary>
        /// Gets or sets the date of last background check
        /// </summary>
        /// <value>The background check date.</value>
        [DataMember]
        public DateTime? BackgroundCheckDate { get; set; }

        /// <summary>
        /// Gets or sets date of 1st reference completion.
        /// </summary>
        /// <value>The 1st reference date.</value>
        [DataMember]
        public DateTime? Reference1Date { get; set; }

        /// <summary>
        /// Gets or sets date of 2nd reference completion.
        /// </summary>
        /// <value>The 2nd reference date.</value>
        [DataMember]
        public DateTime? Reference2Date { get; set; }

        /// <summary>
        /// Gets or sets date of 3rd reference completion.
        /// </summary>
        /// <value>The 3rd reference date.</value>
        [DataMember]
        public DateTime? Reference3Date { get; set; }

        /// <summary>
        /// Gets or sets the date of the application
        /// </summary>
        /// <value>The application date.</value>
        [DataMember]
        public DateTime? ApplicationDate { get; set; }

        /// <summary>
        /// Gets or sets the date of protection policy acknowledgement by applicant
        /// </summary>
        /// <value>The protection policy ackowledgement date.</value>
        [DataMember]
        public DateTime? PolicyAcknowledgmentDate { get; set; }

        /// <summary>
        /// Gets or sets the file identifer of applicant background check
        /// </summary>
        /// <value>Background check identifier</value>
        [DataMember]
        public Guid? BackgroundCheckBinaryFileId { get; set; }

        /// <summary>
        /// Gets or sets the applicant background check result
        /// </summary>
        /// <value>Background check result</value>
        [DataMember]
        [MaxLength(25)]
        public string BackgroundCheckResult { get; set; }

        /// <summary>
        /// Gets or sets the file identifer of first reference
        /// </summary>
        /// <value>Reference identifier</value>
        [DataMember]
        public Guid? Reference1BinaryFileId { get; set; }

        /// <summary>
        /// Gets or sets the file identifer of second reference
        /// </summary>
        /// <valueReference identifier></value>
        [DataMember]
        public Guid? Reference2BinaryFileId { get; set; }

        /// <summary>
        /// Gets or sets the file identifer of third reference
        /// </summary>
        /// <value>Reference identifier</value>
        [DataMember]
        public Guid? Reference3BinaryFileId { get; set; }

        /// <summary>
        /// Gets or sets the file identifer of application
        /// </summary>
        /// <value>Application file identifier </value>
        [DataMember]
        public Guid? ApplicationBinaryFileId { get; set; }

        /// <summary>
        /// Gets or sets the uniqueidentifier of the requester alias
        /// </summary>
        /// <value>Person alias guid of requester</value>
        [DataMember]
        public Guid PersonAliasGuid { get; set; }
        #endregion

        #region Virtual Properties

        #endregion
    }
    #region Entity Configuration

    /// <summary>
    /// 
    /// </summary>
    public partial class ArchiveConfiguration : EntityTypeConfiguration<Archive>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArchiveConfiguration"/> class.
        /// </summary>
        public ArchiveConfiguration()
        {

        }
    }

    #endregion

}
