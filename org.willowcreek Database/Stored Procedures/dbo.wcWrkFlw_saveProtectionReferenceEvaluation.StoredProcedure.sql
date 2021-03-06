/****** Object:  StoredProcedure [dbo].[wcWrkFlw_saveProtectionReferenceEvaluation]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcWrkFlw_saveProtectionReferenceEvaluation]
GO
/****** Object:  StoredProcedure [dbo].[wcWrkFlw_saveProtectionReferenceEvaluation]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[wcWrkFlw_saveProtectionReferenceEvaluation]
	@aliasPersonGuid VARCHAR(36),
	@workflowId INT
AS
BEGIN

	DECLARE @now DATETIME = GETDATE()
	DECLARE @personId INT = (SELECT PersonId FROM PersonAlias WHERE [Guid] = @aliasPersonGuid)

	--TH switched column name to proper name and used guid instead of personid
	--DECLARE @questionnaireId INT = (SELECT TOP 1 Id FROM _org_willowcreek_ProtectionApp_Questionnaire WHERE ApplicantPersonAliasGuid = @aliasPersonGuid ORDER BY CreatedDateTime DESC)

	DECLARE @workflowGUID VARCHAR(36) = (SELECT [GUID] FROM Workflow WHERE Id = @workflowId)

	DECLARE @receivedCount INT

	SELECT @receivedCount = COUNT(*)
	FROM _org_willowcreek_ProtectionApp_Reference
	WHERE WorkflowId = @workflowGUID
	--WHERE QuestionnaireId = @questionnaireId

	DECLARE @approvedCount INT

	SELECT @approvedCount = COUNT(*)
	FROM _org_willowcreek_ProtectionApp_Reference
	--WHERE QuestionnaireId = @questionnaireId
	WHERE WorkflowId = @workflowGUID
	--AND JuniorHighStudent = 0
	AND KnownMoreThanOneYear = 1
	AND IsReference18 = 1
	--AND NatureOfRelationship <> 'Friend'
	AND NatureOfRelationship <> 'Family'
	AND MaintainRelationships = 1
	AND RespectHealthyRelationalBoundaries = 0
	AND CriminalOffenses = 0
	AND ManipulativeBehavior = 0
	AND InflictedEmotionalHarm = 0
	AND TrustInChildCare = 1
	AND WouldRecommend = 1

	DECLARE @statusId INT = (SELECT Id FROM Attribute WHERE [Key] = 'ProtectionStatus')

	IF @approvedCount >= 2
	BEGIN
		IF @receivedCount > @approvedCount
		BEGIN
			UPDATE AttributeValue
			SET Value = '787DFC12-FB8B-4CEB-B98A-FE8125CBB665'	--Needs Review
			WHERE AttributeId = @statusId
			AND EntityId = @personId
		END
		ELSE
		BEGIN
			DECLARE @date1Id INT = (SELECT Id FROM Attribute WHERE [Key] = 'ProtectionReference1Date')
			DECLARE @date2Id INT = (SELECT Id FROM Attribute WHERE [Key] = 'ProtectionReference2Date')
			DECLARE @date3Id INT = (SELECT Id FROM Attribute WHERE [Key] = 'ProtectionReference3Date')
			/*
			IF NOT EXISTS (SELECT NULL FROM AttributeValue WHERE AttributeId = @date1Id AND EntityId = @personId)
			BEGIN
				INSERT INTO AttributeValue
				(IsSystem, AttributeId, EntityId, [Value], [Guid], CreatedDateTime, ModifiedDateTime, CreatedByPersonAliasId, ModifiedByPersonAliasId)
				VALUES
				(0, @date1Id, @personId, CAST(@now AS DATE), NEWID(), @now, @now, 1, 1)
				INSERT INTO AttributeValue
				(IsSystem, AttributeId, EntityId, [Value], [Guid], CreatedDateTime, ModifiedDateTime, CreatedByPersonAliasId, ModifiedByPersonAliasId)
				VALUES
				(0, @date2Id, @personId, CAST(@now AS DATE), NEWID(), @now, @now, 1, 1)
				INSERT INTO AttributeValue
				(IsSystem, AttributeId, EntityId, [Value], [Guid], CreatedDateTime, ModifiedDateTime, CreatedByPersonAliasId, ModifiedByPersonAliasId)
				VALUES
				(0, @date3Id, @personId, CAST(@now AS DATE), NEWID(), @now, @now, 1, 1)
			END
			ELSE
			BEGIN
				UPDATE AttributeValue
				SET [Value] = CAST(@now AS DATE)
				WHERE AttributeId IN (@date1Id,@date2Id,@date3Id) 
				AND EntityId = @personId
			END
			*/
		END
	END
	ELSE
	BEGIN
		IF @receivedCount = 3
		BEGIN
			UPDATE AttributeValue
			SET Value = '787DFC12-FB8B-4CEB-B98A-FE8125CBB665'	--Needs Review
			WHERE AttributeId = @statusId
			AND EntityId = @personId
		END
	END

END

GO
