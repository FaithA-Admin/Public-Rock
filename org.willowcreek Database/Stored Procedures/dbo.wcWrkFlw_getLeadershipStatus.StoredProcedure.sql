/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getLeadershipStatus]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcWrkFlw_getLeadershipStatus]
GO
/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getLeadershipStatus]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[wcWrkFlw_getLeadershipStatus]
	@aliasPersonGuid VARCHAR(36)
AS
BEGIN

DECLARE @statusReturned VARCHAR(100)

	IF LEN(@aliasPersonGuid) > 0
	BEGIN

		SELECT @statusReturned = dv.Value
		
		FROM PersonAlias pa

		JOIN AttributeValue avAD ON pa.PersonId = avAD.EntityId
		JOIN Attribute aAD ON avAD.AttributeId = aAD.Id
						AND aAD.[Key] like 'LeadershipStatus'
		JOIN DefinedValue dv ON avAD.Value = CAST(dv.[Guid] AS varchar(36))

		WHERE pa.[Guid] = @aliasPersonGuid

	END

	SELECT COALESCE(@statusReturned,'') 

END


GO
