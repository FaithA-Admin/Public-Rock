/****** Object:  StoredProcedure [dbo].[wcWrkFlw_saveProtectionReferenceEvaluationSingle]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcWrkFlw_saveProtectionReferenceEvaluationSingle]
GO
/****** Object:  StoredProcedure [dbo].[wcWrkFlw_saveProtectionReferenceEvaluationSingle]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[wcWrkFlw_saveProtectionReferenceEvaluationSingle]
	@aliasPersonGuid VARCHAR(36),
	@currentReference INT
AS
BEGIN
	DECLARE @now DATETIME = GETDATE()
	DECLARE @personId INT = (SELECT PersonId FROM PersonAlias WHERE [Guid] = @aliasPersonGuid)

	--TH switched column to proper name and logic to use guid instead of id
	DECLARE @questionnaireId INT = (SELECT TOP 1 Id FROM _org_willowcreek_ProtectionApp_Questionnaire WHERE ApplicantPersonAliasGuid = @aliasPersonGuid ORDER BY CreatedDateTime DESC)

	DECLARE @refDateAttr INT
	DECLARE @receivedCount INT

	SELECT @receivedCount = COUNT(*)
	FROM _org_willowcreek_ProtectionApp_Reference
	WHERE QuestionnaireId = @questionnaireId
	and SubmissionDate is not null

	DECLARE @approvedCount INT

	SELECT @approvedCount = COUNT(*)
	FROM _org_willowcreek_ProtectionApp_Reference
	WHERE QuestionnaireId = @questionnaireId
	AND SubmissionDate is not null
	--AND JuniorHighStudent = 0
	AND KnownMoreThanOneYear = 1
	AND IsReference18 = 1
	AND NatureOfRelationship <> 'Family'
	AND MaintainRelationships = 1
	AND RespectHealthyRelationalBoundaries = 0
	AND CriminalOffenses = 0
	AND ManipulativeBehavior = 0
	AND InflictedEmotionalHarm = 0
	AND TrustInChildCare = 1
	AND WouldRecommend = 1

	DECLARE @statusId INT = (SELECT Id FROM Attribute WHERE [Key] = 'ProtectionStatus')

	--If there is any non-auto approved references
	IF @receivedCount > @approvedCount
	BEGIN
		--set to needs review
		UPDATE AttributeValue
		SET Value = '787DFC12-FB8B-4CEB-B98A-FE8125CBB665'	--Needs Review
		WHERE AttributeId = @statusId
		AND EntityId = @personId
	END
	ELSE
	BEGIN
		DECLARE @dateId INT
		SET @dateId = 
			CASE @currentReference
				WHEN 1 THEN (SELECT Id FROM Attribute WHERE [Key] = 'ProtectionReference1Date')
				WHEN 2 THEN (SELECT Id FROM Attribute WHERE [Key] = 'ProtectionReference2Date')
				WHEN 3 THEN (SELECT Id FROM Attribute WHERE [Key] = 'ProtectionReference3Date')
			END
		SELECT @refDateAttr = AttributeId FROM AttributeValue WHERE EntityId = @personId AND AttributeId = @dateId

		--IF NOT EXISTS (SELECT NULL FROM AttributeValue WHERE AttributeId IN (@date1Id, @date2Id, @date3Id) AND EntityId = @personId)
		--If this is the first reference to complete the form
		IF @refDateAttr IS NULL
		BEGIN
			--Add all records into the system with null value 
			INSERT INTO AttributeValue
			(IsSystem, AttributeId, EntityId, [Value], [Guid], CreatedDateTime, ModifiedDateTime, CreatedByPersonAliasId, ModifiedByPersonAliasId)
			VALUES
			(0, @dateId, @personId, CAST(@now AS DATE), NEWID(), @now, @now, 1, 1)
		END
		ELSE
		BEGIN
			--Update the current reference complete date to today
			UPDATE AttributeValue
			SET [Value] = CAST(@now AS DATE)
			WHERE EntityId = @personId 
			AND AttributeId = @dateId
		END
	END

	--See if application is completed
	EXEC wcWrkFlw_saveProtectionEvaluationSingle @aliaspersonGuid
END


GO
