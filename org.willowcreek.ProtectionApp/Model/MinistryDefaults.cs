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
    /// Ministry default values for a new protection app
    /// </summary>
    [Table("_org_willowcreek_MinistryDefaults")]
    public class MinistryDefaults : Rock.Data.Model<MinistryDefaults>, Rock.Security.ISecured
    {
        public MinistryDefaults()
            : base()
        {

        }

        #region Entity Properties
        /// <summary>
        /// Gets or sets the Ministry.
        /// </summary>
        /// <value>The ministry guid.</value>
        [DataMember]
        public Guid Ministry { get; set; }

        /// <summary>
        /// Gets or sets the reply to name.
        /// </summary>
        /// <value>The reply to name.</value>
        [DataMember]
        [MaxLength(50)]
        public string ReplyToName { get; set; }

        /// <summary>
        /// Gets or sets the reply to email
        /// </summary>
        /// <value>The reply to email.</value>
        [DataMember]
        [MaxLength(75)]
        public string ReplyToEmail { get; set; }

        /// <summary>
        /// Gets or sets the protection app email template.
        /// </summary>
        /// <value>The protection app email template.</value>
        [DataMember]
        public Guid ProtectionEmailTemplate { get; set; }



        #endregion

        #region Virtual Properties

        #endregion
    }
    #region Entity Configuration

    /// <summary>
    /// 
    /// </summary>
    public partial class MinistryDefaultsConfiguration : EntityTypeConfiguration<MinistryDefaults>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceConfiguration"/> class.
        /// </summary>
        public MinistryDefaultsConfiguration()
        {
           // this.HasOptional(r => r.Questionnaire).WithMany().HasForeignKey(r => r.QuestionnaireId).WillCascadeOnDelete(false);
        }
    }

    #endregion

}
