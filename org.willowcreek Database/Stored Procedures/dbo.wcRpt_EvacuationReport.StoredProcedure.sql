-- wcRpt_EvacuationReport 161, 1112, '7/30/2017'

alter procedure wcRpt_EvacuationReport
	@CheckInTypeID INT
	, @ScheduleId INT
	, @StartDateTime DATE
as begin
	declare @EnRoute INT = (select id from Location where Guid='6318116F-3253-4E50-9B66-9995343C11A8')
	declare @ParentLocation INT = (select ID from attribute where [key] = 'AttendanceParentLocation' and entitytypeid = 58)
	
	;with Grade as (
		select GradeID = AREA.ID, GradeName = AREA.Name, GradeOrder = AREA.[Order], AREA.ID, AREA.Name, Level = 1
		from GroupType CFG
		join GroupTypeAssociation GTA on GTA.GroupTypeId = CFG.Id
		join GroupType AREA on GTA.ChildGroupTypeId = AREA.Id
		where CFG.ID = @CheckInTypeID 
		union all
		select Grade.GradeID, Grade.GradeName, Grade.GradeOrder, GT.ID, GT.Name, Level = Level + 1
		from Grade 
		join GroupTypeAssociation GTA on Grade.ID = GTA.GroupTypeId
		join GroupType GT on GTA.ChildGroupTypeId = GT.Id
	)
	select Grade = GR.GradeName 
		, GradeOrder
		, nodename = G.Name
		, FirstName = P.NickName
		, P.LastName
		, phouseholdid = AC.Code
		, ParentLocation = PL.Value
		, StartDateTime
		, ReportName = case when A.EndDateTime is not null then 'Checked Out' when A.LocationId = @EnRoute THEN 'En Route' else 'In Room' end 
		, LineOrder = case when A.EndDateTime is not null then 2 when A.LocationId = @EnRoute THEN 1 else 0 end 
		, Level
		, Row = row_number() over (
			partition by PA.PersonID 
			order by 
				case when EndDateTime is null and locationId != @EnRoute then 0 else 1 end, -- In Room
				case when locationId = @EnRoute then 0 else 1 end, -- En Route
				coalesce(EndDateTime, StartDateTime) desc -- Most recent activity
			)
	into #Detail
	from Attendance A 
	join PersonAlias PA on A.PersonAliasId = PA.ID
	join Person P on PA.PersonID = P.ID
	left join AttendanceCode AC on A.AttendanceCodeId = AC.ID
	join [Group] G on A.GroupId = G.ID
	join GroupType GT on G.GroupTypeId = GT.ID
	left join AttributeValue PL on PL.AttributeId = @ParentLocation and PL.EntityId = A.Id
	join Grade GR on GR.ID = GT.ID
	where A.ScheduleId = @ScheduleId
	and StartDateTime >= @StartDateTime
	and StartDateTime < dateadd(day, 1, @StartDateTime)

	-- Put Grades 2-3 and 4-5 together in the summary
	select Grade = case when Grade in ('Gr 2', 'Gr 3') then 'Gr 2-3' when Grade in ('Gr 4', 'Gr 5') then 'Gr 4-5' else Grade end
		, GradeOrder = case when Grade in ('Gr 3', 'Gr 5') then GradeOrder - 1 else GradeOrder end
		, NodeName, FirstName, LastName, PHouseholdId, ParentLocation, StartDateTime, ReportName, LineOrder, Level
	into #Detail2
	from #Detail
	where Row = 1

	select GradeOrder, LineOrder, Grade, ReportName, Total = COUNT(*)
	from #Detail2
	group by GradeOrder, LineOrder, Grade, ReportName
	order by GradeOrder, LineOrder

	select * from #Detail
	where Row = 1
	order by GradeOrder, NodeName, LineOrder, LastName, FirstName

	drop table #Detail
	drop table #Detail2
end