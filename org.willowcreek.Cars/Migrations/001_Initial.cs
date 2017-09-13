using Rock.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.willowcreek.Cars.Migrations
{
    [MigrationNumber( 1, "1.5.0" )]
    public class Initial : Migration
    {
        public override void Up()
        {
            #region Defined Types
            // add defined types and values
            RockMigrationHelper.UpdateCategory( "6028D502-79F4-4A74-9323-525E90F900C7", "Care Center Cars", "fa fa-car", "", SystemGuid.Category.DEFINEDTYPE_CARS );

            RockMigrationHelper.AddDefinedType( "Care Center Cars", "Vehicle Make", "The make of the vehicle.", SystemGuid.DefinedType.VEHICLE_MAKE );

            RockMigrationHelper.AddDefinedType( "Care Center Cars", "Vehicle Model", "The model of the vehicle.", SystemGuid.DefinedType.VEHICLE_MODEL );
            RockMigrationHelper.AddDefinedTypeAttribute( SystemGuid.DefinedType.VEHICLE_MODEL, Rock.SystemGuid.FieldType.DEFINED_VALUE, "Vehicle Make", "VehicleMake", "Make of the vehicle model.", 0, "", "536edc6a-d137-4385-91ef-7a7260a5417e" );
            Sql( "UPDATE [Attribute] SET [IsGridColumn] = 1 WHERE [Guid] = '536edc6a-d137-4385-91ef-7a7260a5417e' " );
            Sql( @"  DECLARE @AttributeId int = (SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = '536edc6a-d137-4385-91ef-7a7260a5417e')
                  DECLARE @DefinedTypeId int = (SELECT TOP 1 [Id] FROM [DefinedType] WHERE [Guid] = '3a22c655-c179-40f8-bb21-60d3aec603f6')

                  INSERT INTO [AttributeQualifier]
                  ([IsSystem], [AttributeId], [Key], [Value], [Guid])
                  VALUES
                  (0, @AttributeId, 'definedtype', convert(nvarchar(10), @DefinedTypeId), newid())

                    INSERT INTO [AttributeQualifier]
                  ([IsSystem], [AttributeId], [Key], [Value], [Guid])
                  VALUES
                  (0, @AttributeId, 'allowmultiple', 'False', newid())" );

            RockMigrationHelper.AddDefinedType( "Care Center Cars", "Vehicle Color", "The color of the vehicle.", SystemGuid.DefinedType.VEHICLE_COLOR );

            RockMigrationHelper.AddDefinedType( "Care Center Cars", "Vehicle Body Style", "The body style of the vehicle.", SystemGuid.DefinedType.VEHICLE_BODY_STYLE );

            RockMigrationHelper.AddDefinedType( "Care Center Cars", "Disposition Type", "The disposition type for the sale of the vehicle.", SystemGuid.DefinedType.DISPOSITION_TYPE );
            RockMigrationHelper.AddDefinedTypeAttribute( SystemGuid.DefinedType.DISPOSITION_TYPE, Rock.SystemGuid.FieldType.BOOLEAN, "Has Amount Collected", "HasAmountCollected", "If amount should be tracked.", 0, "True", "473f3b09-23b7-4239-8c6e-df51f9a5e750" );
            Sql( "UPDATE [Attribute] SET [IsGridColumn] = 1 WHERE [Guid] = '473f3b09-23b7-4239-8c6e-df51f9a5e750' " );

            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.DISPOSITION_TYPE, "Give Away (Individual)", "Vehicle was given to an individual.", "4d4943fb-c48e-432b-8ec4-6127de7045ee", false );
            RockMigrationHelper.AddDefinedValueAttributeValue( "4d4943fb-c48e-432b-8ec4-6127de7045ee", "473f3b09-23b7-4239-8c6e-df51f9a5e750", "False" );

            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.DISPOSITION_TYPE, "Give Away", "Vehicle was given away.", "89c41eae-1de4-425b-b7f2-a19f45707a08", false );
            RockMigrationHelper.AddDefinedValueAttributeValue( "89c41eae-1de4-425b-b7f2-a19f45707a08", "473f3b09-23b7-4239-8c6e-df51f9a5e750", "False" );

            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.DISPOSITION_TYPE, "Give Away (Staff)", "Vehicle was given to a staff member.", "6dfc9884-b763-4d01-8080-f50211217772", false );
            RockMigrationHelper.AddDefinedValueAttributeValue( "6dfc9884-b763-4d01-8080-f50211217772", "473f3b09-23b7-4239-8c6e-df51f9a5e750", "False" );

            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.DISPOSITION_TYPE, "Sold eBay", "The vehicle was sold on eBay.", "b2860c7e-a2c6-42fa-a105-65e5b1718625", false );

            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.DISPOSITION_TYPE, "Sold Auction", "The vehicle was sold at auction.", "c795e7cd-ce5a-48f6-9bae-430ceb8f1477", false );

            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.DISPOSITION_TYPE, "Sold Salvage", "The vehicle was sold as salvage.", "902a07c0-99af-45bd-8f20-dc369ffc39e0", false );

            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.DISPOSITION_TYPE, "Purchased", "The vehicle was purchased.", "f18397f7-bf7d-488f-8048-da950a692d69", false );

            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.DISPOSITION_TYPE, "Wholesale", "The vehicle was sold wholesale.", "5efcd65b-d1d8-475f-a7a1-8282527cb509", false );

            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.DISPOSITION_TYPE, "Consignment", "The vehicle was sold on consignment.", "d4fd9623-421a-467a-91ce-ddb6576add77", false );
            RockMigrationHelper.AddDefinedValueAttributeValue( "d4fd9623-421a-467a-91ce-ddb6576add77", "473f3b09-23b7-4239-8c6e-df51f9a5e750", "False" );

            RockMigrationHelper.AddDefinedType( "Care Center Cars", "Disposition Payment Type", "The disposition payment type for the vehicle.", SystemGuid.DefinedType.DISPOSITION_PAYMENT_TYPE );

            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.DISPOSITION_PAYMENT_TYPE, "Cash", "Disposition payment of cash", "bbe4ffef-017f-4989-baa9-9ce04c3eeabe", false );
            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.DISPOSITION_PAYMENT_TYPE, "Check", "Disposition payment of check", "49fef478-01f6-407a-8892-f59b9dfaae82", false );
            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.DISPOSITION_PAYMENT_TYPE, "Credit", "Disposition payment of credit", "c1149a25-34eb-4201-bc88-d47bb3f3aa9d", false );

            #endregion

            AddVehicleTables();
        }

        #region Tables and Indexes

        public void AddVehicleTables()
        {
            Sql( @"
    CREATE FUNCTION [dbo].[UDF_GetVehicleStockNumber] (@Id int,@Status int)
    RETURNS nvarchar(25)
    AS
    BEGIN
    declare @stock nvarchar(25);
    if(@Status=0)
    begin
    set @stock='T'+ Convert(varchar,@Id);
    end
    else
    begin
    set @stock='P'+ Convert(varchar,@Id);
    end
    return @stock;
    END" );
            Sql( @" CREATE TABLE [dbo].[_org_willowcreek_Cars_Vehicle](
	[Id] [int] NOT NULL identity(1,1),
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
	[StockNumber] AS ([dbo].UDF_GetVehicleStockNumber (Id,Status)),
	[PhotoId] [int] NULL,
	[DispositionTypeId] [int] NULL,
	[DispositionAmount] [decimal](18, 2) NULL,
	[DispositionpaymentTypeValueId] [int] NULL,
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


ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.BinaryFile_PhotoId] FOREIGN KEY([PhotoId])
REFERENCES [dbo].[BinaryFile] ([Id])

ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle] CHECK CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.BinaryFile_PhotoId]


ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.BinaryFile_TitleFileId] FOREIGN KEY([TitleFileId])
REFERENCES [dbo].[BinaryFile] ([Id])


ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle] CHECK CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.BinaryFile_TitleFileId]


ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.DefinedValue_BodyStyleValueId] FOREIGN KEY([BodyStyleValueId])
REFERENCES [dbo].[DefinedValue] ([Id])


ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle] CHECK CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.DefinedValue_BodyStyleValueId]


ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.DefinedValue_ColorValueId] FOREIGN KEY([ColorValueId])
REFERENCES [dbo].[DefinedValue] ([Id])


ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle] CHECK CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.DefinedValue_ColorValueId]


ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.DefinedValue_DispositionpaymentTypeValueId] FOREIGN KEY([DispositionpaymentTypeValueId])
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
      
    CREATE UNIQUE INDEX [IX_Guid] ON [dbo].[_org_willowcreek_Cars_Vehicle]([Guid])
    CREATE INDEX [IX_CreatedByPersonAliasId] ON [dbo].[_org_willowcreek_Cars_Vehicle]([CreatedByPersonAliasId])
    CREATE INDEX [IX_ModifiedByPersonAliasId] ON [dbo].[_org_willowcreek_Cars_Vehicle]([ModifiedByPersonAliasId])

" );

        }

        #endregion

        public override void Down()
        {

        }

    }
}
