CREATE PROCEDURE wcWrkFlw_PushpayReport 
	@StartDate datetime, 
	@EndDate datetime
AS BEGIN

	-- CurrencyTypeValueID
	declare @Credit int = 156
	declare @ACH int = 157

	select -- OLG
		BatchDate = convert(date, FB.BatchStartDateTime)
		, ImportDate = convert(date, FB.CreatedDateTime)
		, Account = FA.Name
		, BatchNumber = FB.ID
		, BatchName = FB.Name
		, Transactions = count(distinct FT.Id)
		, BatchTotal = sum(FD.Amount)
		, CurrencyTypeValueId
		, Currency = CASE CurrencyTypeValueID WHEN @Credit then 'Credit Card' WHEN @ACH THEN 'ACH' ELSE 'Other' END
	into #Batches
	from FinancialBatch FB
	join FinancialTransaction FT on FT.BatchId = FB.ID
	join FinancialTransactionDetail FD on FD.TransactionId = FT.Id
	join FinancialPaymentDetail FP on FT.FinancialPaymentDetailId = FP.Id
	join FinancialAccount FA on FD.AccountId = FA.Id
	where FB.BatchStartDateTime >= @StartDate
	and FB.BatchStartDateTime < dateadd(day, 1, @EndDate)
	and FT.TransactionTypeValueId = 53 -- Contribution
	and SourceTypeValueId = 1289
	group by convert(date, FB.CreatedDateTime), FA.Name, FB.ID, FB.Name, convert(date, FB.BatchStartDateTime), convert(date, FB.ModifiedDateTime), SourceTypeValueId, CurrencyTypeValueId
	order by convert(date, FB.BatchStartDateTime), FB.ID, FA.Name

	-- Pushpay Credit
	select BatchDate, ImportDate, Account, BatchNumber, BatchName, Transactions, BatchTotal
	from #Batches
	where CurrencyTypeValueID = @Credit
	order by BatchDate, BatchNumber, BatchName

	select BatchDate, ImportDate, BatchNumber, BatchName, Transactions = SUM(Transactions), BatchTotal = SUM(BatchTotal)
	from #Batches
	where CurrencyTypeValueID = @Credit
	group by BatchDate, ImportDate, BatchNumber, BatchName 
	order by BatchDate, BatchNumber, BatchName

	-- Pushpay ACH
	select BatchDate, ImportDate, Account, BatchNumber, BatchName, Transactions, BatchTotal
	from #Batches
	where CurrencyTypeValueID = @ACH
	order by BatchDate, BatchNumber, BatchName

	select BatchDate, ImportDate, BatchNumber, BatchName, Transactions = SUM(Transactions), BatchTotal = SUM(BatchTotal)
	from #Batches
	where CurrencyTypeValueID = @ACH
	group by BatchDate, ImportDate, BatchNumber, BatchName 
	order by BatchDate, BatchNumber, BatchName

	drop table #Batches
END
