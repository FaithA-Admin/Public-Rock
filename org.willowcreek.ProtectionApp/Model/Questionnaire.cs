using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Runtime.Serialization;

using Rock.Model;
using Newtonsoft.Json;

namespace org.willowcreek.ProtectionApp.Model
{
    /// <summary>
    /// A Protection App Questionnaire
    /// </summary>
    [Table("_org_willowcreek_ProtectionApp_Questionnaire")]
    //-->Does not work<--[EncryptedTextField("ApplicantSsn", "Applicant Social Security Number", true)]
    public class Questionnaire : Rock.Data.Model<Questionnaire>, Rock.Security.ISecured
    {
        #region Entity Properties
        /// <summary>
        /// Gets or sets the submission date.
        /// </summary>
        /// <value>The submission date.</value>
        [DataMember]
        public DateTime SubmissionDate { get; set; }

        /// <summary>
        /// Gets or sets the applicant identifier.
        /// </summary>
        /// <value>The applicant identifier.</value>       
        [DataMember]
        public Guid ApplicantPersonAliasGuid { get; set; }

        /// <summary>
        /// Gets or sets the workflow identifier.
        /// </summary>
        /// <value>The workflow identifier.</value>
        [DataMember]
        public string WorkflowId { get;set;}

        /// <summary>
        /// Gets or sets the applicant SSN.
        /// </summary>
        /// <value>The applicant SSN.</value>
        [Required(ErrorMessage = "Social Security Number is required")]
        [JsonProperty(PropertyName = "ApplicantSsn")]
        [NotMapped]
        public string ApplicantSsn { get; set; }

        /// <summary>
        /// Gets or sets the first name of the legal.
        /// </summary>
        /// <value>The first name of the legal.</value>
        [Required(ErrorMessage = "Legal First Name is required")]
        [DataMember(IsRequired = true)]
        [MaxLength(100)]
        public string LegalFirstName { get; set; }

        /// <summary>
        /// Gets or sets the name of the middle.
        /// </summary>
        /// <value>The name of the middle.</value>
        [DataMember(IsRequired = true)]
        [MaxLength(100)]
        public string MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        [Required(ErrorMessage = "Last Name is required")]
        [DataMember(IsRequired = true)]
        [MaxLength(100)]
        public string LastName { get; set; }

        /// <summary>
        /// Suffix
        /// </summary>
        [JsonProperty(PropertyName = "Suffix")]
        [NotMapped]
        public int? Suffix { get; set; }

        /// <summary>
        /// Gets or sets the full name of the legal.
        /// </summary>
        /// <value>The full name of the legal.</value>
        [Required(ErrorMessage = "Full Legal Name is required")]
        [DataMember(IsRequired = true)]
        [MaxLength(100)]
        public string FullLegalName { get; set; }

        /// <summary>
        /// Gets or sets the name of the nick.
        /// </summary>
        /// <value>The name of the nick.</value>
        [DataMember]
        [MaxLength(100)]
        public string Nickname { get; set; }

        /// <summary>
        /// Gets or sets the name of the maiden.
        /// </summary>
        /// <value>The name of the maiden.</value>
        [DataMember]
        [MaxLength(100)]
        public string MaidenName { get; set; }

        /// <summary>
        /// Gets or sets the current address street.
        /// </summary>
        /// <value>The current address street.</value>
        [DataMember]
        [MaxLength(100)]
        public string CurrentAddressStreet { get; set; }
        /// <summary>
        /// Gets or sets the current address city.
        /// </summary>
        /// <value>The current address city.</value>
        [DataMember]
        [MaxLength(50)]
        public string CurrentAddressCity { get; set; }
        /// <summary>
        /// Gets or sets the state of the current address.
        /// </summary>
        /// <value>The state of the current address.</value>
        [DataMember]
        [MaxLength(50)]
        public string CurrentAddressState { get; set; }
        /// <summary>
        /// Gets or sets the current address zip.
        /// </summary>
        /// <value>The current address zip.</value>
        [DataMember]
        [MaxLength(50)]
        public string CurrentAddressZip { get; set; }

