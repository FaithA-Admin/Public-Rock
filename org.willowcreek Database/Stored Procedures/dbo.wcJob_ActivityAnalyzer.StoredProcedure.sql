/****** Object:  StoredProcedure [dbo].[wcJob_ActivityAnalyzer]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcJob_ActivityAnalyzer]
GO
/****** Object:  StoredProcedure [dbo].[wcJob_ActivityAnalyzer]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[wcJob_ActivityAnalyzer]
AS
BEGIN

	--based on last three months of attendance
	DECLARE @date DATETIME = DATEADD(m,-3,GETDATE())

	DECLARE @family TABLE (familyId INT, campusId INT, kairosCampus INT, active INT)
	DECLARE @familyMember TABLE (familyId INT, memberId INT, attendances INT)

	begin transaction

	--select all attenders
	insert into @family
	select g.Id, null, null, null
	from Person p
	join PersonAlias pa on p.id = pa.PersonId
	join GroupMember gm on p.id = gm.PersonId
	join [Group] g on gm.GroupId = g.Id and g.GroupTypeId = 10
	where exists (select null
					from checkin.dbo.prom_attendance
					where attended > @date
					  and entityid = pa.AliasPersonId)

	--get all family members
	insert into @familyMember
	select gm.GroupId, p.id, 0
	from @family f
	join GroupMember gm on f.familyId = gm.GroupId
	join Person p on gm.PersonId = p.id


	--update campus id with old chronicle id
	update f
	set kairosCampus = (SELECT TOP 1 kpa.Campus
					FROM checkin.dbo.Prom_Attendance kpa
					JOIN @familyMember fm on kpa.EntityId = fm.memberId
					WHERE attended > @date
					AND f.familyId = fm.familyId
					GROUP BY kpa.Campus
					ORDER BY COUNT(*) DESC)
	from @family f


	--convert old chronicle campus id to rock guid
	update f
	set campusId = CASE f.kairosCampus
						WHEN 1 THEN 1 --SBR
						WHEN 2 THEN 2 --DPG
						WHEN 4 THEN 3 --NSH
						WHEN 8 THEN 4 --CLK
						WHEN 32 THEN 6 --CHI
						WHEN 64 THEN 7 -- HNT
						WHEN 128 THEN 8 --SLC
					END
	from @family f

	--update family's campus
	update g
	set g.CampusId = f.campusId
	--select g.id, g.CampusId, f.campusId
	from [group] g
	join @family f on g.Id = f.familyId
	where f.campusId is not null


	--how often did a person attend
	update fm
	set attendances = (SELECT COUNT(*)
					    FROM checkin.dbo.Prom_Attendance kpa
					   WHERE attended > @date
					   AND kpa.EntityId = pa.AliasPersonId)
	from @familyMember fm
	join PersonAlias pa on fm.memberid = pa.personid

	--flag family as active
	update f
	set active = 1
	from @family f
	join @familyMember fm on f.familyId = fm.familyId
	where fm.attendances >= 6 --6 times in 3 months

	--update person from attendee to visitor (by family)
	--update p
	--set p.ConnectionStatusValueId = 66 --visitor
	----select p.id, p.ConnectionStatusValueId
	--from @family f
	--join GroupMember gm on f.familyid = gm.GroupId
	--join person p on gm.PersonId = p.Id
	--where f.active = 0
	--and p.ConnectionStatusValueId = 146 --attendee

	--blindly update every attendee to visitory
	--update p
	--set p.ConnectionStatusValueId = 66 --visitor
	--select p.id, p.ConnectionStatusValueId
	--from person p
	--where p.ConnectionStatusValueId = 146 --attendee

	--update person from visitor to attendee (by family)
	update p
	set p.ConnectionStatusValueId = 146 --attendee
	--select p.id, p.ConnectionStatusValueId
	from @family f
	join GroupMember gm on f.familyid = gm.GroupId
	join person p on gm.PersonId = p.Id
	where f.active = 1
	and p.ConnectionStatusValueId = 66 --visitor


	commit 

END

GO
