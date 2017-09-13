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
    public enum RecommendedStatus
    {
        No = 0,
        Yes = 1,
        Depends = 2
    }
    /// <summary>
    /// A Protection App Reference
    /// </summary>
    [Table("_org_willowcreek_ProtectionApp_Reference")]
    public class Reference : Rock.Data.Model<Reference>, Rock.Security.ISecured
    {
        public Reference()
            : base()
        {

        }

        #region Entity Properties
        /// <summary>
        /// Gets or sets the submission date.
        /// </summary>
        /// <value>The submission date.</value>
        [DataMember]
        public DateTime? SubmissionDate { get; set; }

        /// <summary>
        /// Gets or sets the questionnaire identifier.
        /// </summary>
        /// <value>The questionnaire identifier.</value>
        [DataMember]
        public int? QuestionnaireId { get; set; }

        /// <summary>
        /// Gets or sets the referenced person identifier.
        /// </summary>
        /// <value>The referenced person identifier.</value>
        [DataMember]
        public Guid ReferencePersonAliasGuid { get; set; }

        /// <summary>
        /// Gets or sets the workflow identifier.
        /// </summary>
        /// <value>The workflow identifier.</value>
        [DataMember]
        public string WorkflowId { get;set;}

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        [DataMember]
        [MaxLength(100)]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the name of the middle.
        /// </summary>
        /// <value>The name of the middle.</value>
        [DataMember]
        [MaxLength(100)]
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        [DataMember]
        [MaxLength(100)]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [junior high student].
        /// </summary>
        /// <value><c>true</c> if [junior high student]; otherwise, <c>false</c>.</value>
        /// <remarks>Marked Obsolete per Protection Management July 2016</remarks>
        [DataMember]
        [Obsolete]
        public bool? JuniorHighStudent { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [known more than one year].
        /// </summary>
        /// <value><c>true</c> if [known more than one year]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool? KnownMoreThanOneYear { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is reference18.
        /// </summary>
        /// <value><c>true</c> if this instance is reference18; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool? IsReference18 { get; set; }

        /// <summary>
        /// Gets or sets the nature of relationship.
        /// </summary>
        /// <value>The nature of relationship.</value>
        [DataMember]
        [MaxLength(100)]
        public string NatureOfRelationship { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [maintain relationships].
        /// </summary>
        /// <value><c>true</c> if [maintain relationships]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool? MaintainRelationships { get; set; }

        /// <summary>
        /// Gets or sets the maintain relationships explain.
        /// </summary>
        /// <value>The maintain relationships explain.</value>
        [DataMember]
        [MaxLength(2000)]
        public string MaintainRelationshipsExplain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [respect healthy relational boundaries].
        /// </summary>
        /// <value><c>true</c> if [respect healthy relational boundaries]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool? RespectHealthyRelationalBoundaries { get; set; }

        /// <summary>
        /// Gets or sets the respect healthy relational boundaries explain.
        /// </summary>
        /// <value>The respect healthy relational boundaries explain.</value>
        [DataMember]
        [MaxLength(2000)]
        public string RespectHealthyRelationalBoundariesExplain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [criminal offenses].
        /// </summary>
        /// <value><c>true</c> if [criminal offenses]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool? CriminalOffenses { get; set; }

        /// <summary>
        /// Gets or sets the criminal offenses explain.
        /// </summary>
        /// <value>The criminal offenses explain.</value>
        [DataMember]
        [MaxLength(2000)]
        public string CriminalOffensesExplain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [manipulative behavior].
        /// </summary>
        /// <value><c>true</c> if [manipulative behavior]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool? ManipulativeBehavior { get; set; }

        /// <summary>
        /// Gets or sets the manipulative behavior explain.
        /// </summary>
        /// <value>The manipulative behavior explain.</value>
        [DataMember]
        [MaxLength(2000)]
        public string ManipulativeBehaviorExplain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [inflicted emotional harm].
        /// </summary>
        /// <value><c>true</c> if [inflicted emotional harm]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool? InflictedEmotionalHarm { get; set; }

        /// <summary>
        /// Gets or sets the inflicted emotional harm explain.
        /// </summary>
        /// <value>The inflicted emotional harm explain.</value>
        [DataMember]
        [MaxLength(2000)]
        public string InflictedEmotionalHarmExplain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [trust in child care].
        /// </summary>
        /// <value><c>true</c> if [trust in child care]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool? TrustInChildCare { get; set; }

        /// <summary>
        /// Gets or sets the trust in child care explain.
        /// </summary>
        /// <value>The trust in child care explain.</value>
        [DataMember]
        [MaxLength(2000)]
        public string TrustInChildCareExplain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [would recommend].
        /// </summary>
        /// <value><c>1</c> if [would recommend];<c> 2, false</c><c> 3, Depends</c></value>
        [DataMember]
        public int? WouldRecommend { get; set; }

        /// <summary>
        /// Text value of WouldRecommend
        /// </summary>
        [NotMapped]
        public string WouldRecommendDesc { 
            get
            {
                switch(WouldRecommend)
                {
                    case 0:
                        return "No";
                    case 1:
                        return "Yes";
                    case 2:
                        return "Depends";
                    default:
                        return "";
                }
            } 
        }

        /// <summary>
        /// Gets or sets the would recommend explain.
        /// </summary>
        /// <value>The would recommend explain.</value>
        [DataMember]
        [MaxLength(2000)]
        public string WouldRecommendExplain { get; set; }

        /// <summary>
        /// Gets or sets the signature.
        /// </summary>
        /// <value>The signature.</value>
        [DataMember]
        [MaxLength(100)]
        public string Signature { get; set; }

        /// <summary>
        /// Gets or sets the signature date.
        /// </summary>
        /// <value>The signature date.</value>
        [DataMember]
        public DateTime? SignatureDate { get; set; }

        /// <summary>
        /// Gets or sets the refnumber.
        /// </summary>
        /// <value>The refnumber.</value>
        [DataMember]
        public int? RefNumber { get; set; }

        /// <summary>
        /// Gets or sets the referenced person identifier.
        /// </summary>
        /// <value>The referenced person identifier.</value>
        [DataMember]
        public Guid ApplicantPersonAliasGuid { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        [DataMember]
        [MaxLength(100)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the reference nature of association as specified by the Applicant.
        /// </summary>
        /// <value>The reference nature of association.</value>
        [DataMember]
        [MaxLength(100)]
        public string NatureOfRelationshipApplicant { get; set; }
        #endregion

        #region Virtual Properties
        /// <summary>
        /// Gets or sets the questionnaire.
        /// </summary>
        /// <value>The questionnaire.</value>
        public virtual Questionnaire Questionnaire { get; set; }
        #endregion
    }
    #region Entity Configuration

    /// <summary>
    /// 
    /// </summary>
    public partial class ReferenceConfiguration : EntityTypeConfiguration<Reference>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceConfiguration"/> class.
        /// </summary>
        public ReferenceConfiguration()
        {
            this.HasOptional(r => r.Questionnaire).WithMany().HasForeignKey(r => r.QuestionnaireId).WillCascadeOnDelete(false);
        }
    }

    #endregion

}
