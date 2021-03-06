/****** Object:  View [dbo].[wcView_WillowActiveVolunteersInGroups]    Script Date: 6/13/2016 11:20:36 AM ******/
DROP VIEW [dbo].[wcView_WillowActiveVolunteersInGroups]
GO
/****** Object:  View [dbo].[wcView_WillowActiveVolunteersInGroups]    Script Date: 6/13/2016 11:20:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO







CREATE VIEW [dbo].[wcView_WillowActiveVolunteersInGroups] AS
WITH CTE_Volunteers
AS
	(
		SELECT p.Id AS Id,
			   p.FirstName AS FirstName,
			   p.LastName AS LastName,
			   p.CreatedDateTime,
			   p.ModifiedDateTime,
			   p.CreatedByPersonAliasId,
			   p.ModifiedByPersonAliasId,
			   pa.Guid,
			   pa.ForeignId,
			   pa.ForeignGuid,
			   pa.ForeignKey
		FROM Person p
		JOIN PersonAlias pa
			ON pa.AliasPersonId = p.Id 
	),
CTE_GroupMembers
AS
	(
		SELECT gm.PersonId AS PersonId,
			   g.Id AS GroupId,
			   g.Name AS GroupName
		FROM [Group] g
		JOIN GroupType gt
			ON gt.Id = g.GroupTypeId
		JOIN GroupMember gm
			ON gm.GroupId = g.Id
		WHERE gt.Id = 42 --Only Volunteer Group (P)
			  AND gm.GroupMemberStatus = 1 --Only Active group members
	),
CTE_ProtectionStatus
AS
	(
		SELECT p.Id AS PersonId,
		  dv.Value AS ProtectionStatus
		FROM Person p
		JOIN AttributeValue av
			ON av.EntityId = p.Id
		JOIN Attribute a
			ON a.Id = av.AttributeId
		JOIN DefinedValue dv
			ON dv.Guid = av.Value
		WHERE a.Id = 1731 --Protection Status attribute
			  AND av.Value <> '' --Skip members with no protection status
			  AND dv.Value <> 'Declined'
	),
CTE_BackgroundCheckDate
AS
	(
		SELECT p.Id AS PersonId,
		   CAST(av.Value AS date) AS BackgroundCheckDate
		FROM Person p
		JOIN AttributeValue av
			ON av.EntityId = p.Id
		JOIN Attribute a
			ON a.Id = av.AttributeId	
		WHERE a.Id = 1298 --Background check date attribute
			  AND av.Value <> '' --Skip empty dates
	),
CTE_PMMLastBackgrouncheckOrderId
AS
	(
		SELECT av.EntityId AS PersonId,
			   av.Value AS OrderId
		FROM AttributeValue av
		WHERE av.AttributeId = 4778
			AND av.Value <> ''
	) 
	SELECT DISTINCT v.Id,
		   v.FirstName,
		   v.LastName,
		   gm.GroupId,
		   gm.GroupName,
		   ps.ProtectionStatus,
		   bcd.BackgroundCheckDate,	   
		   CAST(pmm.OrderId AS int) AS OrderId,
		   v.CreatedDateTime,
		   v.ModifiedDateTime,
		   v.CreatedByPersonAliasId,
		   v.ModifiedByPersonAliasId,
		   v.Guid,
		   v.ForeignId,
		   v.ForeignGuid,
		   v.ForeignKey
	FROM CTE_Volunteers v
	JOIN CTE_GroupMembers gm
		ON gm.PersonId = v.Id
	LEFT JOIN CTE_ProtectionStatus ps
		ON ps.PersonId = v.Id
	LEFT JOIN CTE_BackgroundCheckDate bcd
		ON bcd.PersonId = v.Id
	LEFT JOIN CTE_PMMLastBackgrouncheckOrderId pmm
		ON pmm.PersonId = v.Id		







GO
