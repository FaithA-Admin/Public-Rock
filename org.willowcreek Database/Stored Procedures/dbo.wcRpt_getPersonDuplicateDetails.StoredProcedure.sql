/****** Object:  StoredProcedure [dbo].[wcRpt_getPersonDuplicateDetails]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcRpt_getPersonDuplicateDetails]
GO
/****** Object:  StoredProcedure [dbo].[wcRpt_getPersonDuplicateDetails]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Duplicate List for Excel
CREATE PROCEDURE [dbo].[wcRpt_getPersonDuplicateDetails]
	@ConfidenceScore DECIMAL(5,2)
AS
BEGIN
	SET NOCOUNT ON;

	--spCrm_PersonDuplicateFinder

	declare @t table ( DupId int, Score decimal(5,2),
					   FamilyId1 int, FamilyName1 nvarchar(100), PersonId1 int, LastName1 nvarchar(50), FirstName1 nvarchar(50), Nickname1 nvarchar(50), BirthDate1 date, Email1 nvarchar(75), Street1 nvarchar(100), City1 nvarchar(50), State1 nvarchar(50), PostalCode1 nvarchar(50),
					   Mobile1 nvarchar(50), Work1 nvarchar(50), Home1 nvarchar(50),
					   FamilyId2 int, FamilyName2 nvarchar(100), PersonId2 int, LastName2 nvarchar(50), FirstName2 nvarchar(50), Nickname2 nvarchar(50), BirthDate2 date, Email2 nvarchar(75), Street2 nvarchar(100), City2 nvarchar(50), State2 nvarchar(50), PostalCode2 nvarchar(50),
					   Mobile2 nvarchar(50), Work2 nvarchar(50), Home2 nvarchar(50))

	insert into @t
	select distinct pd.Id DupId, CAST(pd.ConfidenceScore AS DECIMAL(5,2)) Score,
		   g1.Id, g1.Name, p1.Id, p1.LastName, p1.FirstName, p1.Nickname, p1.BirthDate, p1.email, l1.Street1, l1.City, l1.[State], l1.PostalCode,
		   (select NumberFormatted from PhoneNumber where personid = p1.Id and numbertypevalueid = 12) Mobile,
		   (select NumberFormatted from PhoneNumber where personid = p1.Id and numbertypevalueid = 136) Work,
		   (select NumberFormatted from PhoneNumber where personid = p1.Id and numbertypevalueid = 12) Home,

		   g2.Id, g2.Name, p2.Id, p2.LastName, p2.FirstName, p2.Nickname, p2.BirthDate, p2.email, l2.Street1, l2.City, l2.[State], l2.PostalCode,
		   (select NumberFormatted from PhoneNumber where personid = p2.Id and numbertypevalueid = 12) Mobile,
		   (select NumberFormatted from PhoneNumber where personid = p2.Id and numbertypevalueid = 136) Work,
		   (select NumberFormatted from PhoneNumber where personid = p2.Id and numbertypevalueid = 12) Home

	from PersonDuplicate pd

	join PersonAlias pa1 on pd.PersonAliasId = pa1.Id 
	join Person p1 on pa1.PersonId = p1.Id
	join GroupMember gm1 on p1.Id = gm1.PersonId
	join [Group] g1 on gm1.GroupId = g1.Id and g1.GroupTypeId = 10		--if in more than one family
	left join GroupLocation gl1 on g1.Id = gl1.GroupId					--if you have more than one address
	left join Location l1 on gl1.LocationId = l1.Id

	join PersonAlias pa2 on pd.DuplicatePersonAliasId = pa2.Id 
	join Person p2 on pa2.PersonId = p2.Id
	join GroupMember gm2 on p2.Id = gm2.PersonId
	join [Group] g2 on gm2.GroupId = g2.Id and g2.GroupTypeId = 10
	left join GroupLocation gl2 on g2.Id = gl2.GroupId
	left join Location l2 on gl2.LocationId = l2.Id

	where pd.IsConfirmedAsNotDuplicate = 0
	and pd.IgnoreUntilScoreChanges = 0
	--and pd.ConfidenceScore >= @ConfidenceScore

	--order by g1.Id, p1.FirstName

	select * 
	from @t
	where Score >= @ConfidenceScore
	--order by DupId
	order by FamilyId1, FirstName1

END
GO
