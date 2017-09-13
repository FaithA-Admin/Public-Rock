DROP PROCEDURE [dbo].wcRpt_getPersonDuplicateList
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create procedure wcRpt_getPersonDuplicateList as begin

	-- Get all duplicate records that do not reference the same person for both aliases
	select PD.ID
		, PD.ConfidenceScore
		, IsConfirmedAsNotDuplicate
		, IgnoreUntilScoreChanges
		, PersonID = P1.ID
		, DuplicatePersonID = P2.ID
	into #AllDuplicates
	from PersonDuplicate PD
	join PersonAlias PA1 on PA1.ID = PD.PersonAliasID
	join Person P1 on P1.ID = PA1.PersonID
	join PersonAlias PA2 on PA2.ID = PD.DuplicatePersonAliasId
	join Person P2 on P2.ID = PA2.PersonID
	where P1.ID != P2.ID

	-- Get a list of all confirmed non-duplicates
	select LowPersonID = case when PersonID < DuplicatePersonID then PersonID else DuplicatePersonID end
		, HighPersonID = case when PersonID > DuplicatePersonID then PersonID else DuplicatePersonID end
	into #NotDuplicates
	from #AllDuplicates
	where IsConfirmedAsNotDuplicate = 1
	group by case when PersonID < DuplicatePersonID then PersonID else DuplicatePersonID end, case when PersonID > DuplicatePersonID then PersonID else DuplicatePersonID end

	delete AD
	from #AllDuplicates AD
	where exists (select * from #NotDuplicates ND where ND.LowPersonID = AD.PersonID and ND.HighPersonID = AD.DuplicatePersonID)
	or exists (select * from #NotDuplicates ND where ND.HighPersonID = AD.PersonID and ND.LowPersonID = AD.DuplicatePersonID)
	
	-- Get a list of all that have been marked as ignore, with the highest score that should be ignored
	select LowPersonID = case when PersonID < DuplicatePersonID then PersonID else DuplicatePersonID end
		, HighPersonID = case when PersonID > DuplicatePersonID then PersonID else DuplicatePersonID end
		, ConfidenceScore = MAX(ConfidenceScore)
	into #IgnoreUntilScoreChanges
	from #AllDuplicates
	where IgnoreUntilScoreChanges = 1
	group by case when PersonID < DuplicatePersonID then PersonID else DuplicatePersonID end, case when PersonID > DuplicatePersonID then PersonID else DuplicatePersonID end

	delete AD
	from #AllDuplicates AD
	where exists (select * from #IgnoreUntilScoreChanges ND where ND.LowPersonID = AD.PersonID and ND.HighPersonID = AD.DuplicatePersonID and AD.ConfidenceScore <= ND.ConfidenceScore)
	or exists (select * from #IgnoreUntilScoreChanges ND where ND.HighPersonID = AD.PersonID and ND.LowPersonID = AD.DuplicatePersonID and AD.ConfidenceScore <= ND.ConfidenceScore)

	-- At this point #AllDuplicates will have only real duplicates

	-- Get the unique list of duplicates with some columns that will allow grouping dupes together if they have reversed person IDs
	select PersonID, DuplicatePersonID, ConfidenceScore = MAX(ConfidenceScore), Dupes = COUNT(*), LowPersonID = case when PersonID < DuplicatePersonID then PersonID else DuplicatePersonID end
		, HighPersonID = case when PersonID > DuplicatePersonID then PersonID else DuplicatePersonID end 
	into #UniqueDuplicates
	from #AllDuplicates 
	group by PersonID, DuplicatePersonID
	
	-- Format the unique list so the PersonID and DuplicatePersonID used have actual duplicate records, prioritizing high score and number of dupes
	;with d as (
		select *, row = ROW_NUMBER() over (partition by LowPersonID, HighPersonID order by ConfidenceScore desc, dupes desc) from #UniqueDuplicates
		)
	select PersonID, DuplicatePersonID, ConfidenceScore
	into #RemainingDuplicates
	from d 
	where row = 1

	-- Get a list of all contributors
	select distinct PersonID 
	into #Contributors
	from wcView_Contributions

	-- Get a list of all family members of all contributors
	select distinct GM2.PersonID
	into #ExtendedContributors
	from #Contributors C
	join GroupMember GM on GM.PersonID = C.PersonID
	join [Group] G on GM.GroupID = G.ID and G.GroupTypeId = 10
	join GroupMember GM2 on G.ID = GM2.GroupID

	-- Get a list of all contributors who have no current address on file
	select distinct C.*
	into #NoCurrentAddress
	from #Contributors C
	join GroupMember GM on GM.PersonID = C.PersonID
	join [Group] G on GM.GroupID = G.ID and G.GroupTypeId = 10
	left join GroupLocation FL on FL.GroupID = G.ID and GroupLocationTypeValueId in (19,20) -- Home, Work
	where FL.ID is null

	-- Get the final result set
	select D.*
		, PersonStatusValueID = ISNULL(P1.RecordStatusValueID, 3)
		, P1.LastName
		, P1.FirstName
		, PersonModifiedDateTime = P1.ModifiedDateTime
		, CreatedByPerson = CP.NickName + ' ' + CP.LastName
		, DuplicatePersonStatusValueID = ISNULL(P2.RecordStatusValueId, 3)
		, AccountingOnly = case 
			when EC1.PersonID is not null and EC2.PersonID is not null then 1 -- True if a family member of both person records has contributions
			when NCA1.PersonID is not null or NCA2.PersonID is not null then 1 -- True if one person is a contributor with no current address
			else 0 end
		, CareCenterOnly =  case when P1.ConnectionStatusValueId = 1449 or P2.ConnectionStatusValueId = 1449 then 1 else 0 end -- True if either member is a Care Center Guest
	from #RemainingDuplicates D
	join Person P1 on P1.ID = D.PersonID
	left join PersonAlias CPA on P1.CreatedByPersonAliasId = CPA.ID
	left join Person CP on CPA.PersonID = CP.ID
	join Person P2 on P2.ID = D.DuplicatePersonID
	left join #ExtendedContributors EC1 on P1.ID = EC1.PersonID
	left join #ExtendedContributors EC2 on P2.ID = EC2.PersonID
	left join #NoCurrentAddress NCA1 on P1.ID = NCA1.PersonID
	left join #NoCurrentAddress NCA2 on P2.ID = NCA2.PersonID
	order by ConfidenceScore desc, LastName, FirstName, PersonID
end