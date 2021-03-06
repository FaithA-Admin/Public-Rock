/****** Object:  StoredProcedure [dbo].[wcWrkFlw_setInProgressProtectionStatus]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcWrkFlw_setInProgressProtectionStatus]
GO
/****** Object:  StoredProcedure [dbo].[wcWrkFlw_setInProgressProtectionStatus]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[wcWrkFlw_setInProgressProtectionStatus]
	@aliasPersonGuid VARCHAR(36)
AS
BEGIN

	DECLARE @personId INT
	declare @personAliasID int
	SELECT @personId = PersonId, @personAliasId = id FROM PersonAlias WHERE [Guid] = @aliasPersonGuid
	DECLARE @statusId INT = (SELECT Id FROM Attribute WHERE [Key] = 'ProtectionStatus')
	DECLARE @currStatus NVARCHAR(MAX)
	DECLARE @history NVARCHAR(MAX)

	SELECT TOP 1 @currStatus = Value
	FROM AttributeValue
	WHERE AttributeId = @statusId
	AND EntityId = @personId

	IF isnull(@currStatus, '') = '' OR @currStatus = 'd6751337-e6a8-4c32-9b40-270c50a561ea' --Process Initiated
	BEGIN
		UPDATE AttributeValue
		SET Value = 'E4A8815E-6300-45D0-A07F-068698A39638'	--In Progress
		WHERE AttributeId = @statusId
		AND EntityId = @personId

		SELECT @@ROWCOUNT

		if isnull(@currStatus, '') = '' BEGIN
			set @history = 'Added <span class=''field-name''>Protection Status</span> value of <span class=''field-value''>In Progress</span>.'
		END
		ELSE BEGIN
			set @history = 'Modified <span class=''field-name''>Protection Status</span> value from <span class=''field-value''>Process Initiated</span> to <span class=''field-value''>In Progress</span>.'
		END

		insert into history (IsSystem, CategoryId, EntityTypeId, EntityId, Summary, Guid, CreatedDateTime, ModifiedDateTime, CreatedByPersonAliasId, ModifiedByPersonAliasId)
		values (0, 136, 15, @personId, @history, newid(), GETDATE(), GETDATE(), @personAliasId, @personAliasId)
	END
END
GO
