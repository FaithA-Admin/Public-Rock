/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getProtectionBackgroundCheckStatus]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcWrkFlw_getProtectionBackgroundCheckStatus]
GO
/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getProtectionBackgroundCheckStatus]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[wcWrkFlw_getProtectionBackgroundCheckStatus]
	@aliasPersonGuid VARCHAR(36)
AS
BEGIN

	DECLARE @Required VARCHAR(5)

	IF LEN(@aliasPersonGuid) > 0
	BEGIN

		/*
		SELECT @Required =
				CASE avC.Value WHEN 'True' THEN
    				CASE avR.Value WHEN 'Pass' THEN
    					CASE WHEN CAST(avD.Value AS DATE) >= DATEADD(YEAR,-1,GETDATE()) THEN 'False'
    					ELSE 'True' END --Expired
    				ELSE 'True' END --Failed
    			ELSE 'True' END --Not Run
		*/
		SELECT @Required = CASE WHEN CAST(avD.Value AS DATE) >= DATEADD(YEAR,-1,GETDATE()) THEN 'False'
    					        ELSE 'True' END --Expired
		FROM PersonAlias pa 

		--JOIN AttributeValue avC ON pa.PersonId = avC.EntityId
		--JOIN Attribute aC ON avC.AttributeId = aC.Id
		--				AND aC.[Key] like 'BackgroundChecked'

		--JOIN AttributeValue avR ON pa.PersonId = avR.EntityId
		--JOIN Attribute aR ON avR.AttributeId = aR.Id
		--				AND aR.[Key] like 'BackgroundCheckResult'

		JOIN AttributeValue avD ON pa.PersonId = avD.EntityId
		JOIN Attribute aD ON avD.AttributeId = aD.Id
						AND aD.[Key] like 'BackgroundCheckDate'

		WHERE pa.[Guid] = @aliasPersonGuid

	END

	SELECT COALESCE(@Required,'True') --If Null, B/C never requested

END
GO
