/****** Object:  UserDefinedFunction [dbo].[ufnCrm_GetFamilyTitleFromGivingId]    Script Date: 6/13/2016 11:25:28 AM ******/
DROP FUNCTION [dbo].[ufnCrm_GetFamilyTitleFromGivingId]
GO
/****** Object:  UserDefinedFunction [dbo].[ufnCrm_GetFamilyTitleFromGivingId]    Script Date: 6/13/2016 11:25:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
<doc>
	<summary>
 		This function returns the household name from a giving id.
	</summary>

	<returns>
		String of household name. 
	</returns>
	<remarks>
		

	</remarks>
	<code>
		SELECT [dbo].[ufnCrm_GetFamilyTitleFromGivingId]('G63') -- Decker's (married) Returns 'Ted & Cindy Decker'
		SELECT [dbo].[ufnCrm_GetFamilyTitleFromGivingId]('G64') -- Jones' (single) Returns 'Ben Jones'
	</code>
</doc>
*/

CREATE FUNCTION [dbo].[ufnCrm_GetFamilyTitleFromGivingId](@GivingId varchar(31) ) 

RETURNS nvarchar(250) AS
BEGIN
	DECLARE @UnitType char(1)
	DECLARE @UnitId int
	DECLARE @Result varchar(250)

	SET @UnitType = LEFT(@GivingId, 1)
	SET @UnitId = CAST(SUBSTRING(@GivingId, 2, LEN(@GivingId)) AS INT)

	IF @UnitType = 'P'
		SET @Result = (SELECT TOP 1 [NickName] + ' ' + [LastName] FROM [Person] WHERE [GivingId] = @GivingId)
	ELSE
		SET @Result = (SELECT * FROM dbo.ufnCrm_GetFamilyTitle(null, @UnitId, default, 1))

	RETURN @Result
END

GO
