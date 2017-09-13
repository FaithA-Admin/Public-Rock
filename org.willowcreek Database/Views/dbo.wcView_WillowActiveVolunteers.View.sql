USE [Rock]
GO

/****** Object:  View [dbo].[wcView_WillowActiveVolunteers]    Script Date: 5/2/2017 3:36:20 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



ALTER VIEW [dbo].[wcView_WillowActiveVolunteers] AS
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
	FROM Person p WITH(NOLOCK)
	JOIN PersonAlias pa WITH(NOLOCK)
		ON pa.AliasPersonId = p.Id 
	),
CTE_ProtectionGroupTypes
AS
	(
		SELECT *
		FROM [Group] WITH(NOLOCK)
		WHERE GroupTypeId IN (SELECT DISTINCT Id FROM GroupType WITH(NOLOCK) WHERE Guid IN ('338765C6-A314-40C4-AE92-39550AE3EA49', 'AAB2E9F4-E828-4FEE-8467-73DC9DAB784C')) --Volunteer Root(P), Organization Unit
		UNION ALL
		SELECT g.*
		FROM [Group] g WITH(NOLOCK)
		JOIN CTE_ProtectionGroupTypes c 
			ON c.Id = g.ParentGroupId
	),
CTE_GroupMembers
AS
	(
		SELECT gm.PersonId AS PersonId,
			   g.Id AS GroupId,
			   g.Name AS GroupName
		FROM [Group] g WITH(NOLOCK)
		JOIN GroupType gt WITH(NOLOCK)
			ON gt.Id = g.GroupTypeId
		JOIN GroupMember gm WITH(NOLOCK)
			ON gm.GroupId = g.Id
		JOIN CTE_ProtectionGroupTypes pgt WITH(NOLOCK)	
			ON pgt.GroupTypeId = gt.Id 
				AND pgt.Id = g.Id--Only volunteer groups that belong to the Volunteer Root(P) or Organizational Unit group types.
		WHERE gm.GroupMemberStatus = 1 --Only Active group members
			AND g.IsActive = 1 --Only members in Active groups
	),
CTE_ProtectionStatus
AS
	(
		SELECT p.Id AS PersonId,
		  dv.Value AS ProtectionStatus
		FROM Person p WITH(NOLOCK)
		JOIN AttributeValue av WITH(NOLOCK)
			ON av.EntityId = p.Id
		JOIN Attribute a WITH(NOLOCK)
			ON a.Id = av.AttributeId
		JOIN DefinedValue dv WITH(NOLOCK)
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
		FROM Person p WITH(NOLOCK)
		JOIN AttributeValue av WITH(NOLOCK)
			ON av.EntityId = p.Id
		JOIN Attribute a WITH(NOLOCK)
			ON a.Id = av.AttributeId	
		WHERE a.Id = 1298 --Background check date attribute
			  AND av.Value <> '' --Skip empty dates
	),
CTE_PMMLastBackgrouncheckOrderId 
AS
	(
		SELECT av.EntityId AS PersonId,
			   av.Value AS OrderId
		FROM AttributeValue av WITH(NOLOCK)
		WHERE av.AttributeId = 4778
			AND av.Value <> ''
	),	
CTE_ValidProtectionDates 
AS
	(
		SELECT SUM(t.ValidDates) as ValidProtectionDates,
			   t.PersonId as PersonId
		FROM 
		(
			SELECT COUNT(*) as ValidDates,
				av.EntityId as PersonId
			FROM AttributeValue av WITH(NOLOCK)
			JOIN Attribute a WITH(NOLOCK)
				on av.AttributeId = a.Id
			WHERE a.GUID IN ('E48A103B-9FD1-45E2-BB67-A575FCCB4429') --Protection application date attribute
				AND av.Value <>''
				AND CAST(av.Value as date) >= CAST(DATEADD(Year, -1 * CAST((SELECT DefaultValue FROM Attribute WITH(NOLOCK) where Guid = '348AE146-4557-44DC-8E05-4A87D03FD20C') as int) , getdate()) as date)  --Protection application expiration date grater than expiration value	
			GROUP BY av.EntityId
			UNION ALL
			SELECT COUNT(*) as ValidDates,
				av.EntityId as PersonId
			FROM AttributeValue av WITH(NOLOCK)
			JOIN Attribute a WITH(NOLOCK)
				on av.AttributeId = a.Id
			WHERE a.GUID IN ('2F97DC60-57DB-4F0A-B1BC-D45F1D7669E0') --Policy acknowledment date attribute
				AND av.Value <>''
				AND CAST(av.Value as date) >= CAST(DATEADD(Year, -1 * CAST((SELECT DefaultValue FROM Attribute WITH(NOLOCK) where Guid = '0AEE2C5E-87A3-4B31-9A7F-914D0ED4CECB') as int) , getdate()) as date)  --Policy acknowledment expiration date grater than expiration value
			GROUP BY av.EntityId
			UNION ALL
			SELECT CASE  WHEN COUNT(*) >= 2 THEN 1 ELSE 0 END as ValidDates,
				av.EntityId as PersonId
			FROM AttributeValue av WITH(NOLOCK)
			JOIN Attribute a WITH(NOLOCK)
				on av.AttributeId = a.Id
			WHERE a.GUID IN ('FBC3A1E0-82E0-4128-A2B0-29446DD4EE44', '473ED6E5-AB9F-4B60-B13B-85512B5B6EF0','4A58D0D8-9EDE-4008-9714-D70BA4B7C774') --Protection reference dates attributes
				AND av.Value <>''
				AND CAST(av.Value as date) >= CAST(DATEADD(Year, -1 * CAST((SELECT DefaultValue FROM Attribute WITH(NOLOCK) where Guid = '7B86C69B-3FD2-4E42-A101-B2D13774C2F7') as int) , getdate()) as date)  --References expiration dates grater than expiration value
			GROUP BY av.EntityId
		) t
		GROUP By t.PersonId
	)	
	SELECT DISTINCT v.Id,
		   v.FirstName,
		   v.LastName,
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
	FROM CTE_Volunteers v WITH(NOLOCK)
	JOIN CTE_GroupMembers gm WITH(NOLOCK)
		ON gm.PersonId = v.Id
	LEFT JOIN CTE_ProtectionStatus ps WITH(NOLOCK)
		ON ps.PersonId = v.Id
	LEFT JOIN CTE_BackgroundCheckDate bcd WITH(NOLOCK)
		ON bcd.PersonId = v.Id
	LEFT JOIN CTE_PMMLastBackgrouncheckOrderId pmm WITH(NOLOCK)
		ON pmm.PersonId = v.Id	
	JOIN CTE_ValidProtectionDates vpd WITH(NOLOCK)
		ON vpd.PersonId = v.Id
	WHERE v.Id NOT IN (   ---already in progress
						SELECT pa.PersonId
						FROM Workflow w WITH(NOLOCK)
						JOIN WorkflowType wt WITH(NOLOCK)
							ON wt.Id = w.WorkflowTypeId  
								AND wt.Id = 56
						JOIN Attribute a WITH(NOLOCK)
							ON a.EntityTypeID = 113 
								AND a.[Key] = 'Applicant'
						JOIN AttributeValue av WITH(NOLOCK)
							ON av.AttributeID = a.Id 
								AND av.EntityId = w.Id
						JOIN PersonAlias pa WITH(NOLOCK)
							ON CAST(pa.[Guid] as nvarchar(max)) = av.[Value] 
						WHERE w.completedDateTime IS NULL
						)
		AND YEAR(bcd.BackgroundCheckDate) < YEAR(GETDATE())
		AND vpd.ValidProtectionDates = 3  --Need to have a valid protection application, references (2 at least) and policy acknowledgment dates


GO


