-- wcRpt_CurrentCheckInGroupList 161, 2340, '08/02/2017'
alter procedure wcRpt_CurrentCheckInGroupList
	@CheckinTypeId int,
	@ScheduleId int,
	@StartDateTime datetime
as begin
	
	declare @EnRoute int -- 68691
	select @EnRoute = id from location where name = 'En Route' and LocationTypeValueId = 183

	select PA.PersonID, LocationId, GroupTypeId = AREA.Id, StartDateTime, EndDateTime
		, Row = row_number() over (
			partition by PA.PersonID 
			order by 
				case when EndDateTime is null and locationId != @EnRoute then 0 else 1 end, -- In Room
				case when locationId = @EnRoute then 0 else 1 end, -- En Route
				coalesce(EndDateTime, StartDateTime) desc -- Most recent activity
			)
	into #People
	from Attendance A
	join PersonAlias PA on A.PersonAliasId = PA.Id
	join [Group] G on A.GroupId = G.ID
	join GroupType AREA on G.GroupTypeId = AREA.Id
	where A.ScheduleId = @ScheduleId
	and DidAttend = 1
	and StartDateTime >= @StartDateTime
	and StartDateTime < DATEADD(day, 1, CONVERT(DATE, @StartDateTime))

	-- Create attendance totals
	select GroupTypeId
		, LocationId
		, EnRoute = sum(case when LocationId = @EnRoute and EndDateTime is null then 1 else 0 end)
		, InRoom = sum(case when LocationId != @EnRoute and EndDateTime is null then 1 else 0 end)
		, CheckedOut = sum(case when EndDateTime is not null then 1 else 0 end)
		, Total = count(*)
	into #TotalsByGroup
	from #People where row = 1
	group by GroupTypeId, LocationId

	select Distinct Area = AREA.Name
		, AreaOrder = AREA.[Order]
		, LocationId = L.ID
		, Location = L.Name
		, EnRoute
		, InRoom
		, CheckedOut
		, Total
	into #results
	from GroupType CFG
	join GroupTypeAssociation GTA on GTA.GroupTypeId = CFG.Id
	join GroupType AREA on GTA.ChildGroupTypeId = AREA.Id
	join [Group] G on G.GroupTypeId = AREA.Id
	join GroupLocation GL on G.ID = GL.GroupId
	join Location L on GL.LocationID = L.ID
	left join #TotalsByGroup T on T.GroupTypeId = AREA.ID and (T.LocationId = L.ID or T.LocationId = @EnRoute)
	where CFG.Id = @CheckinTypeId
	and L.ID <> @EnRoute
	--order by AREA.[Order]
	union all
	select Distinct Area = AREA.Name
		, AreaOrder = AREA.[Order]
		, LocationId = L.ID
		, Location = L.Name
		, EnRoute
		, InRoom
		, CheckedOut
		, Total
	from GroupType CFG
	join GroupTypeAssociation GTA on GTA.GroupTypeId = CFG.Id
	join GroupType AREA on GTA.ChildGroupTypeId = AREA.Id
	join GroupTypeAssociation GTA2 on GTA2.GroupTypeId = AREA.Id
	join GroupType DEF on GTA2.ChildGroupTypeId = DEF.Id
	join [Group] G on G.GroupTypeId = DEF.Id
	join GroupLocation GL on G.ID = GL.GroupId
	join Location L on GL.LocationID = L.ID
	left join #TotalsByGroup T on T.GroupTypeId = DEF.ID and (T.LocationId = L.ID or T.LocationId = @EnRoute)
	where CFG.Id = @CheckinTypeId
	and L.ID <> @EnRoute
	order by AREA.[Order]

	select Area, AreaOrder, LocationId, Location
	    , EnRoute = SUM(ISNULL(EnRoute, 0))
		, InRoom = SUM(ISNULL(InRoom, 0))
		, CheckedOut = SUM(ISNULL(CheckedOut, 0))
		, Total = SUM(ISNULL(Total, 0))
	from #Results
	group by Area, AreaOrder, LocationId, Location
	union all
		select 'Total', 999, null, null
		    , EnRoute = SUM(ISNULL(EnRoute, 0))
		, InRoom = SUM(ISNULL(InRoom, 0))
		, CheckedOut = SUM(ISNULL(CheckedOut, 0))
		, Total = SUM(ISNULL(Total, 0))
	from #TotalsByGroup
	order by AreaOrder



	drop table #People
	drop table #TotalsByGroup
end