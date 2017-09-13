USE [Rock]
GO

/****** Object:  View [dbo].[wcView_NameGender]    Script Date: 2/1/2017 10:48:49 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE view [dbo].[wcView_NameGender] as
	with NameGender as (
		select Name = FirstName, Gender from Person where Gender != 0 and RecordTypeValueID = 1 and LEN(FirstName) > 1
		union all
		select Name = NickName, Gender from Person where Gender != 0 and RecordTypeValueID = 1 and LEN(NickName) > 1 and NickName != FirstName
		),
		NameGenderCount as (select Name, Gender, Count = COUNT(*) from NameGender group by Name, Gender),
		Male as (select * from NameGenderCount where Gender = 1),
		Female as (select * from NameGenderCount where Gender = 2),
		Counts as (select Name = COALESCE(M.Name, F.Name), MaleCount = COALESCE(M.Count, 0), FemaleCount = COALESCE(F.Count, 0)
		from Male M
		full outer join Female F on M.Name = F.Name
		)
	select FirstName = Name, Gender = CASE WHEN MaleCount > FemaleCount THEN 1 else 2 end
	from Counts 
	where ((MaleCount > 1 and FemaleCount < 2) or (FemaleCount > 1 and MaleCount < 2))
	and not (MaleCount > 0 and FemaleCount > 0 and ABS(MaleCount - FemaleCount) < 10)
GO