/****** Object:  StoredProcedure [dbo].[wcUtil_deletePerson]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcUtil_deletePerson]
GO
/****** Object:  StoredProcedure [dbo].[wcUtil_deletePerson]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[wcUtil_deletePerson]
	@sourceID NVARCHAR(36),
	@sourceTargetCD NVARCHAR(3)
AS
BEGIN

	SET XACT_ABORT ON

	IF @sourceID IS NOT NULL AND @sourceTargetCD IS NOT NULL
	BEGIN		
	
		DECLARE @rockAliasId INT;
	 
		IF @sourceTargetCD = 'R' 
			SET @rockAliasId = (SELECT Id FROM PersonAlias where PersonId = CAST(@sourceID AS INT))
		ELSE IF @sourceTargetCD = 'A' OR @sourceTargetCD = 'C'
			SET @rockAliasId = (SELECT Id FROM PersonAlias pa
									JOIN _org_willowcreek_LegacyEntityXRef xr ON xr.SourceID = @sourceID 
																				AND xr.SourceTargetCD = @sourceTargetCD 
																				AND xr.EntityCD = 'P'
									WHERE PersonId = xr.AliasPersonId)
		ELSE 
			SET @rockAliasId = Null

		DECLARE @rockPersonID INT = (SELECT PersonId FROM PersonAlias WHERE Id = @rockAliasId) 

		IF @rockAliasId IS NOT NULL AND @rockPersonID IS NOT NULL
		BEGIN

			BEGIN TRY

				BEGIN TRANSACTION

					delete from PersonDuplicate where PersonAliasId = @rockAliasId OR DuplicatePersonAliasId = @rockAliasId
					delete from PersonViewed where TargetPersonAliasId = @rockAliasId
					--delete from CommunicationRecipient where PersonAliasId = @rockAliasId
					--update FinancialTransaction SET AuthorizedPersonAliasId = 1 where AuthorizedPersonAliasId = @rockAliasId
					--update FinancialScheduledTransaction SET AuthorizedPersonAliasId = 1 where AuthorizedPersonAliasId = @rockAliasId
					--delete from PhoneNumber where CreatedByPersonAliasId = @rockAliasId OR ModifiedByPersonAliasId = @rockAliasId
					--delete from AttributeValue where CreatedByPersonAliasId = @rockAliasId OR ModifiedByPersonAliasId = @rockAliasId
					--update ServiceLog SET CreatedByPersonAliasId = 1, ModifiedByPersonAliasId = 1 where CreatedByPersonAliasId = @rockAliasId OR ModifiedByPersonAliasId = @rockAliasId
					--update Workflow SET CreatedByPersonAliasId = 1, ModifiedByPersonAliasId = 1 where CreatedByPersonAliasId = @rockAliasId OR ModifiedByPersonAliasId = @rockAliasId
					--update History SET CreatedByPersonAliasId = 1, ModifiedByPersonAliasId = 1 where CreatedByPersonAliasId = @rockAliasId OR ModifiedByPersonAliasId = @rockAliasId
					--update ExceptionLog SET CreatedByPersonAliasId = 1, ModifiedByPersonAliasId = 1 where CreatedByPersonAliasId = @rockAliasId OR ModifiedByPersonAliasId = @rockAliasId
					--update [Note] SET  CreatedByPersonAliasId = 1, ModifiedByPersonAliasId = 1 where CreatedByPersonAliasId = @rockAliasId OR ModifiedByPersonAliasId = @rockAliasId
					delete from [PageView] where PersonAliasId = @rockAliasId
					--update [person] SET CreatedByPersonAliasId = 1, ModifiedByPersonAliasId = 1 where (CreatedByPersonAliasId = @rockAliasId OR ModifiedByPersonAliasId = @rockAliasId)
					--update [Attribute] SET CreatedByPersonAliasId = 1, ModifiedByPersonAliasId = 1 where (CreatedByPersonAliasId = @rockAliasId OR ModifiedByPersonAliasId = @rockAliasId)
					--update [WorkflowActionForm] SET CreatedByPersonAliasId = 1, ModifiedByPersonAliasId = 1 where (CreatedByPersonAliasId = @rockAliasId OR ModifiedByPersonAliasId = @rockAliasId)
					--update [WorkflowActionFormAttribute] SET CreatedByPersonAliasId = 1, ModifiedByPersonAliasId = 1 where (CreatedByPersonAliasId = @rockAliasId OR ModifiedByPersonAliasId = @rockAliasId)
					--update [WorkflowActionType] SET CreatedByPersonAliasId = 1, ModifiedByPersonAliasId = 1 where (CreatedByPersonAliasId = @rockAliasId OR ModifiedByPersonAliasId = @rockAliasId)
					--update [WorkflowActivityType] SET CreatedByPersonAliasId = 1, ModifiedByPersonAliasId = 1 where (CreatedByPersonAliasId = @rockAliasId OR ModifiedByPersonAliasId = @rockAliasId)
					
					delete ad from AuditDetail ad JOIN [Audit] a ON ad.AuditId = a.Id AND a.EntityTypeId = 15 WHERE entityid = @rockPersonID  
					delete from [Audit] where EntityTypeId = 15 AND entityid = @rockPersonID

					delete from GroupMember where personid = @rockPersonID --and grouptypeid = 10
					delete from PhoneNumber where personid = @rockPersonID
					delete av from [AttributeValue] av JOIN Attribute a ON av.AttributeId = a.Id AND a.EntityTypeId = 15 WHERE entityid = @rockPersonID  

					delete from personalias where Id = @rockAliasId 
					delete from [person] where id = @rockPersonID

					--delete from _org_willowcreek_LegacyEntityXRef WHERE AliasPersonId = @rockAliasId 
					--												AND SourceTargetCD = @sourceTargetCD 
					--												AND EntityCD = 'P'

				COMMIT

			END TRY
			BEGIN CATCH
				
				SELECT @sourceID, ERROR_LINE(), ERROR_MESSAGE()

				ROLLBACK

			END CATCH

		END

	END

END



GO
