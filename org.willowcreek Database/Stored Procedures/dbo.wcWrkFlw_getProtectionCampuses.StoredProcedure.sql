/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getProtectionCampuses]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcWrkFlw_getProtectionCampuses]
GO
/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getProtectionCampuses]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[wcWrkFlw_getProtectionCampuses]
	@aliasPersonGuid UNIQUEIDENTIFIER
AS
BEGIN

	DECLARE @personId INT = (SELECT PersonId FROM PersonAlias WHERE AliasPersonGuid = @aliasPersonGuid)

	--group id for 'WC - Protection Directors'
	DECLARE @directorGrpId INT = (SELECT Id FROM [Group] WHERE [Guid] = '084F144C-D239-4878-A2CF-4567DEBD1939')
	--group id for 'WC - Protection Coordinators'
	DECLARE @coordinatorGrpId INT = (SELECT Id FROM [Group] WHERE [Guid] = '084F144C-D239-4878-A2CF-4567DEBD1939')

	--Check if Protection Director
	IF EXISTS (SELECT NULL FROM GroupMember WHERE GroupId = @directorGrpId AND PersonId = @personId AND GroupMemberStatus = 1)
	BEGIN
		--get all campuses
		SELECT Id [Value], Name [Text]
		FROM Campus
		ORDER BY Name
	END
	ELSE IF EXISTS (SELECT NULL FROM GroupMember WHERE GroupId = @coordinatorGrpId AND PersonId = @personId AND GroupMemberStatus = 1)
	BEGIN
		--group type id for "organization unit"
		DECLARE @gt INT = (SELECT Id FROM GroupType WHERE [Guid] = 'AAB2E9F4-E828-4FEE-8467-73DC9DAB784C')

		--get campuses based on org chart
		SELECT DISTINCT c.Id [Value], c.Name [Text]
		FROM GroupMember gm
		JOIN [Group] g ON gm.GroupId = g.Id
					  AND g.GroupTypeId = @gt
		JOIN Campus c ON g.CampusId = c.Id
		WHERE gm.PersonId = @personId

	END

END
GO
