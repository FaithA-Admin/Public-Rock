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
    /// Primary ministry contact for new protection app
    /// </summary>
    [Table("_org_willowcreek_UserDefaults")]
    public class UserDefaults : Rock.Data.Model<UserDefaults>, Rock.Security.ISecured
    {
        public UserDefaults()
            : base()
        {

        }

        #region Entity Properties

        /// <summary>
        /// Gets or sets contact person guid identifier
        /// </summary>
        /// <value>contact person alias uniqueidentifer</value>
        [DataMember]
        public Guid PersonAliasGuid { get; set; }

        /// <summary>
        /// Gets or sets the reply to name of protection app requester
        /// </summary>
        /// <value>The reply to name.</value>
        [DataMember]
        [MaxLength(50)]
        public string ProtectionReplyToName { get; set; }

        /// <summary>
        /// Gets or sets the reply to email of protection app requester
        /// </summary>
        /// <value>The reply to email.</value>
        [DataMember]
        [MaxLength(75)]
        public string ProtectionReplyToEmail { get; set; }

        /// <summary>
        /// Gets or sets the guid identifier of protection app requester
        /// </summary>
        /// <value>The ministry uniqueidentifer.</value>
        [DataMember]
        public Guid ProtectionRequesterPersonAliasGuid { get; set; }

        /// <summary>
        /// Gets or sets the protection personal email text
        /// </summary>
        /// <value>protection app personal message text</value>
        [DataMember]
        [MaxLength(1000)]
        public string ProtectionEmailPersonalMessage { get; set; }


        #endregion

        #region Virtual Properties

        #endregion
    }
    #region Entity Configuration

    /// <summary>
    /// 
    /// </summary>
    public partial class UserDefaultsConfiguration : EntityTypeConfiguration<UserDefaults>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserDefaultsConfiguration"/> class.
        /// </summary>
        public UserDefaultsConfiguration()
        {
           // this.HasOptional(r => r.Questionnaire).WithMany().HasForeignKey(r => r.QuestionnaireId).WillCascadeOnDelete(false);
        }
    }

    #endregion

}
