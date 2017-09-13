--select * from notetype
--select * from Rock_v3_BAK.dbo.NoteType
--select * from Note

--select n1.id, n2.id, n1.notetypeid, n2.notetypeid, n1.[text]
--from rock.dbo.note n1
--join rock_v3_BAK.dbo.Note n2 on n1.id = n2.id

UPDATE [Rock].[dbo].Note
SET NoteTypeId = 
(
	SELECT
	CASE
		 WHEN n1.NoteTypeId = 1 THEN 7
		 WHEN n1.NoteTypeId = 5 THEN 11
		 ELSE n1.NoteTypeId 
	END
	FROM [Rock_v3_BAK].[dbo].Note n1 WHERE n1.Id = [Rock].[dbo].[Note].Id
)
WHERE EXISTS
(
    SELECT n1.* 
    FROM [Rock_v3_BAK].[dbo].Note n1 
    WHERE n1.Id = [Rock].[dbo].Note.Id
)
