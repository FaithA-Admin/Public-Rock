/****** Object:  StoredProcedure [dbo].[wcJob_fixProtectionStatus]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcJob_fixProtectionStatus]
GO
/****** Object:  StoredProcedure [dbo].[wcJob_fixProtectionStatus]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[wcJob_fixProtectionStatus]
AS
BEGIN

	--PROCEDURE [dbo].[wcWrkFlw_saveProtectionEvaluation]
	--PROCEDURE [dbo].[wcWrkFlw_saveProtectionEvaluationSingle]
	--FUNCTION [dbo].[wcfnWrkFlw_getProtectionEvaluation]

	--Protection Workflow Expire Code

	--select * from attributevalue where entityid = 3 and attributeid = 1687

	declare @now datetime = getdate()

	--this routine reapplies archived protection dates to protection attributes if they are still within the active date range

	--declare @attrs table ( personId int, attrId int, attrVal nvarchar(max), protDate date, [key] nvarchar(200), archiveDateTime datetime, processed int)

	--insert into @attrs
	----declare @now datetime = getdate()
	--select * from (
	--select pa.PersonId, av.id, av.Value, paa.BackgroundCheckDate protDate, 'BackgroundCheckDate' [Key], paa.ArchiveDateTime, 0 processed
	--from _org_willowcreek_ProtectionApp_Archive paa
	--join PersonAlias pa on paa.PersonAliasGuid = pa.[Guid]
	--left join AttributeValue av on pa.PersonId = av.EntityId and av.AttributeId = 1298	--BackgroundCheckDate
	--where paa.BackgroundCheckDate > dateadd(year,-1,@now)
	--and (av.Value = '' or av.Value is null)
	--union
	--select pa.PersonId, av.id, av.Value, paa.Reference1Date protDate, 'ProtectionReference1Date' [Key], paa.ArchiveDateTime, 0 processed
	--from _org_willowcreek_ProtectionApp_Archive paa
	--join PersonAlias pa on paa.PersonAliasGuid = pa.[Guid]
	--left join AttributeValue av on pa.PersonId = av.EntityId and av.AttributeId = 1687	--ProtectionReference1Date
	--where paa.Reference1Date > dateadd(year,-5,@now)
	--and (av.Value = '' or av.Value is null)
	--union
	--select pa.PersonId, av.id, av.Value, paa.Reference2Date protDate, 'ProtectionReference2Date' [Key], paa.ArchiveDateTime, 0 processed
	--from _org_willowcreek_ProtectionApp_Archive paa
	--join PersonAlias pa on paa.PersonAliasGuid = pa.[Guid]
	--left join AttributeValue av on pa.PersonId = av.EntityId and av.AttributeId = 1688	--ProtectionReference2Date
	--where paa.Reference2Date > dateadd(year,-5,@now)
	--and (av.Value = '' or av.Value is null)
	--union
	--select pa.PersonId, av.id, av.Value, paa.Reference3Date protDate, 'ProtectionReference3Date' [Key], paa.ArchiveDateTime, 0 processed
	--from _org_willowcreek_ProtectionApp_Archive paa
	--join PersonAlias pa on paa.PersonAliasGuid = pa.[Guid]
	--left join AttributeValue av on pa.PersonId = av.EntityId and av.AttributeId = 1880	--ProtectionReference3Date
	--where paa.Reference3Date > dateadd(year,-5,@now)
	--and (av.Value = '' or av.Value is null)
	--union
	--select pa.PersonId, av.id, av.Value, paa.ApplicationDate protDate, 'ProtectionApplicationDate' [Key], paa.ArchiveDateTime, 0 processed
	--from _org_willowcreek_ProtectionApp_Archive paa
	--join PersonAlias pa on paa.PersonAliasGuid = pa.[Guid]
	--left join AttributeValue av on pa.PersonId = av.EntityId and av.AttributeId = 1685	--ProtectionApplicationDate
	--where paa.ApplicationDate > dateadd(year,-5,@now)
	--and (av.Value = '' or av.Value is null)
	--union
	--select pa.PersonId, av.id, av.Value, paa.PolicyAcknowledgmentDate protDate, 'PolicyAcknowledgmentDate' [Key], paa.ArchiveDateTime, 0 processed
	--from _org_willowcreek_ProtectionApp_Archive paa
	--join PersonAlias pa on paa.PersonAliasGuid = pa.[Guid]
	--left join AttributeValue av on pa.PersonId = av.EntityId and av.AttributeId = 1691	--PolicyAcknowledgmentDate
	--where paa.PolicyAcknowledgmentDate > dateadd(year,-5,@now)
	--and (av.Value = '' or av.Value is null)
	--) x order by personId, [key], protDate desc

	DECLARE @id INT
	DECLARE @date DATE
	DECLARE @key nvarchar(200)
	--WHILE (SELECT Count(*) FROM @attrs WHERE processed = 0) > 0
	--BEGIN
	--	SELECT TOP 1 
	--		@id = personId, 
	--		@date = protDate,
	--		@key = [key]
	--	FROM @attrs 
	--	WHERE processed = 0

	--	EXEC wcUtil_saveAttributeValue @key, 'Person', @id, @date
	--	UPDATE @attrs set processed = 1 WHERE personId = @id and [key] = @key
	--END

	--set protection status to declined if they are restricted from volunteering

	--select * from attribute where [key] like '%ProtectionRestrictionMinor%'	--ProtectionRestrictionMinor 2062
	--select * from attribute where [key] like '%volunte%'	--ProtectionRestrictionVolunteering 2063
	--select * from attribute where [key] like '%protectionstatus%'	--ProtectionStatus 1731

	--declare @now datetime = getdate()

	declare @person table ( personId int, processed int )

	insert @person
	select p.Id, 0 --, avRV.Value, avPS.Value, dvPS.Value
	from person p
	join attributevalue avRV on p.id = avRV.entityid and avRV.attributeid = 2063
	left join attributevalue avPS on p.id = avPS.entityid and avPS.attributeid = 1731
	left join definedvalue dvPS on avPS.Value = dvPS.[guid]
	where avRV.Value = 'Restricted'
	and (avPS.Value <> '5f6f6381-e8c1-4b89-b49e-248b734bb685' or avPS.Value is null)

	--DECLARE @id INT
	WHILE (SELECT Count(*) FROM @person WHERE processed = 0) > 0
	BEGIN
		SELECT TOP 1 
			@id = personId
		FROM @person 
		WHERE processed = 0

		EXEC wcUtil_saveAttributeValue 'ProtectionStatus', 'Person', @id, '5f6f6381-e8c1-4b89-b49e-248b734bb685'
		UPDATE @person set processed = 1 WHERE personId = @id
	END

	--reset

	delete from @person

	--set protection status to approved with restrictions (or declined) if they are restricted from minors/adults

	insert @person
	select p.Id, 0 --, avRV.Value, avPS.Value, dvPS.Value
	from person p
	join attributevalue avRV on p.id = avRV.entityid and avRV.attributeid = 2062
	left join attributevalue avPS on p.id = avPS.entityid and avPS.attributeid = 1731
	left join definedvalue dvPS on avPS.Value = dvPS.[guid]
	where avRV.Value = 'Restricted'
	and ((avPS.Value <> '51dce5f0-4fdf-4a5a-86ad-93e25a899593'
		  and avPS.Value <> '5f6f6381-e8c1-4b89-b49e-248b734bb685') 
		 or avPS.Value is null)

	--DECLARE @id INT
	WHILE (SELECT Count(*) FROM @person WHERE processed = 0) > 0
	BEGIN
		SELECT TOP 1 
			@id = personId
		FROM @person 
		WHERE processed = 0

		EXEC wcUtil_saveAttributeValue 'ProtectionStatus', 'Person', @id, '51dce5f0-4fdf-4a5a-86ad-93e25a899593'
		UPDATE @person set processed = 1 WHERE personId = @id
	END

END

GO
