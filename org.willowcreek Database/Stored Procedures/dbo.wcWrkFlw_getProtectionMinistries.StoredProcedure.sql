/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getProtectionMinistries]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcWrkFlw_getProtectionMinistries]
GO
/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getProtectionMinistries]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[wcWrkFlw_getProtectionMinistries]
	@aliasPersonGuid UNIQUEIDENTIFIER
AS
BEGIN

	DECLARE @personId INT = (SELECT PersonId FROM PersonAlias WHERE AliasPersonGuid = @aliasPersonGuid)

	--group id for 'WC - Protection Directors'
	DECLARE @directorGrpId INT = (SELECT Id FROM [Group] WHERE [Guid] = '084F144C-D239-4878-A2CF-4567DEBD1939')
	--group id for 'WC - Protection Coordinators'
	DECLARE @coordinatorGrpId INT = (SELECT Id FROM [Group] WHERE [Guid] = '084F144C-D239-4878-A2CF-4567DEBD1939')

	--group type id of 'organization unit'
	DECLARE @gt INT = (SELECT Id FROM GroupType WHERE [Guid] = 'AAB2E9F4-E828-4FEE-8467-73DC9DAB784C')
	DECLARE @orgGrpId INT = (SELECT Id FROM [Group] WHERE [Guid] = 'EF41CD00-1266-4BE6-9130-453982014B79')

	--Check if Protection Director
	IF EXISTS (SELECT NULL FROM GroupMember WHERE GroupId = @directorGrpId AND PersonId = @personId AND GroupMemberStatus = 1)
	BEGIN

		--get all ministries (three deep) TODO: make dynamic
		SELECT g.Id [Value], c.ShortCode + ' - ' + g.Name [Text]
		FROM [Group] g
		JOIN Campus c ON g.CampusId = c.Id
		WHERE g.ParentGroupId IN (SELECT Id
								  FROM [Group]
								  WHERE GroupTypeId = @gt
								  AND ParentGroupId = @orgGrpId)
		ORDER BY c.ShortCode, g.Name

	END
	ELSE IF EXISTS (SELECT NULL FROM GroupMember WHERE GroupId = @coordinatorGrpId AND PersonId = @personId AND GroupMemberStatus = 1)
	BEGIN

		--get ministries based on org chart membership
		SELECT g.Id [Value], g.Name [Text]
		FROM GroupMember gm
		JOIN [Group] g ON gm.GroupId = g.Id
					  AND g.GroupTypeId = @gt
		WHERE gm.PersonId = @personId

	END

END
GO
