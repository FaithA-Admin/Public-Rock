using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Runtime.Serialization;

using Rock.Attribute;
using Rock.Model;


namespace org.willowcreek.Model
{

    /// <summary>
    /// Primary ministry contact for new protection app
    /// </summary>
    [Table("_org_willowcreek_CampusMinistryPerson")]
    public class CampusMinistryPerson : Rock.Data.Model<CampusMinistryPerson>, Rock.Security.ISecured
    {
        public CampusMinistryPerson()
            : base()
        {

        }

        #region Entity Properties
        /// <summary>
        /// Gets or sets the Campus.
        /// </summary>
        /// <value>The campus guid.</value>
        [DataMember]
        public Guid? Campus { get; set; }

        /// <summary>
        /// Gets or sets the reply to name.
        /// </summary>
        /// <value>The Ministry guid.</value>
        [DataMember]
        public Guid? Ministry { get; set; }

        /// <summary>
        /// Guid 
        /// </summary>
        /// <value>The application primary contact guid.</value>
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
    public partial class CampusMinistryPersonConfiguration : EntityTypeConfiguration<CampusMinistryPerson>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceConfiguration"/> class.
        /// </summary>
        public CampusMinistryPersonConfiguration()
        {
           // this.HasOptional(r => r.Questionnaire).WithMany().HasForeignKey(r => r.QuestionnaireId).WillCascadeOnDelete(false);
        }
    }

    #endregion

}