        /// <summary>
        /// Gets or sets the time at current address.
        /// </summary>
        /// <value>The time at current address.</value>
        [DataMember]
        public LivedAtTimePeriod TimeAtCurrentAddress { get; set; }

        /// <summary>
        /// Gets or sets the previous address street.
        /// </summary>
        /// <value>The previous address street.</value>
        [DataMember]
        [MaxLength(100)]
        public string PreviousAddressStreet { get; set; }
        /// <summary>
        /// Gets or sets the previous address city.
        /// </summary>
        /// <value>The previous address city.</value>
        [DataMember]
        [MaxLength(50)]
        public string PreviousAddressCity { get; set; }
        /// <summary>
        /// Gets or sets the state of the previous address.
        /// </summary>
        /// <value>The state of the previous address.</value>
        [DataMember]
        [MaxLength(50)]
        public string PreviousAddressState { get; set; }
        /// <summary>
        /// Gets or sets the previous address zip.
        /// </summary>
        /// <value>The previous address zip.</value>
        [DataMember]
        [MaxLength(50)]
        public string PreviousAddressZip { get; set; }

        /// <summary>
        /// Gets or sets the home phone.
        /// </summary>
        /// <value>The home phone.</value>
        [DataMember]
        [MaxLength(15)]
        public string HomePhone { get; set; }

