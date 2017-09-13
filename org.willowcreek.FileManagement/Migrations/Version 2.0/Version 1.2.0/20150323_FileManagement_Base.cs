using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rock.Plugin;

namespace org.willowcreek.FileManagement.Migrations.Version_2._0.Version_1._2._0
{
    [MigrationNumber(1, "1.2")]
    public class _20150323_FileManagement_Base : Migration
    {
        public override void Up()
        {
            CreatePersonDocumentTable();

            // Add the new block type/attributes and attach it to the Person page (93)
            RockMigrationHelper.UpdateBlockType("Person Document List", "Shows a list of all binary files for the Person in context.", "~/Plugins/org_willowcreek/FileManagement/PersonDocumentList.ascx", "org_willowcreek > File Management", "211C5C88-9EFE-41F2-955B-669B5E560D8D");
            //RockMigrationHelper.AddBlock("08DBD8A5-2C35-4146-B4A8-0F7652348B25", "", "211C5C88-9EFE-41F2-955B-669B5E560D8D", "Person Document List", "SectionB3", "", "", 0, "BE30F5E0-2F5A-4B02-BFAC-E9D81EECAB9E");
            RockMigrationHelper.AddBlockTypeAttribute("211C5C88-9EFE-41F2-955B-669B5E560D8D", "09EC7F0D-3505-4090-B010-ABA68CB9B904", "Binary File Type", "BinaryFileType", "", "", 0, @"", "8E9CEA36-7980-4622-98BD-1F00E390FC35");
        }

        public override void Down()
        {
            // It is so much easier to destroy than create..
            RockMigrationHelper.DeleteAttribute("8E9CEA36-7980-4622-98BD-1F00E390FC35");
            //RockMigrationHelper.DeleteBlock("BE30F5E0-2F5A-4B02-BFAC-E9D81EECAB9E"); 
            RockMigrationHelper.DeleteBlockType("211C5C88-9EFE-41F2-955B-669B5E560D8D");
        }

        protected void CreatePersonDocumentTable()
        {
            Sql(@"
CREATE TABLE [dbo].[_org_willowcreek_PersonDocument](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PersonAliasId] [uniqueidentifier] NOT NULL,
	[BinaryFileId] [uniqueidentifier] NOT NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[CreatedByPersonAliasId] [int] NULL,
	[ModifiedByPersonAliasId] [int] NULL,
	[Guid] [uniqueidentifier] NULL,
	[ForeignId] [nvarchar](50) NULL,
 CONSTRAINT [PK__org_willowcreek_PersonDocument] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[_org_willowcreek_PersonDocument]  WITH CHECK ADD  CONSTRAINT [FK__org_willowcreek_PersonDocument_BinaryFile] FOREIGN KEY([BinaryFileId])
REFERENCES [dbo].[BinaryFile] ([Guid])

ALTER TABLE [dbo].[_org_willowcreek_PersonDocument] CHECK CONSTRAINT [FK__org_willowcreek_PersonDocument_BinaryFile]

ALTER TABLE [dbo].[_org_willowcreek_PersonDocument]  WITH CHECK ADD  CONSTRAINT [FK__org_willowcreek_PersonDocument_PersonAlias] FOREIGN KEY([PersonAliasId])
REFERENCES [dbo].[PersonAlias] ([Guid])

ALTER TABLE [dbo].[_org_willowcreek_PersonDocument] CHECK CONSTRAINT [FK__org_willowcreek_PersonDocument_PersonAlias]
");

        }
    }
}
