using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Rock.Model;

namespace org.willowcreek.FileManagement.Model
{
    [Table("_org_willowcreek_PersonDocument")]
    public class PersonDocument : Rock.Data.Model<PersonDocument>, Rock.Security.ISecured
    {
        #region Entity Properties
        [DataMember]
        [Required(ErrorMessage = "Person is required")]
        public Guid PersonAliasGuid
        {
            get;
            set;
        }

        [DataMember]
        [Required(ErrorMessage = "File is required")]
        public Guid BinaryFileId
        { 
            get; 
            set;
        }
        #endregion

        public partial class PersonDocmentConfiguration : EntityTypeConfiguration<PersonDocument>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PersonDocmentConfiguration"/> class.
            /// </summary>
            public PersonDocmentConfiguration()
            {
                //this.HasRequired(r => r.PersonAlias);
                //this.HasRequired(r => r.File);
            }
        }

    }
}