        /// <summary>
        /// Gets or sets the mobile phone.
        /// </summary>
        /// <value>The mobile phone.</value>
        [DataMember]
        [MaxLength(15)]
        public string MobilePhone { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        /// <value>The email address.</value>
        [DataMember]
        [MaxLength(100)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the date of birth.
        /// </summary>
        /// <value>The date of birth.</value>
        [DataMember]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        /// <value>The gender.</value>
        [DataMember]
        public Gender Gender { get; set; }

        /// <summary>
        /// Gets or sets the marital status.
        /// </summary>
        /// <value>The marital status.</value>
        [DataMember]
        public MaritalStatus MaritalStatus { get; set; }

        /// <summary>
        /// Gets or sets the children count.
        /// </summary>
        /// <value>The children count.</value>
        [DataMember]
        public int ChildrenCount { get; set; }

        /// <summary>
        /// Gets or sets the children ages.
        /// </summary>
        /// <value>The children ages.</value>
        [DataMember]
        [MaxLength(100)]
        public string ChildrenAges { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [legal guardian].
        /// </summary>
        /// <value><c>true</c> if [legal guardian]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool LegalGuardian { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [attend WCCC].
        /// </summary>
        /// <value><c>true</c> if [attend WCCC]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool AttendWccc { get; set; }

        /// <summary>
        /// Gets or sets the start WCCC.
        /// </summary>
        /// <value>The start WCCC.</value>
        [DataMember]
        public DateTime? StartWccc { get; set; }

        /// <summary>
        /// Gets or sets the pornography addiction.
        /// </summary>
        /// <value>The pornography addiction.</value>
        [DataMember]
        public AddictionStatus PornographyAddiction { get; set; }

        /// <summary>
        /// Gets or sets the alcohol addiction.
        /// </summary>
        /// <value>The alcohol addiction.</value>
        [DataMember]
        public AddictionStatus AlcoholAddiction { get; set; }

        /// <summary>
        /// Gets or sets the drug addiction.
        /// </summary>
        /// <value>The drug addiction.</value>
        [DataMember]
        public AddictionStatus DrugAddiction { get; set; }

        /// <summary>
        /// Gets or sets the addiction explain.
        /// </summary>
        /// <value>The addiction explain.</value>
        [DataMember]
        [MaxLength(2000)]
        public string AddictionExplain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [pornography vulnerable].
        /// </summary>
        /// <value><c>true</c> if [pornography vulnerable]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool PornographyVulnerable { get; set; }

        /// <summary>
        /// Gets or sets the pornography vulnerable explain.
        /// </summary>
        /// <value>The pornography vulnerable explain.</value>
        [DataMember]
        [MaxLength(2000)]
        public string PornographyVulnerableExplain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [DCFS investigation].
        /// </summary>
        /// <value><c>true</c> if [DCFS investigation]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool DcfsInvestigation { get; set; }

        /// <summary>
        /// Gets or sets the DCFS investigation explain.
        /// </summary>
        /// <value>The DCFS investigation explain.</value>
        [DataMember]
        [MaxLength(2000)]
        public string DcfsInvestigationExplain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [order of protection].
        /// </summary>
        /// <value><c>true</c> if [order of protection]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool OrderOfProtection { get; set; }

        /// <summary>
        /// Gets or sets the order of protection explain.
        /// </summary>
        /// <value>The order of protection explain.</value>
        [DataMember]
        [MaxLength(2000)]
        public string OrderOfProtectionExplain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [committed or accused].
        /// </summary>
        /// <value><c>true</c> if [committed or accused]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool CommittedOrAccused { get; set; }

        /// <summary>
        /// Gets or sets the committeed or accused explain.
        /// </summary>
        /// <value>The committeed or accused explain.</value>
        [DataMember]
        [MaxLength(2000)]
        public string CommittedOrAccusedExplain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [relationship vulnerable].
        /// </summary>
        /// <value><c>true</c> if [relationship vulnerable]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool RelationshipVulnerable { get; set; }

        /// <summary>
        /// Gets or sets the relationship vulnerable explain.
        /// </summary>
        /// <value>The relationship vulnerable explain.</value>
        [DataMember]
        [MaxLength(2000)]
        public string RelationshipVulnerableExplain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [asked to leave].
        /// </summary>
        /// <value><c>true</c> if [asked to leave]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool AskedToLeave { get; set; }

        /// <summary>
        /// Gets or sets the asked to leave explain.
        /// </summary>
        /// <value>The asked to leave explain.</value>
        [DataMember]
        [MaxLength(2000)]
        public string AskedToLeaveExplain { get; set; }

        /// <summary>
        /// Gets or sets the name of the reference1.
        /// </summary>
        /// <value>The name of the reference1.</value>
        [DataMember]
        [MaxLength(100)]
        public string Reference1Name { get; set; }
        /// <summary>
        /// Gets or sets the reference1 phone.
        /// </summary>
        /// <value>The reference1 phone.</value>
        /// <remarks>Marked Obsolete per Protection Management July 2016</remarks>
        [DataMember]
        [MaxLength(15)]
        [Obsolete]
        public string Reference1Phone { get; set; }
        /// <summary>
        /// Gets or sets the reference1 email.
        /// </summary>
        /// <value>The reference1 email.</value>
        [DataMember]
        [MaxLength(100)]
        public string Reference1Email { get; set; }

        /// <summary>
        /// Gets or sets the reference1 nature of association.
        /// </summary>
        /// <value>The reference1 nature of association.</value>
        [DataMember]
        [MaxLength(100)]
        public string Reference1NatureOfAssociation { get; set; }

        /// <summary>
        /// Gets or sets the reference identifier.
        /// </summary>
        /// <value>The reference identifier.</value>
        [JsonProperty(PropertyName = "Reference1PersonAliasGuid")]
        [NotMapped]
        public string Reference1PersonAliasGuid { get; set; }

        /// <summary>
        /// Gets or sets the name of the reference2.
        /// </summary>
        /// <value>The name of the reference2.</value>
        [DataMember]
        [MaxLength(100)]
        public string Reference2Name { get; set; }
        /// <summary>
        /// Gets or sets the reference2 phone.
        /// </summary>
        /// <value>The reference2 phone.</value>
        /// <remarks>Marked Obsolete per Protection Management July 2016</remarks>
        [DataMember]
        [MaxLength(15)]
        [Obsolete]
        public string Reference2Phone { get; set; }
        /// <summary>
        /// Gets or sets the reference2 email.
        /// </summary>
        /// <value>The reference2 email.</value>
        [DataMember]
        [MaxLength(100)]
        public string Reference2Email { get; set; }

        /// <summary>
        /// Gets or sets the reference2 nature of association.
        /// </summary>
        /// <value>The reference2 nature of association.</value>
        [DataMember]
        [MaxLength(100)]
        public string Reference2NatureOfAssociation { get; set; }

        /// <summary>
        /// Gets or sets the reference identifier.
        /// </summary>
        /// <value>The reference identifier.</value>       
        [JsonProperty(PropertyName = "Reference2PersonAliasGuid")]
        [NotMapped]
        public string Reference2PersonAliasGuid { get; set; }

        /// <summary>
        /// Gets or sets the name of the reference3.
        /// </summary>
        /// <value>The name of the reference3.</value>
        [DataMember]
        [MaxLength(100)]
        public string Reference3Name { get; set; }
        /// <summary>
        /// Gets or sets the reference3 phone.
        /// </summary>
        /// <value>The reference3 phone.</value>
        /// <remarks>Marked Obsolete per Protection Management July 2016</remarks>
        [DataMember]
        [MaxLength(15)]
        [Obsolete]
        public string Reference3Phone { get; set; }
        /// <summary>
        /// Gets or sets the reference3 email.
        /// </summary>
        /// <value>The reference3 email.</value>
        [DataMember]
        [MaxLength(100)]
        public string Reference3Email { get; set; }

        /// <summary>
        /// Gets or sets the reference3 nature of association.
        /// </summary>
        /// <value>The reference3 nature of association.</value>
        [DataMember]
        [MaxLength(100)]
        public string Reference3NatureOfAssociation { get; set; }

        /// <summary>
        /// Gets or sets the reference identifier.
        /// </summary>
        /// <value>The reference identifier.</value>       
        [JsonProperty(PropertyName = "Reference3PersonAliasGuid")]
        [NotMapped]
        public string Reference3PersonAliasGuid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [correct information].
        /// </summary>
        /// <value><c>true</c> if [correct information]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool CorrectInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [authorize release].
        /// </summary>
        /// <value><c>true</c> if [authorize release]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool AuthorizeRelease { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [authorize reference].
        /// </summary>
        /// <value><c>true</c> if [authorize reference]; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool AuthorizeReference { get; set; }

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
        /// Gets or sets the guardian signature.
        /// </summary>
        /// <value>The guardian signature.</value>
        [DataMember]
        [MaxLength(100)]
        public string GuardianSignature { get; set; }

        /// <summary>
        /// Gets or sets the guardian signature date.
        /// </summary>
        /// <value>The guardian signature date.</value>
        [DataMember]
        public DateTime? GuardianSignatureDate { get; set; }

        /// <summary>
        /// Gets or sets the campus identifier.
        /// </summary>
        /// <value>
        /// The campus identifier.
        /// </value>
        [DataMember]
        public int? CampusId { get; set; }

        #endregion

        #region Virtual Properties

        /// <summary>
        /// Gets or sets the campus.
        /// </summary>
        /// <value>
        /// The campus.
        /// </value>
        public virtual Campus Campus { get; set; }
        #endregion
    }

    #region Entity Configuration

    /// <summary>
    /// 
    /// </summary>
    public partial class QuestionnaireConfiguration : EntityTypeConfiguration<Questionnaire>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QuestionnaireConfiguration"/> class.
        /// </summary>
        public QuestionnaireConfiguration()
        {
            this.HasOptional(r => r.Campus).WithMany().HasForeignKey(r => r.CampusId).WillCascadeOnDelete(false);
        }
    }

    #endregion

}
