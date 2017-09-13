/****** Object:  StoredProcedure [dbo].[wcWrkFlw_saveProtectionApplicationEvaluation]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcWrkFlw_saveProtectionApplicationEvaluation]
GO
/****** Object:  StoredProcedure [dbo].[wcWrkFlw_saveProtectionApplicationEvaluation]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[wcWrkFlw_saveProtectionApplicationEvaluation]
	@aliasPersonGuid VARCHAR(36)
AS
BEGIN

	DECLARE @now DATETIME = GETDATE()
	DECLARE @personId INT = (SELECT PersonId FROM PersonAlias WHERE [Guid] = @aliasPersonGuid)
	DECLARE @statusId INT = (SELECT Id FROM Attribute WHERE [Key] = 'ProtectionStatus')
	DECLARE @autoApprovedDate DATETIME

	-- If the most recent application has no problems, auto-approve
	SELECT @autoApprovedDate = @now 
	FROM (
		-- Rank = 1 for the most recent application
		SELECT Rank = RANK() OVER (PARTITION BY ApplicantPersonAliasGuid ORDER BY CreatedDateTime DESC), *
		FROM _org_willowcreek_ProtectionApp_Questionnaire
		WHERE ApplicantPersonAliasGuid = @aliasPersonGuid
		) R
	WHERE Rank = 1 
	AND PornographyAddiction = 0
	AND AlcoholAddiction = 0
	AND DrugAddiction = 0
	AND PornographyVulnerable = 0
	AND DcfsInvestigation = 0
	AND OrderOfProtection = 0
	AND CommittedOrAccused = 0
	AND RelationshipVulnerable = 0
	AND AskedToLeave = 0

	IF @autoApprovedDate IS NULL
	BEGIN
		UPDATE AttributeValue
		SET Value = '787DFC12-FB8B-4CEB-B98A-FE8125CBB665'	--Needs Review
		WHERE AttributeId = @statusId
		AND EntityId = @personId
	END
	ELSE
	BEGIN
		UPDATE AttributeValue
		SET Value = 'E4A8815E-6300-45D0-A07F-068698A39638'	--In Progress
		WHERE AttributeId = @statusId
		AND EntityId = @personId

		DECLARE @dateId INT = (SELECT Id FROM Attribute WHERE [Key] = 'ProtectionApplicationDate')
		IF NOT EXISTS (SELECT NULL FROM AttributeValue WHERE AttributeId = @dateId AND EntityId = @personId)
		BEGIN
			INSERT INTO AttributeValue
			(IsSystem, AttributeId, EntityId, [Value], [Guid], CreatedDateTime, ModifiedDateTime, CreatedByPersonAliasId, ModifiedByPersonAliasId)
			VALUES
			(0, @dateId, @personId, CAST(@now AS DATE), NEWID(), @now, @now, 1, 1)
		END
		ELSE
		BEGIN
			UPDATE AttributeValue
			SET [Value] = CAST(@now AS DATE)
			WHERE AttributeId = @dateId 
			AND EntityId = @personId
		END
	END

	--See if application is completed (Not anymore, will happen nightly)
	--EXEC wcWrkFlw_saveProtectionEvaluationSingle @aliaspersonGuid
END
GO
