/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getProtectionReferenceStatus]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcWrkFlw_getProtectionReferenceStatus]
GO
/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getProtectionReferenceStatus]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[wcWrkFlw_getProtectionReferenceStatus]
	@aliasPersonGuid VARCHAR(36)
AS
BEGIN

	DECLARE @Required VARCHAR(5)

	IF LEN(@aliasPersonGuid) > 0
	BEGIN

		DECLARE @ref1 INT = 0
		DECLARE @ref2 INT = 0
		DECLARE @ref3 INT = 0

		SELECT @ref1 = CASE WHEN CAST(avRD.Value AS DATE) >= DATEADD(Year,-3,GETDATE()) THEN 1 ELSE 0 END
		FROM PersonAlias pa
		JOIN AttributeValue avRD ON pa.PersonId = avRD.EntityId
		JOIN Attribute aRD ON avRD.AttributeId = aRD.Id
						AND aRD.[Key] like 'ProtectionReference1Date'
		WHERE pa.[Guid] = @aliasPersonGuid

		SELECT @ref2 = CASE WHEN CAST(avRD.Value AS DATE) >= DATEADD(Year,-3,GETDATE()) THEN 1 ELSE 0 END
		FROM PersonAlias pa
		JOIN AttributeValue avRD ON pa.PersonId = avRD.EntityId
		JOIN Attribute aRD ON avRD.AttributeId = aRD.Id
						AND aRD.[Key] like 'ProtectionReference2Date'
		WHERE pa.[Guid] = @aliasPersonGuid

		SELECT @ref3 = CASE WHEN CAST(avRD.Value AS DATE) >= DATEADD(Year,-3,GETDATE()) THEN 1 ELSE 0 END
		FROM PersonAlias pa
		JOIN AttributeValue avRD ON pa.PersonId = avRD.EntityId
		JOIN Attribute aRD ON avRD.AttributeId = aRD.Id
						AND aRD.[Key] like 'ProtectionReference3Date'
		WHERE pa.[Guid] = @aliasPersonGuid

		IF (@ref1 + @ref2 + @ref3) < 2
			SET @Required = 'True'
		ELSE
			SET @Required = 'False'

	END

	SELECT COALESCE(@Required,'True') --If Null, Reference Check never requested

END
GO
