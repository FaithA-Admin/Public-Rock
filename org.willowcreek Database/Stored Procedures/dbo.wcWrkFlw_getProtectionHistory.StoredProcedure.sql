/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getProtectionHistory]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcWrkFlw_getProtectionHistory]
GO
/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getProtectionHistory]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[wcWrkFlw_getProtectionHistory]
	@aliasPersonGuid VARCHAR(36),
	@protectionSummary VARCHAR(MAX)
AS
BEGIN

	DECLARE @date VARCHAR(10)

	IF LEN(@aliasPersonGuid) > 0
	BEGIN

		--Background Check
		SELECT @date = CONVERT(VARCHAR(10), CAST(avD.Value AS DATE), 101)
		FROM PersonAlias pa 

		JOIN AttributeValue avD ON pa.PersonId = avD.EntityId
		JOIN Attribute aD ON avD.AttributeId = aD.Id
						AND aD.[Key] like 'BackgroundCheckDate'

		WHERE pa.[Guid] = @aliasPersonGuid

		SET @protectionSummary = REPLACE(@protectionSummary,'[BC]',COALESCE(@date,'none'))

		--Reference
		SELECT @date = CONVERT(VARCHAR(10), CAST(avR1D.Value AS DATE), 101)
		FROM PersonAlias pa

		JOIN AttributeValue avR1D ON pa.PersonId = avR1D.EntityId
		JOIN Attribute aR1D ON avR1D.AttributeId = aR1D.Id
						AND aR1D.[Key] like 'ProtectionReference1Date'

		WHERE pa.[Guid] = @aliasPersonGuid

		SET @protectionSummary = REPLACE(@protectionSummary,'[REF]',COALESCE(@date,'none'))

		--Application
		SELECT  @date = CONVERT(VARCHAR(10), CAST(avAD.Value AS DATE), 101)
		FROM PersonAlias pa

		JOIN AttributeValue avAD ON pa.PersonId = avAD.EntityId
		JOIN Attribute aAD ON avAD.AttributeId = aAD.Id
						AND aAD.[Key] like 'ProtectionApplicationDate'

		WHERE pa.[Guid] = @aliasPersonGuid

		SET @protectionSummary = REPLACE(@protectionSummary,'[APP]',COALESCE(@date,'none'))

		--Policy Acknowledgment
		SELECT  @date = CONVERT(VARCHAR(10), CAST(avAD.Value AS DATE), 101)
		FROM PersonAlias pa

		JOIN AttributeValue avAD ON pa.PersonId = avAD.EntityId
		JOIN Attribute aAD ON avAD.AttributeId = aAD.Id
						AND aAD.[Key] like 'PolicyAcknowledgmentDate'

		WHERE pa.[Guid] = @aliasPersonGuid

		SET @protectionSummary = REPLACE(@protectionSummary,'[POL]',COALESCE(@date,'none'))

	END
	ELSE
	BEGIN
		SET @protectionSummary = REPLACE(REPLACE(REPLACE(REPLACE(@protectionSummary,'[BC]','unknown'),'[REF]','unknown'),'[APP]','unknown'),'[POL]','unknown')
	END

	SELECT @protectionSummary

END
GO
