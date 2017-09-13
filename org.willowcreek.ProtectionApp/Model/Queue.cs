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
    [Table("_org_willowcreek_ProtectionApp_Queue")]
    public class Queue : Rock.Data.Model<Queue>, Rock.Security.ISecured
    {
        public Queue()
            : base()
        {

        }

        #region Entity Properties
        /// <summary>
        /// Gets or sets the protection app requester first name
        /// </summary>
        /// <value>Requester first name.</value>
        [DataMember]
        [MaxLength(50)]
        public string RequesterFirstName { get; set; }

        /// <summary>
        /// Gets or sets the protection app requester last name
        /// </summary>
        /// <value>Requester last name.</value>
        [DataMember]
        [MaxLength(50)]
        public string RequesterLastName { get; set; }

        /// <summary>
        /// Gets or sets the campus.
        /// </summary>
        /// <value>Campus value</value>
        [DataMember]
        [MaxLength(100)]
        public string Campus { get; set; }

        /// <summary>
        /// Gets or sets the ministry.
        /// </summary>
        /// <value>Ministry value</value>
        [DataMember]
        [MaxLength(100)]
        public string Ministry { get; set; }

        [DataMember]
        public int? WorkflowId { get; set; }

        /// <summary>
        /// Gets or sets the protection applicant first name.
        /// </summary>
        /// <value>The applicant first name.</value>
        [DataMember]
        [MaxLength(50)]
        public string ApplicantFirstName { get; set; }

        /// <summary>
        /// Gets or sets the protection applicant last name.
        /// </summary>
        /// <value>The applicant last name.</value>
        [DataMember]
        [MaxLength(50)]
        public string ApplicantLastName { get; set; }

        /// <summary>
        /// Gets or sets the applicant person alias guid
        /// </summary>
        /// <value>Applicant person alias uniqueidentifer</value>
        [DataMember]
        public Guid ApplicantPersonAliasGuid { get; set; }

        /// <summary>
        /// Gets or sets the requester person alias guid
        /// </summary>
        /// <value>Requester person alias uniqueidentifer</value>
        [DataMember]
        public Guid RequesterPersonAliasGuid { get; set; }


        #endregion

        #region Virtual Properties

        #endregion
    }
    #region Entity Configuration

    /// <summary>
    /// 
    /// </summary>
    public partial class QueueConfiguration : EntityTypeConfiguration<Queue>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceConfiguration"/> class.
        /// </summary>
        public QueueConfiguration()
        {
            //this.HasOptional(r => r.Questionnaire).WithMany().HasForeignKey(r => r.QuestionnaireId).WillCascadeOnDelete(false);
        }
    }

    #endregion

}
