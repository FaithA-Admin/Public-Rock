--USE [RockDev_v4_SC_3]
--GO

--Update protection restrictions
INSERT INTO AttributeValue (IsSystem, AttributeId, EntityId, Value, Guid, CreatedDateTime)
SELECT IsSystem, AttributeId, EntityId, Value, Guid, CreatedDateTime FROM (
	SELECT 0 AS IsSystem, 2904 AS AttributeId, p.Id AS EntityId, 
		CASE
			WHEN a.[Key] = 'ProtectionRestrictionMinor' THEN 'Minors and Vulnerable Adults'
			WHEN a.[Key] = 'ProtectionRestrictionVolunteering' THEN 'Volunteering'
		END AS Value,
		NEWID() AS [Guid], GETDATE() AS CreatedDateTime, ROW_NUMBER() OVER(PARTITION BY p.Id ORDER BY a.[Key] DESC) rn
	FROM AttributeValue av
	JOIN Attribute a ON av.AttributeId = a.Id 
	JOIN Person p ON p.Id = av.EntityId
	WHERE a.[Key] IN ('ProtectionRestrictionMinor','ProtectionRestrictionVolunteering')
	AND av.Value != '' AND av.Value IS NOT NULL
) data
WHERE rn = 1
