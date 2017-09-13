-- wcRpt_CheckIn 161, '2017-08-19', '2017-08-20'
alter procedure wcRpt_CheckIn
	@CheckInTypeID INT
	, @StartDate DATE
	, @EndDate DATE
as begin
	declare @NoteTypeId int
	select @NoteTypeId = id from NoteType where EntityTypeId = 58

		;with Grade as (
			select GradeID = AREA.ID, GradeName = AREA.Name, GradeOrder = AREA.[Order], AREA.ID, AREA.Name, Level = 1, ConfigName = CFG.Name, ConfigID = CFG.ID
			from GroupType CFG
			join GroupTypeAssociation GTA on GTA.GroupTypeId = CFG.Id
			join GroupType AREA on GTA.ChildGroupTypeId = AREA.Id
			where CFG.GroupTypePurposeValueId = 142
			union all
			select Grade.GradeID, Grade.GradeName, Grade.GradeOrder, GT.ID, GT.Name, Level = Level + 1, ConfigName, ConfigID
			from Grade 
			join GroupTypeAssociation GTA on Grade.ID = GTA.GroupTypeId
			join GroupType GT on GTA.ChildGroupTypeId = GT.Id
		)
		select GroupTypeId = ID, ConfigID 
		into #GroupTypes
		from Grade

	-- People in a group that checked into a different group (not en route)
	-- People not in a group that checked into a non-default group

	-- All people who checked into a group they're not a member of, or that have a note
	select P.ID
		, P.NickName
		, P.LastName
		, Campus = C.Name
		, Schedule = S.Name
		, ScheduleId = S.Id
		, Room = L.Name
		, [Group] = G.Name
		, GroupId = G.ID
		, A.StartDateTime
		, Note = COALESCE(N.Text, '')
		, GT.ConfigID
	into #Report
	from attendance a
	join PersonAlias PA on A.PersonAliasId = PA.ID
	join Person P on PA.PersonID = P.ID
	left join Note n on n.NoteTypeId = @NoteTypeId and n.EntityId = a.Id
	join Location L on A.LocationId = L.ID
	join Schedule S on A.ScheduleId = S.Id
	join [Group] G on A.GroupId = G.Id
	join Campus C on A.CampusId = C.Id
	join #GroupTypes GT on G.GroupTypeId = GT.GroupTypeId
	where (
		-- Not a member of the group they checked into, or there is a note
		not exists (select * from groupmember GM where GM.GroupID = A.GroupID and GM.PersonId = PA.PersonId) 
		or (N.Id is not null and LEN(N.Text) > 0)
		)
	and a.startdatetime >= @StartDate
	and a.startdatetime < DATEADD(day, 1, @EndDate)
	and P.lastname not in ('testhousehold')
	and GT.ConfigId = @CheckInTypeID
	order by a.startdatetime

	-- Now find out if this user is a member of any groups descended from CheckInConfigId

	select distinct ConfigID, GroupID = G.ID, GLS.ScheduleId 
	into #Groups
	from [Group] G
	join GroupLocation GL on G.ID = GL.GroupId
	join GroupLocationSchedule GLS on GL.ID = GLS.GroupLocationId
	join #GroupTypes GT on G.GroupTypeId = GT.GroupTypeId
	where GLS.ScheduleId not in (2340)

	select StartDateTime
		, PersonID = R.ID
		, NickName
		, LastName
		, Schedule
		, CheckedInGroup = [Group]
		, MemberOfGroup = ISNULL(GMX.Name, '')
		, InGroupFromOtherService = case when (select count(distinct GM.GroupId) from GroupMember GM 
			join #Groups G on GM.GroupId = G.GroupID and G.ConfigID = R.ConfigID and G.ScheduleId != R.ScheduleId
			where R.ID = GM.PersonId) > 0 then 'Yes' else 'No' end
		, R.Note
	from #Report R
	left join (select GM2.PersonId, G.* from GroupMember GM2
		join [Group] G on GM2.GroupId = G.ID
		join #GroupTypes GT on G.GroupTypeId = GT.GroupTypeId and GT.ConfigID = @CheckInTypeID) GMX on GMX.PersonId = R.Id
	order by StartDateTime, R.Id

	drop table #Groups
	drop table #Report
	drop table #grouptypes
end