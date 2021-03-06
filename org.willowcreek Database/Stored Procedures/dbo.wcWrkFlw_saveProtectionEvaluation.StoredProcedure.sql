/****** Object:  StoredProcedure [dbo].[wcWrkFlw_saveProtectionEvaluation]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcWrkFlw_saveProtectionEvaluation]
GO
/****** Object:  StoredProcedure [dbo].[wcWrkFlw_saveProtectionEvaluation]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[wcWrkFlw_saveProtectionEvaluation]
AS
BEGIN

	--DECLARE @Id INT = (SELECT Id FROM DefinedType WHERE [Guid] = '4C052B4C-7341-4E27-A356-A7943EA48B34')
	--SELECT * FROM DefinedValue WHERE DefinedTypeId = @Id
	--DECLARE @inProgress VARCHAR(36) = 'E4A8815E-6300-45D0-A07F-068698A39638'
	--DECLARE @approved VARCHAR(36) = '74603219-8636-4166-A684-AB7621419723'
	--DECLARE @approvedWithRestrictions VARCHAR(36) = '51DCE5F0-4FDF-4A5A-86AD-93E25A899593'
	--DECLARE @expired VARCHAR(36) = '9A71A268-5150-4560-A151-C364C21B63BA'
	--DECLARE @now DATE = GETDATE()
	--UPDATE AttributeValue
	--SET Value = X.StatusGuid
	--FROM AttributeValue
	--JOIN (	SELECT avPS.Id,
	--				dbo.wcfnWrkFlw_getProtectionEvaluation(
	--					avPS.Value,
	--					avBC.ValueAsDateTime, 
	--					avR1.ValueAsDateTime, 
	--					avR2.ValueAsDateTime, 
	--					 avR3.ValueAsDateTime,
	--					avA.ValueAsDateTime, 
	--					avP.ValueAsDateTime) StatusGuid
	--		FROM Person p
	--		JOIN AttributeValue avPS ON p.Id = avPS.EntityId
	--		JOIN Attribute aPS ON avPS.AttributeId = aPS.Id	
	--						  AND aPS.[Key] = 'ProtectionStatus' 
	--		JOIN AttributeValue avBC ON p.Id = avBC.EntityId
	--		JOIN Attribute aBC ON avBC.AttributeId = aBC.Id	
	--						  AND aBC.[Key] = 'BackgroundCheckDate' 
	--		JOIN AttributeValue avR1 ON p.Id = avR1.EntityId
	--		JOIN Attribute aR1 ON avR1.AttributeId = aR1.Id	
	--						  AND aR1.[Key] = 'ProtectionReference1Date' 
	--		JOIN AttributeValue avR2 ON p.Id = avR2.EntityId
	--		JOIN Attribute aR2 ON avR2.AttributeId = aR2.Id	
	--						  AND aR2.[Key] = 'ProtectionReference2Date'
	--		JOIN AttributeValue avR3 ON p.Id = avR3.EntityId
	--		JOIN Attribute aR3 ON avR3.AttributeId = aR3.Id	
	--						  AND aR3.[Key] = 'ProtectionReference3Date' 
	--		JOIN AttributeValue avA ON p.Id = avA.EntityId
	--		JOIN Attribute aA ON avA.AttributeId = aA.Id	
	--						  AND aA.[Key] = 'ProtectionApplicationDate' 
	--		JOIN AttributeValue avP ON p.Id = avP.EntityId
	--		JOIN Attribute aP ON avP.AttributeId = aP.Id	
	--						  AND aP.[Key] = 'PolicyAcknowledgmentDate' 
	--		WHERE avPS.Value IN (@approved,@approvedWithRestrictions,@inProgress)
	--		) X ON AttributeValue.Id = X.Id

			--match protection fields
	DECLARE @protection TABLE ( AliasPersonGuid VARCHAR(36), Processed BIT )

	INSERT INTO @protection
	SELECT pa.[Guid], 0
	FROM PersonAlias pa
	INNER JOIN Person p on pa.PersonId = p.Id
	INNER JOIN AttributeValue av on p.Id = av.EntityId
	INNER JOIN Attribute a on av.AttributeId = a.Id and a.EntityTypeId = 15 and a.[Key] = 'ProtectionStatus'
	WHERE av.Value is not null and av.Value <> ''

	--loop to process save attribute value
	DECLARE @aliasPersonGuid VARCHAR(36) 

	WHILE (SELECT Count(*) FROM @protection WHERE processed = 0) > 0
	BEGIN
		SELECT TOP 1 
		@aliasPersonGuid = AliasPersonGuid
		FROM @protection 
		WHERE processed = 0

		EXEC [wcWrkFlw_saveProtectionEvaluationSingle] @aliasPersonGuid
		UPDATE @protection set processed = 1 WHERE AliasPersonGuid = @aliasPersonGuid 
	END

END

GO
