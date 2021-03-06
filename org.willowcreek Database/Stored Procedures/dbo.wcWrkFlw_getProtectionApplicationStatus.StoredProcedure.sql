/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getProtectionApplicationStatus]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcWrkFlw_getProtectionApplicationStatus]
GO
/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getProtectionApplicationStatus]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[wcWrkFlw_getProtectionApplicationStatus]
	@aliasPersonGuid VARCHAR(36)
AS
BEGIN

DECLARE @Required VARCHAR(5)

	IF LEN(@aliasPersonGuid) > 0
	BEGIN

		SELECT @Required =
			   CASE WHEN CAST(avAD.Value AS DATE) >= DATEADD(Year,-5,GETDATE()) THEN 'False'
					ELSE 'True' END
		FROM PersonAlias pa

		JOIN AttributeValue avAD ON pa.PersonId = avAD.EntityId
		JOIN Attribute aAD ON avAD.AttributeId = aAD.Id
						AND aAD.[Key] like 'ProtectionApplicationDate'

		WHERE pa.[Guid] = @aliasPersonGuid

	END

	SELECT COALESCE(@Required,'True') --If Null, Applicant never requested

END
GO
