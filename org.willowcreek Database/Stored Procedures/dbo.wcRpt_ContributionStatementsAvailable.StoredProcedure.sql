drop procedure wcRpt_ContributionStatementsAvailable
go
create procedure wcRpt_ContributionStatementsAvailable
	@PersonID int
	, @AccountGuids varchar(max) = null
as begin
	-- We don't want to be able to print statements for anything before this year because we don't have complete data in the system
	declare @MinYear int = 2016 

	select distinct Year = year(transactiondatetime), A.IsTaxDeductible, AccountGuid = A.Guid
	into #Years
	from Person P
	join GroupMember GM on P.ID = GM.PersonID
	join [Group] G on GM.GroupID = G.ID and G.GroupTypeId = 10 -- Family
	join GroupMember GM2 on G.ID = GM2.GroupID
	join Person P2 on GM2.PersonID = P2.ID and P.GivingId = P2.GivingID
	join wcView_Contributions C on P2.ID = C.PersonID
	join FinancialTransactionDetail TD on TD.TransactionId = C.Id
	join FinancialAccount A on TD.AccountId = A.ID
	where P.ID = @PersonID

	delete from #Years where Year < @MinYear

	if LEN(ISNULL(@AccountGuids, '')) > 0 begin
		select distinct Year
		from #Years 
		where AccountGuid IN (select RTRIM(LTRIM(data)) from dbo.Split(@AccountGuids, ','))
		order by Year desc
	end
	else begin
		select distinct Year
		from #Years 
		where IsTaxDeductible = 1 
		order by Year desc
	end

	drop table #Years
end