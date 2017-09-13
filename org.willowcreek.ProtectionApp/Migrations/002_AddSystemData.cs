using System;
using System.Linq;
using Rock.Plugin;

namespace org.willowcreek.ProtectionApp.Migrations
{
    [MigrationNumber(2, "1.2")]
    public class AddSystemData : Migration
    {
        /// <summary>
        /// The commands to run to migrate plugin to the specific version
        /// </summary>
        public override void Up()
        {
            //create the api records
            BuildApi();
            //add marital status defined values
            UpdateMaritalStatus();
        }
 
        private void BuildApi()
        {
            //create the api records
            Sql(@"/*
                    Create the rest controller
                     - make sure to go to Admin/Security/RestControllers and allow view/edit for All Authenticated Users
	                    otherwise only admins can utilize the service api
                    */
                    INSERT INTO dbo.RestController
                            ( Name ,
                              ClassName ,
                              CreatedDateTime ,
                              ModifiedDateTime ,
                              Guid ,
                              ForeignId
                            )
                    VALUES  ( N'Protection App Questionnaire' , -- Name - nvarchar(100)
                              N'org.willowcreek.ProtectionApp.Rest.QuestionnaireController' , -- ClassName - nvarchar(500)
                              GETDATE() , -- CreatedDateTime - datetime
                              GETDATE() , -- ModifiedDateTime - datetime
                              NEWID() , -- Guid - uniqueidentifier
                              N''  -- ForeignId - nvarchar(50)
                            )
                    DECLARE @controllerId INT = SCOPE_IDENTITY()
                    /*Create the rest actions*/
                    --GET
                    INSERT INTO dbo.RestAction
                            ( ControllerId ,
                              Method ,
                              ApiId ,
                              Path ,
                              CreatedDateTime ,
                              ModifiedDateTime ,
                              Guid ,
                              ForeignId
                            )
                    VALUES  ( @controllerId , -- ControllerId - int
                              N'GET' , -- Method - nvarchar(100)
                              N'GETapi/questionnaire' , -- ApiId - nvarchar(2000)
                              N'api/questionnaire' , -- Path - nvarchar(2000)
                              GETDATE() , -- CreatedDateTime - datetime
                              GETDATE() , -- ModifiedDateTime - datetime
                              NEWID() , -- Guid - uniqueidentifier
                              N''  -- ForeignId - nvarchar(50)
                            )
                    --POST
                    INSERT INTO dbo.RestAction
                            ( ControllerId ,
                              Method ,
                              ApiId ,
                              Path ,
                              CreatedDateTime ,
                              ModifiedDateTime ,
                              Guid ,
                              ForeignId
                            )
                    VALUES  ( @controllerId , -- ControllerId - int
                              N'POST' , -- Method - nvarchar(100)
                              N'POSTapi/questionnaire' , -- ApiId - nvarchar(2000)
                              N'api/questionnaire' , -- Path - nvarchar(2000)
                              GETDATE() , -- CreatedDateTime - datetime
                              GETDATE() , -- ModifiedDateTime - datetime
                              NEWID() , -- Guid - uniqueidentifier
                              N''  -- ForeignId - nvarchar(50)
                            )");
            //Reference Controller
            Sql(@"/*
                    Create the rest controller
                     - make sure to go to Admin/Security/RestControllers and allow view/edit for All Authenticated Users
	                    otherwise only admins can utilize the service api
                    */
                    INSERT INTO dbo.RestController
                            ( Name ,
                              ClassName ,
                              CreatedDateTime ,
                              ModifiedDateTime ,
                              Guid ,
                              ForeignId
                            )
                    VALUES  ( N'Protection App Reference' , -- Name - nvarchar(100)
                              N'org.willowcreek.ProtectionApp.Rest.ReferenceController' , -- ClassName - nvarchar(500)
                              GETDATE() , -- CreatedDateTime - datetime
                              GETDATE() , -- ModifiedDateTime - datetime
                              NEWID() , -- Guid - uniqueidentifier
                              N''  -- ForeignId - nvarchar(50)
                            )
                    DECLARE @controllerId INT = SCOPE_IDENTITY()
                    /*Create the rest actions*/
                    --GET
                    INSERT INTO dbo.RestAction
                            ( ControllerId ,
                              Method ,
                              ApiId ,
                              Path ,
                              CreatedDateTime ,
                              ModifiedDateTime ,
                              Guid ,
                              ForeignId
                            )
                    VALUES  ( @controllerId , -- ControllerId - int
                              N'GET' , -- Method - nvarchar(100)
                              N'GETapi/reference' , -- ApiId - nvarchar(2000)
                              N'api/reference' , -- Path - nvarchar(2000)
                              GETDATE() , -- CreatedDateTime - datetime
                              GETDATE() , -- ModifiedDateTime - datetime
                              NEWID() , -- Guid - uniqueidentifier
                              N''  -- ForeignId - nvarchar(50)
                            )
                    --POST
                    INSERT INTO dbo.RestAction
                            ( ControllerId ,
                              Method ,
                              ApiId ,
                              Path ,
                              CreatedDateTime ,
                              ModifiedDateTime ,
                              Guid ,
                              ForeignId
                            )
                    VALUES  ( @controllerId , -- ControllerId - int
                              N'POST' , -- Method - nvarchar(100)
                              N'POSTapi/reference' , -- ApiId - nvarchar(2000)
                              N'api/reference' , -- Path - nvarchar(2000)
                              GETDATE() , -- CreatedDateTime - datetime
                              GETDATE() , -- ModifiedDateTime - datetime
                              NEWID() , -- Guid - uniqueidentifier
                              N''  -- ForeignId - nvarchar(50)
                            )");
        }
 
        private void UpdateMaritalStatus()
        {
            //update unknown status to order 6
            Sql(@"UPDATE dbo.DefinedValue SET [Order] = 6 WHERE [Guid]='d9cfd343-6a56-45f6-9e26-3269ba4fbc02'");

            #region Defined Types
            RockMigrationHelper.AddDefinedValue("B4B92C3F-A935-40E1-A00B-BA484EAD613B", "Separated", "Used with an individual is separated.", "67179C69-E03F-4C67-A5B1-4D4058D741DA", false);
            RockMigrationHelper.AddDefinedValue("B4B92C3F-A935-40E1-A00B-BA484EAD613B", "Divorced", "Used with an individual is divorced.", "ACC0B1D1-891D-4B53-AA74-3A1060ECCE97", false);
            RockMigrationHelper.AddDefinedValue("B4B92C3F-A935-40E1-A00B-BA484EAD613B", "Widowed", "Used with an individual is widowed.", "59FF0D5A-2CE4-4AD6-A950-AF2293DB50DE", false);
            #endregion
        }

        /// <summary>
        /// The commands to undo a migration from a specific version
        /// </summary>
        public override void Down()
        {
            #region Defined Types
            RockMigrationHelper.DeleteDefinedValue("67179C69-E03F-4C67-A5B1-4D4058D741DA");
            RockMigrationHelper.DeleteDefinedValue("ACC0B1D1-891D-4B53-AA74-3A1060ECCE97");
            RockMigrationHelper.DeleteDefinedValue("59FF0D5A-2CE4-4AD6-A950-AF2293DB50DE");
            #endregion

            #region New Person Attributes
            #endregion

            #region Bio block attribute removal
            // As far as I can tell, there is no single item removal delete for a multi-value attribute
            #endregion
        }
    }
}
