using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.willowcreek.FileManagement.Model;

namespace org.willowcreek.FileManagement.Data
{
    public class PersonDocumentService : Rock.Data.Service<PersonDocument>
    {
        public PersonDocumentService(FileManagementContext context) : base(context) { }
    }
}
