using Rock.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.willowcreek.Cars.Migrations
{
    [MigrationNumber( 2, "1.5.0" )]
    public class AddAddlPhotoFields : Migration
    {
        public override void Up()
        {
            Sql( @"
    DROP TABLE[dbo].[_org_willowcreek_Cars_Vehicle]
    CREATE TABLE [dbo].[_org_willowcreek_Cars_Vehicle](
	    [Id] [int] IDENTITY(1,1) NOT NULL,
	    [DonorPersonAliasId] [int] NOT NULL,
	    [DonorType] [int] NOT NULL,
	    [PickUpLocationId] [int] NULL,
	    [DateEntered] [datetime] NOT NULL,
	    [DateInInventory] [datetime] NULL,
	    [DateCompleted] [datetime] NULL,
	    [MakeValueId] [int] NULL,
	    [ModelValueId] [int] NULL,
	    [Year] [int] NULL,
	    [Mileage] [int] NULL,
	    [ColorValueId] [int] NULL,
	    [BodyStyleValueId] [int] NULL,
	    [Vin] [nvarchar](25) NULL,
	    [Note] [nvarchar](max) NULL,
	    [AssessedValueType] [int] NULL,
	    [Tax1098Summary] [nvarchar](30) NULL,
	    [Condition] [int] NULL,
	    [Status] [int] NULL,
	    [TStockNumber] [int] NOT NULL,
	    [PStockNumber] [int] NULL,
	    [Photo1Id] [int] NULL,
	    [Photo2Id] [int] NULL,
	    [Photo3Id] [int] NULL,
	    [Photo4Id] [int] NULL,
	    [DispositionTypeId] [int] NULL,
	    [DispositionAmount] [decimal](18, 2) NULL,
	    [DispositionPaymentTypeValueId] [int] NULL,
	    [DispositionNote] [nvarchar](max) NULL,
	    [TitleFileId] [int] NULL,
	    [LastDonarLetterSendDate] [datetime] NULL,
	    [LastSoldLetterSendDate] [datetime] NULL,
	    [CreatedDateTime] [datetime] NULL,
	    [ModifiedDateTime] [datetime] NULL,
	    [CreatedByPersonAliasId] [int] NULL,
	    [ModifiedByPersonAliasId] [int] NULL,
	    [Guid] [uniqueidentifier] NOT NULL,
	    [ForeignKey] [nvarchar](100) NULL,
	    [ForeignGuid] [uniqueidentifier] NULL,
	    [ForeignId] [uniqueidentifier] NULL,
     CONSTRAINT [PK__org_willowcreek_Cars_Vehicle] PRIMARY KEY CLUSTERED 
    (
	    [Id] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.BinaryFile_Photo1Id] FOREIGN KEY([Photo1Id])
    REFERENCES [dbo].[BinaryFile] ([Id])
    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle] CHECK CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.BinaryFile_Photo1Id]

    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.BinaryFile_Photo2Id] FOREIGN KEY([Photo2Id])
    REFERENCES [dbo].[BinaryFile] ([Id])
    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle] CHECK CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.BinaryFile_Photo2Id]

    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.BinaryFile_Photo3Id] FOREIGN KEY([Photo3Id])
    REFERENCES [dbo].[BinaryFile] ([Id])
    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle] CHECK CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.BinaryFile_Photo3Id]

    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.BinaryFile_Photo4Id] FOREIGN KEY([Photo4Id])
    REFERENCES [dbo].[BinaryFile] ([Id])
    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle] CHECK CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.BinaryFile_Photo4Id]

    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.BinaryFile_TitleFileId] FOREIGN KEY([TitleFileId])
    REFERENCES [dbo].[BinaryFile] ([Id])
    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle] CHECK CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.BinaryFile_TitleFileId]

    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.DefinedValue_BodyStyleValueId] FOREIGN KEY([BodyStyleValueId])
    REFERENCES [dbo].[DefinedValue] ([Id])
    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle] CHECK CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.DefinedValue_BodyStyleValueId]

    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.DefinedValue_ColorValueId] FOREIGN KEY([ColorValueId])
    REFERENCES [dbo].[DefinedValue] ([Id])
    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle] CHECK CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.DefinedValue_ColorValueId]

    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.DefinedValue_DispositionpaymentTypeValueId] FOREIGN KEY([DispositionPaymentTypeValueId])
    REFERENCES [dbo].[DefinedValue] ([Id])
    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle] CHECK CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.DefinedValue_DispositionpaymentTypeValueId]

    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.DefinedValue_DispositionTypeId] FOREIGN KEY([DispositionTypeId])
    REFERENCES [dbo].[DefinedValue] ([Id])
    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle] CHECK CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.DefinedValue_DispositionTypeId]

    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.DefinedValue_MakeValueId] FOREIGN KEY([MakeValueId])
    REFERENCES [dbo].[DefinedValue] ([Id])
    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle] CHECK CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.DefinedValue_MakeValueId]

    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.DefinedValue_ModelValueId] FOREIGN KEY([ModelValueId])
    REFERENCES [dbo].[DefinedValue] ([Id])
    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle] CHECK CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.DefinedValue_ModelValueId]

    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.Location_PickUpLocationId] FOREIGN KEY([PickUpLocationId])
    REFERENCES [dbo].[Location] ([Id])
    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle] CHECK CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.Location_PickUpLocationId]

    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.PersonAlias_CreatedByPersonAliasId] FOREIGN KEY([CreatedByPersonAliasId])
    REFERENCES [dbo].[PersonAlias] ([Id])
    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle] CHECK CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.PersonAlias_CreatedByPersonAliasId]

    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.PersonAlias_DonorPersonAliasId] FOREIGN KEY([DonorPersonAliasId])
    REFERENCES [dbo].[PersonAlias] ([Id])
    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle] CHECK CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.PersonAlias_DonorPersonAliasId]

    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.PersonAlias_ModifiedByPersonAliasId] FOREIGN KEY([ModifiedByPersonAliasId])
    REFERENCES [dbo].[PersonAlias] ([Id])
    ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle] CHECK CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.PersonAlias_ModifiedByPersonAliasId]

    IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UDF_GetVehicleStockNumber]') AND type = 'FN')
    DROP FUNCTION [dbo].[UDF_GetVehicleStockNumber]
" );
        }

        public override void Down()
        {
        }

    }
}
