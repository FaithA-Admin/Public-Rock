﻿// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
namespace Rock.Plugin.HotFixes
{
    /// <summary>
    /// 
    /// </summary>
    [MigrationNumber( 15, "1.6.0" )]
    public class CheckinByBirthdate : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
// Moved to core migration: 201612132013351_WorkflowTypeText
//
//            RockMigrationHelper.UpdateEntityAttribute( "Rock.Model.Group", "9C7D431C-875C-4792-9E76-93F3A32BB850", "GroupTypeId", "", "Birthdate Range", "The birth date range allowed to check in to these group types.", 0, "", "F1A43EAB-D682-403F-A05E-CCFFBF879F32" );

//            Sql( @"
//    DECLARE @GroupTypeId int = ( SELECT TOP 1 [Id] FROM [GroupType] WHERE [Guid] = '0572A5FE-20A4-4BF1-95CD-C71DB5281392' )
//    DECLARE @AttributeId int = ( SELECT TOP 1 [Id] FROM [Attribute] WHERE [Guid] = 'F1A43EAB-D682-403F-A05E-CCFFBF879F32' )
//    DECLARE @CategoryId int = ( SELECT TOP 1 [Id] FROM [Category] WHERE [Guid] = 'C8E0FD8D-3032-4ACD-9DB9-FF70B11D6BCC' )

//    IF @GroupTypeId IS NOT NULL AND @AttributeId IS NOT NULL AND @CategoryId IS NOT NULL
//    BEGIN
//        UPDATE [Attribute] SET [EntityTypeQualifierValue] = CAST(@GroupTypeId AS VARCHAR) WHERE [Id] = @AttributeId
//        IF NOT EXISTS ( SELECT * FROM [AttributeCategory] WHERE [AttributeId] = @AttributeId AND [CategoryId] = @CategoryId )
//        BEGIN
//            INSERT INTO [AttributeCategory] ( [AttributeId], [CategoryId] )
//            VALUES ( @AttributeId, @CategoryId )
//        END
//    END
//" );
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
        }
    }
}
