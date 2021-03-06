/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getProtectionIsDirector]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcWrkFlw_getProtectionIsDirector]
GO
/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getProtectionIsDirector]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[wcWrkFlw_getProtectionIsDirector]
	@aliasPersonGuid VARCHAR(36)
AS
BEGIN

	DECLARE @IsDirector VARCHAR(5) = 'False'

	IF LEN(@aliasPersonGuid) > 0
	BEGIN

		SELECT @IsDirector = 'True'
		FROM PersonAlias pa
		JOIN GroupMember gm ON pa.PersonId = gm.PersonId
		JOIN [Group] g ON gm.GroupId = g.Id
					  AND g.[Guid] = '084F144C-D239-4878-A2CF-4567DEBD1939'--Director
		WHERE pa.[Guid] = @aliasPersonGuid

	END

	SELECT COALESCE(@IsDirector,'False') --If Null, Not Found

END
GO
