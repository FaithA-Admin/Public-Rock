/****** Object:  UserDefinedFunction [dbo].[ufnCrm_FamilyMembersOfPersonId]    Script Date: 6/13/2016 11:25:28 AM ******/
DROP FUNCTION [dbo].[ufnCrm_FamilyMembersOfPersonId]
GO
/****** Object:  UserDefinedFunction [dbo].[ufnCrm_FamilyMembersOfPersonId]    Script Date: 6/13/2016 11:25:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
<doc>
	<summary>
 		This function returns all people in a family with the provided person id includes the person.
	</summary>

	<returns>
		* Id
		* NickName
		* LastName
		* Guid
		* GroupId
		* GroupGuid
		* GroupName
		* GroupRoleId
	</returns>
	<param name="PersonId" datatype="int">Person Id of family member</param>
	<remarks>
		Uses the following constants:
			* Group Type - Family: 790E3215-3B10-442B-AF69-616C0DCB998E
	</remarks>
	<code>
		SELECT * FROM [dbo].[ufnCrm_FamilyMembersOfPersonId](6) -- Ted Decker's family in sample data
	</code>
</doc>
*/

CREATE FUNCTION [dbo].[ufnCrm_FamilyMembersOfPersonId](@PersonId int)
RETURNS TABLE AS

RETURN ( 
	SELECT 
		p.[Id]
		, [NickName]
		, [LastName]
		, p.[Guid]
		, g.[Id] as [GroupId]
		, g.[Guid] as [GroupGuid]
		, g.[Name] as [GroupName]
		, gm.[GroupRoleId]

	FROM
		[Person] p
		INNER JOIN [GroupMember] gm ON gm.[PersonId] = p.[Id]
		INNER JOIN [Group] g ON g.Id = gm.[GroupId]
	WHERE
		g.[GroupTypeId] = (SELECT [Id] FROM [GroupType] WHERE [Guid] = '790E3215-3B10-442B-AF69-616C0DCB998E')
		AND g.[Id] IN (SELECT [GroupId] FROM [GroupMember] WHERE [PersonId] = @PersonId)
)

GO
