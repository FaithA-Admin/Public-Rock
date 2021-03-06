/****** Object:  StoredProcedure [dbo].[wcUtil_saveAttributeValue]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcUtil_saveAttributeValue]
GO
/****** Object:  StoredProcedure [dbo].[wcUtil_saveAttributeValue]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[wcUtil_saveAttributeValue]
	@key NVARCHAR(200),
	@entityType NVARCHAR(100),
	@entityId INT,
	@value NVARCHAR(MAX)
AS
BEGIN

	DECLARE @entityTypeId INT = (SELECT Id FROM EntityType WHERE FriendlyName = @entityType)
	DECLARE @attributeId INT = (SELECT Id FROM Attribute WHERE [key] = @key AND EntityTypeId = @entityTypeId)
	DECLARE @attributeValueId INT = (SELECT Id FROM AttributeValue WHERE AttributeId = @attributeId AND EntityId = @entityId)

	IF @entityTypeId IS NOT NULL AND @attributeId IS NOT NULL
		IF @attributeValueId IS NULL
			INSERT INTO AttributeValue
			(IsSystem,AttributeId,EntityId,Value,[Guid],CreatedDateTime,ModifiedDateTime,CreatedByPersonAliasId,ModifiedByPersonAliasId)
			VALUES
			(0,@attributeId,@entityId,@value,NEWID(),GETDATE(),GETDATE(),1,1)
		ELSE
			UPDATE AttributeValue
			SET Value = @value
			WHERE Id = @attributeValueId

END
GO
