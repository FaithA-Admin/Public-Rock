/****** Object:  UserDefinedFunction [dbo].[ufnCrm_GetHeadOfHousePersonIdFromGivingId]    Script Date: 6/13/2016 11:25:28 AM ******/
DROP FUNCTION [dbo].[ufnCrm_GetHeadOfHousePersonIdFromGivingId]
GO
/****** Object:  UserDefinedFunction [dbo].[ufnCrm_GetHeadOfHousePersonIdFromGivingId]    Script Date: 6/13/2016 11:25:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
<doc>
	<summary>
 		This function returns the head of house for the giving id provided
	</summary>

	<returns>
		Person Id of the head of household. 
	</returns>
	<remarks>
		

	</remarks>
	<code>
		SELECT [dbo].[ufnCrm_GetHeadOfHousePersonIdFromGivingId]('G63') -- Decker's (married) 
		SELECT [dbo].[ufnCrm_GetHeadOfHousePersonIdFromGivingId]('G64') -- Jones' (single)
	</code>
</doc>
*/

CREATE FUNCTION [dbo].[ufnCrm_GetHeadOfHousePersonIdFromGivingId](@GivingId varchar(31) ) 

RETURNS int AS
BEGIN
	DECLARE @UnitType char(1)
	DECLARE @UnitId int
	DECLARE @Result int

	SET @UnitType = LEFT(@GivingId, 1)
	SET @UnitId = CAST(SUBSTRING(@GivingId, 2, LEN(@GivingId)) AS INT)

	IF @UnitType = 'P' -- person
		SET @Result = @UnitId
	ELSE -- family
		SET @Result =	(
							SELECT TOP 1 p.[Id] 
							FROM 
								[Person] p
								INNER JOIN [GroupMember] gm ON gm.[PersonId] = p.[Id]
								INNER JOIN [GroupTypeRole] gtr ON gtr.[Id] = gm.[GroupRoleId]
							WHERE 
								gtr.[Guid] = '2639F9A5-2AAE-4E48-A8C3-4FFE86681E42' -- adult
								AND gm.[GroupId] = @UnitId
							ORDER BY p.[Gender]
						)

	RETURN @Result
END

GO
