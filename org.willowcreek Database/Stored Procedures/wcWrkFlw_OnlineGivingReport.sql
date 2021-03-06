ALTER PROCEDURE [dbo].[wcWrkFlw_OnlineGivingReport] 
	@StartDate datetime, 
	@EndDate datetime
AS BEGIN

	select BatchDate = convert(date, FB.BatchStartDateTime)
		, ImportDate = convert(date, FB.CreatedDateTime)
		, Account = FA.Name
		, BatchNumber = FB.ID
		, BatchName = FB.Name
		, Transactions = count(distinct FT.Id)
		, BatchTotal = sum(FD.Amount)
	from FinancialBatch FB
	join FinancialTransaction FT on FT.BatchId = FB.ID
	join FinancialTransactionDetail FD on FD.TransactionId = FT.Id
	join FinancialPaymentDetail FP on FT.FinancialPaymentDetailId = FP.Id
	join FinancialAccount FA on FD.AccountId = FA.Id
	where FB.BatchStartDateTime >= @StartDate
	and FB.BatchStartDateTime < dateadd(day, 1, @EndDate)
	and FT.TransactionTypeValueId = 53 -- Contribution
	and SourceTypeValueId = 10
	group by convert(date, FB.CreatedDateTime), FA.Name, FB.ID, FB.Name, convert(date, FB.BatchStartDateTime), convert(date, FB.ModifiedDateTime), SourceTypeValueId, CurrencyTypeValueId
	order by convert(date, FB.BatchStartDateTime), FB.ID, FA.Name
END
