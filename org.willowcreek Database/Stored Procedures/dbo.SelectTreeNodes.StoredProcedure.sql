/****** Object:  StoredProcedure [dbo].[SelectTreeNodes]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[SelectTreeNodes]
GO
/****** Object:  StoredProcedure [dbo].[SelectTreeNodes]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SelectTreeNodes] @nodeNum varchar(20)
AS

/*
This section is used to select/include the Chronicle groups that should be created.
Only members at the lowest node level will be converted to Rock.

Any groups that are not to be inlcued in this selection must have their NodeNum in the Rock.[dbo].[_org_willowcreek_IMPORT_group_exclude] table.
All children of these exluded nodeNums will not be included in the group selection even if they are contained under the @nodeNum node
*/

INSERT INTO Rock.dbo._org_willowcreek_IMPORT_group_exclude
(ChronicleGroupId)
SELECT NodeNum
FROM [kairos].[dbo].[Tree_Node]
WHERE NodeName LIKE '%5th Grade%'
OR
NodeName LIKE '%Sunday 9 Incom%'
OR 
NodeName LIKE '%2014%'
OR 
NodeName LIKE '%2013%'
OR 
NodeName LIKE '%PLNS HOLIDAY GOLD TEAMS%'

--@lastNodeLevel : this is the lowest level of the @nodeNum which will contain all the members that will be selected
--				   Normally this should be 4
Declare @lastNodeLevel INT = (SELECT MAX([LevelNum]) FROM [kairos].[dbo].[Tree_Lineage] tl   JOIN [kairos].[dbo].[Tree_Node] tn
  ON tn.NodeNum = tl.ObjectNum
  WHERE Lineage LIKE '%'+@nodeNum+'/%'
  AND ObjectTypeCD = 'NODE'
  AND tn.Ended = '1900-01-01')

INSERT INTO [Rock].dbo._org_willowcreek_IMPORT_group
(ChronicleGroupId)
SELECT [ObjectNum]
  FROM [kairos].[dbo].[Tree_Lineage] tl
  JOIN [kairos].[dbo].[Tree_Node] tn
  ON tn.NodeNum = tl.ObjectNum
  WHERE Lineage LIKE '%'+@nodeNum+'/%'
  AND ObjectTypeCD = 'NODE'
  AND tn.Ended = '1900-01-01'
  AND tl.[LevelNum] = @lastNodeLevel
  AND (SELECT COUNT(*) FROM Rock.[dbo].[_org_willowcreek_IMPORT_group_exclude] WHERE tl.lineage LIKE '%/'+ChronicleGroupId+'/%') = 0

  DELETE FROM Rock.dbo._org_willowcreek_IMPORT_group_exclude

GO
