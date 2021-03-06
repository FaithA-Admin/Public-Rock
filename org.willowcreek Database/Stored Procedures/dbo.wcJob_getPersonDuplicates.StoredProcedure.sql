DROP PROCEDURE [dbo].[wcJob_getPersonDuplicates]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[wcJob_getPersonDuplicates]

AS
BEGIN
	-- TODO: The scoring methods here are arbitrary and still leave much to be desired, when a matching first/last name and email can return 90% even when the birthdate and middle name conflict

	-- If we decide to match/score based on middle name, use something like this
	-- 	, case when ISNULL(p1.MiddleName, '') like ISNULL(p2.MiddleName, '') + '%' or ISNULL(p2.MiddleName, '') like ISNULL(p1.MiddleName, '') + '%' then 98 else 50 end

	-- Get all valid Person records
	select ID = CONVERT(int, ID) -- Do not treat this as a unique identity field
		, FirstName, NickName, MiddleName, LastName, BirthYear, BirthMonth, BirthDay, GraduationYear, Email
	into #PersonRecord
	from Person
	where ConnectionStatusValueId <> 899
	and RecordTypeValueID = 1

	-- Add dummy person records using previous last name
	insert into #PersonRecord 
	select P.ID, FirstName, NickName, MiddleName, PN.LastName, BirthYear, BirthMonth, BirthDay, GraduationYear, Email
	from #PersonRecord P
	join PersonAlias PA on PA.PersonID = P.ID
	join PersonPreviousName PN on PN.PersonAliasID = PA.ID
	where P.LastName != PN.LastName

	-- Add dummy person records for each partial first name
	insert into #PersonRecord 
	select P.ID, Data, NickName, MiddleName, LastName, BirthYear, BirthMonth, BirthDay, GraduationYear, Email
	from #PersonRecord P
	cross apply dbo.Split( RTRIM(LTRIM(REPLACE(FirstName, '-', ' '))),' ')
	where RTRIM(LTRIM(REPLACE(FirstName, '-', ' '))) like '% %'

	-- Add dummy person records for each partial nick name
	insert into #PersonRecord 
	select P.ID, FirstName, Data, MiddleName, LastName, BirthYear, BirthMonth, BirthDay, GraduationYear, Email
	from #PersonRecord P
	cross apply dbo.Split( RTRIM(LTRIM(REPLACE(NickName, '-', ' '))),' ')
	where RTRIM(LTRIM(REPLACE(NickName, '-', ' '))) like '% %'

	-- Add dummy person records for each partial last name
	insert into #PersonRecord 
	select P.ID, FirstName, NickName, MiddleName, Data, BirthYear, BirthMonth, BirthDay, GraduationYear, Email
	from #PersonRecord P
	cross apply dbo.Split( RTRIM(LTRIM(REPLACE(LastName, '-', ' '))),' ')
	where RTRIM(LTRIM(REPLACE(LastName, '-', ' '))) like '% %'

	-- All First/Last Name Matches
	select P1.ID
		, ID2 = P2.ID
		, P1.FirstName
		, FirstName2 = P2.FirstName
		, P1.NickName
		, NickName2 = P2.NickName
		, P1.MiddleName
		, P1.LastName
		, LastName2 = P2.LastName
		, P1.BirthYear
		, BirthYear2 = P2.BirthYear
		, P1.BirthMonth
		, BirthMonth2 = P2.BirthMonth
		, P1.BirthDay
		, BirthDay2 = P2.BirthDay
		, P1.GraduationYear
		, GraduationYear2 = P2.GraduationYear
		, Email = case when len(P1.Email) > 1 then P1.Email else null end
		, Email2 = case when len(P2.Email) > 1 then P2.Email else null end
	into #NameMatches
	from #PersonRecord P1
	join #PersonRecord P2 on (p1.firstname = p2.firstname or p1.nickname = p2.firstname or p1.firstname = p2.nickname or p1.nickname = p2.nickname)
						  and p1.lastname = p2.lastname
						  and p1.id < p2.id

	create table #Dupes (P1ID int, P2ID int, ConfidenceScore float)

	-- Birthdate match
	insert into #Dupes
	select ID, ID2, 98
	from #NameMatches 
	where BirthYear = BirthYear2
	and BirthMonth = BirthMonth2
	and BirthDay = BirthDay2

	-- Birthday match (missing/different Birth Year)
	insert into #Dupes
	select ID, ID2, 85
	from #NameMatches 
	where BirthMonth = BirthMonth2
	and BirthDay = BirthDay2

	-- Missing birth date(s)
	insert into #Dupes
	select ID, ID2, 30
	from #NameMatches 
	where BirthMonth is null
	or BirthDay is null
	or BirthMonth2 is null
	or BirthDay2 is null

	-- Grade match
	insert into #Dupes
	select ID, ID2, 75
	from #NameMatches 
	where GraduationYear = GraduationYear2

	-- Email match
	insert into #Dupes
	select N.ID, N.ID2, 90 
	from #NameMatches N
	left join AttributeValue V1 on N.ID = V1.EntityID and V1.AttributeID = 2310 and len(V1.Value) > 1 -- Other Email
	left join AttributeValue V2 on N.ID2 = V2.EntityID and V2.AttributeID = 2310 and len(V2.Value) > 1 -- Other Email
	where Email = Email2
	or Email = V2.Value
	or Email2 = V1.Value

	-- Phone Number match
	insert into #Dupes
	select N.ID, N.ID2, 90
	from #NameMatches N
	join PhoneNumber P1 on N.ID = P1.PersonID
	join PhoneNumber P2 on N.ID2 = P2.PersonID
	where P1.Number = P2.Number

	-- Address match
	insert into #Dupes
	select N.ID, N.ID2, 90
	from #NameMatches N
	join GroupMember GM1 on N.ID = GM1.PersonID
	join [Group] G1 on GM1.GroupID = G1.ID and G1.GroupTypeId = 10
	join GroupLocation GL1 on G1.ID = GL1.GroupID
	join Location L1 on GL1.LocationID = L1.ID
	join GroupMember GM2 on N.ID2 = GM2.PersonID
	join [Group] G2 on GM2.GroupID = G2.ID and G2.GroupTypeId = 10
	join GroupLocation GL2 on G2.ID = GL2.GroupID
	join Location L2 on GL2.LocationID = L2.ID
	where UPPER(LEFT(dbo.fn_StripCharacters(L1.Street1, '^a-z0-9'), 6)) = UPPER(LEFT(dbo.fn_StripCharacters(L2.Street1, '^a-z0-9'), 6))
	and UPPER(LEFT(dbo.fn_StripCharacters(L1.PostalCode, '^a-z0-9'), 5)) = UPPER(LEFT(dbo.fn_StripCharacters(L2.PostalCode, '^a-z0-9'), 5))

	-- Get all the current duplicates with their person IDs
	select PD.*, PersonID = PA1.PersonID, DuplicatePersonID = PA2.PersonID
	into #ExistingDupes
	from PersonDuplicate PD
	join PersonAlias PA1 on PA1.ID = PD.PersonAliasID
	join PersonAlias PA2 on PA2.ID = PD.DuplicatePersonAliasId

	-- Combine all the possible new dupes and choose the highest score for each
	select D.P1ID, D.P2ID, ConfidenceScore = MAX(D.ConfidenceScore)
	into #NewDupes
	from #Dupes D
	group by P1ID, P2ID

	-- Insert Totally new matches
	insert into [PersonDuplicate] ( PersonAliasId, DuplicatePersonAliasId, ConfidenceScore, CreatedDateTime, ModifiedDateTime, [Guid], IsConfirmedAsNotDuplicate, IgnoreUntilScoreChanges)
	select PA1.ID, PA2.ID, ConfidenceScore, getdate(), getdate(), newid(), 0, 0
	from #NewDupes D
	join PersonAlias PA1 on D.P1ID = PA1.AliasPersonID
	join PersonAlias PA2 on D.P2ID = PA2.AliasPersonID
	where not exists (
		select * from #ExistingDupes E 
		where ((D.P1ID = E.PersonID and D.P2ID = E.DuplicatePersonID) or (D.P1ID = E.DuplicatePersonID and D.P2ID = E.PersonID))
		)

	-- Update Matches that have changed score
	update PD
	set ConfidenceScore = D.ConfidenceScore, ModifiedDateTime = getdate(), Score = null, ScoreDetail = null, IgnoreUntilScoreChanges = 0
	from PersonDuplicate PD
	join PersonAlias PA1 on PA1.ID = PD.PersonAliasID
	join PersonAlias PA2 on PA2.ID = PD.DuplicatePersonAliasId
	join #NewDupes D on ((D.P1ID = PA1.PersonID and D.P2ID = PA2.PersonID) or (D.P1ID = PA2.PersonID and D.P2ID = PA1.PersonID)) 
						and PD.IsConfirmedAsNotDuplicate = 0 
						and D.ConfidenceScore != PD.ConfidenceScore

	drop table #Dupes
	drop table #NewDupes
	drop table #ExistingDupes
	drop table #NameMatches
	drop table #PersonRecord
END

GO


