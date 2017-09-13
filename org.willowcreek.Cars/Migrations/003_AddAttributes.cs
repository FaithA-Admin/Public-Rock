using Rock.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.willowcreek.Cars.Migrations
{
    [MigrationNumber( 3, "1.5.0" )]
    public class AddAttributes : Migration
    {
        public override void Up()
        {
            RockMigrationHelper.UpdateFieldType( "SSN", "Social Security Number Field Type", "Rock", "Rock.Field.Types.SSNFieldType", Rock.SystemGuid.FieldType.SSN );

            RockMigrationHelper.UpdatePersonAttributeCategory( "Care Center Cars Attributes", "fa fa-car", "Used for attributes having to do with car donations", SystemGuid.Category.PERSON_CARECENTER_CARS_ATTRIBUTES );
            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.SSN, SystemGuid.Category.PERSON_CARECENTER_CARS_ATTRIBUTES, "SSN", "core_SSN", "", "Social Security Number (SSN)", 0, "", SystemGuid.Attribute.PERSON_SSN );
            RockMigrationHelper.UpdatePersonAttribute( Rock.SystemGuid.FieldType.TEXT, SystemGuid.Category.PERSON_CARECENTER_CARS_ATTRIBUTES, "EIN", "core_EIN", "", "Employer Identification Number (EIN)", 0, "", SystemGuid.Attribute.BUSINESS_EIN );

            RockMigrationHelper.AddDefinedType( "Care Center Cars", "Vehicle Sub-Status", "The sub status of a donated vehicle.", SystemGuid.DefinedType.VEHICLE_SUB_STATUS );
            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.VEHICLE_SUB_STATUS, "Pick-up Scheduled", "", "36F3BB8D-618A-46B7-B9E9-E486E4A549EC", false );
            RockMigrationHelper.UpdateDefinedValue( SystemGuid.DefinedType.VEHICLE_SUB_STATUS, "Contacted Guest", "", "822E6E82-B5AC-40EE-8BBD-233DB5874107", false );

            RockMigrationHelper.UpdateEntityType( "org.willowcreek.Cars.Model.Vehicle", "B88DDC59-AB04-421E-82B2-2533CC0284E3", true, true );
            Sql( @"
    DECLARE @NoteEntityTypeId int = ( SELECT TOP 1 [Id] FROM [EntityType] WHERE [Guid] = 'B88DDC59-AB04-421E-82B2-2533CC0284E3' )
    IF @NoteEntityTypeId IS NOT NULL AND NOT EXISTS ( SELECT [Id] FROM [NoteType] WHERE [Guid] = '7EF7435F-D3F0-484F-B4F1-761F909994BD' )
    BEGIN
	    INSERT INTO [NoteType] ([IsSystem], [EntityTypeId], [Name], [Guid], [UserSelectable], [IconCssClass], [EntityTypeQualifierColumn], [EntityTypeQualifierValue])
	    VALUES (1, @NoteEntityTypeId, 'Vehicle Donation Notes', '7EF7435F-D3F0-484F-B4F1-761F909994BD', 1, 'fa fa-comment', '', '')
    END
" );
            Sql( @"
    ALTER TABLE [_org_willowcreek_Cars_Vehicle] ADD 
        [RecipientPersonAliasId] int NULL,
        [SubStatusValueId] int NULL;
" );
            Sql( @"
ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.PersonAlias_RecipientPersonAliasId] FOREIGN KEY([RecipientPersonAliasId])
REFERENCES [dbo].[PersonAlias] ([Id])
ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle] CHECK CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.PersonAlias_RecipientPersonAliasId]

ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle]  WITH CHECK ADD  CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.DefinedValue_SubStatusValueId] FOREIGN KEY([SubStatusValueId])
REFERENCES [dbo].[DefinedValue] ([Id])
ALTER TABLE [dbo].[_org_willowcreek_Cars_Vehicle] CHECK CONSTRAINT [FK_dbo._org_willowcreek_Cars_Vehicle_dbo.DefinedValue_SubStatusValueId]
" );



        }

        public override void Down()
        {
        }

    }
}
