/****** Object:  StoredProcedure [dbo].[wcWrkFlw_saveProtectionEvaluationSingle]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcWrkFlw_saveProtectionEvaluationSingle]
GO
/****** Object:  StoredProcedure [dbo].[wcWrkFlw_saveProtectionEvaluationSingle]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[wcWrkFlw_saveProtectionEvaluationSingle]
	@aliasPersonGuid VARCHAR(36)
AS
BEGIN
	DECLARE @entityId INT = (SELECT PersonId FROM PersonAlias WHERE [Guid] = @aliasPersonGuid)
	--DECLARE @entityId INT = 335382
	--DECLARE @Id INT = (SELECT Id FROM DefinedType WHERE [Guid] = '4C052B4C-7341-4E27-A356-A7943EA48B34')
	--SELECT * FROM DefinedValue WHERE DefinedTypeId = @Id
	DECLARE @inProgress VARCHAR(36) = 'E4A8815E-6300-45D0-A07F-068698A39638'
	DECLARE @approved VARCHAR(36) = '74603219-8636-4166-A684-AB7621419723'
	DECLARE @approvedWithRestrictions VARCHAR(36) = '51DCE5F0-4FDF-4A5A-86AD-93E25A899593'
	DECLARE @expired VARCHAR(36) = '9A71A268-5150-4560-A151-C364C21B63BA'
	DECLARE @needsReview VARCHAR(36) = '787DFC12-FB8B-4CEB-B98A-FE8125CBB665'

	DECLARE @now DATE = GETDATE()
	DECLARE @currStatus VARCHAR(36) = (SELECT av.Value FROM AttributeValue av INNER JOIN Attribute a ON av.AttributeId = a.Id WHERE a.[Key] = 'ProtectionStatus' AND av.EntityId = @entityId)
	DECLARE @BCDate datetime
	DECLARE @BCResult nvarchar(max)
	DECLARE @ADate  datetime
	DECLARE @PDate  datetime
	DECLARE @R1Date datetime
	DECLARE @R2Date datetime
	DECLARE @R3Date datetime

	IF @currStatus IN (@approved, @approvedWithRestrictions, @inProgress)
	BEGIN
		DECLARE @attributeId INT = (SELECT av.Id FROM AttributeValue av INNER JOIN Attribute a ON av.AttributeId = a.Id WHERE a.[Key] = 'ProtectionStatus' AND EntityId = @entityId)
		SET @BCDate = (SELECT ValueAsDateTime FROM AttributeValue av INNER JOIN Attribute a ON av.AttributeId = a.Id WHERE a.[Key] = 'BackgroundCheckDate' AND EntityId = @entityId)
		SET @BCResult = (SELECT Value FROM AttributeValue av INNER JOIN Attribute a ON av.AttributeId = a.Id WHERE a.[Key] = 'BackgroundCheckResult' AND EntityId = @entityId)
		SET @ADate  = (SELECT ValueAsDateTime FROM AttributeValue av INNER JOIN Attribute a ON av.AttributeId = a.Id WHERE a.[Key] = 'ProtectionApplicationDate' AND EntityId = @entityId)
		SET @PDate  = (SELECT ValueAsDateTime FROM AttributeValue av INNER JOIN Attribute a ON av.AttributeId = a.Id WHERE a.[Key] = 'PolicyAcknowledgmentDate'  AND EntityId = @entityId)
		SET @R1Date = (SELECT ValueAsDateTime FROM AttributeValue av INNER JOIN Attribute a ON av.AttributeId = a.Id WHERE a.[Key] = 'ProtectionReference1Date'  AND EntityId = @entityId)
		SET @R2Date = (SELECT ValueAsDateTime FROM AttributeValue av INNER JOIN Attribute a ON av.AttributeId = a.Id WHERE a.[Key] = 'ProtectionReference2Date'  AND EntityId = @entityId)
		SET @R3Date = (SELECT ValueAsDateTime FROM AttributeValue av INNER JOIN Attribute a ON av.AttributeId = a.Id WHERE a.[Key] = 'ProtectionReference3Date'  AND EntityId = @entityId)

		DECLARE @isRestricted BIT = 0
		SET @isRestricted = (SELECT TOP 1 1 FROM AttributeValue av INNER JOIN Attribute a on av.AttributeId = a.Id WHERE a.[Key] LIKE '%restriction%' AND EntityTypeId = 15 AND EntityId = @entityId AND (Value IS NOT NULL AND Value != ''))

		IF @isRestricted IS NULL
			SET @isRestricted = 0

--		SELECT @currStatus, @BCDate, @R1Date, @R2Date, @R3Date, @ADate, @PDate
		DECLARE @newStatus VARCHAR(36) = dbo.wcfnWrkFlw_getProtectionEvaluation(@currStatus,@BCDate, @R1Date, @R2Date, @R3Date, @ADate, @PDate, @isRestricted)
		
		---HACK START
		IF @newStatus = @inProgress AND @BCResult = 'Review'
			SET @newStatus = @needsReview
		---HACK END

		UPDATE AttributeValue
		SET Value = @newStatus
		WHERE EntityId = @entityId
		AND Id = @attributeId

		SELECT @newStatus

	END

	--SELECT @currStatus AS 'Current', @newStatus AS 'New', @entityId
	--select top 100 * from AttributeValue order by CreatedDateTime desc
	--select * from attribute where id in (1880,1688)
	--update AttributeValue set Value = CAST(DATEADD(yy,-0,getdate()) AS DATE) where id in (1264720,1264718,1264716)
END



GO
