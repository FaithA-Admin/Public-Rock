ALTER PROCEDURE [dbo].[wcUtil_MergePushpayBatches] 
	@StartDate DATETIME,
	@EndDate DATETIME,
	@PersonAliasID INT
AS BEGIN

	-- Move transactions
	with Survivor as (
		select rownum = row_number() over (partition by Name order by BatchStartDateTime, Id), * 
		from FinancialBatch 
		where Name like 'Pushpay%'
		and BatchStartDateTime >= @StartDate
		and BatchStartDateTime < DATEADD(DAY, 1, @EndDate)
		and Status = 1
	)
	update T
	set BatchID = S.ID
		, ModifiedByPersonAliasId = @PersonAliasID
		, ModifiedDateTime = getdate()
	from FinancialBatch B
	join Survivor S on B.Name = S.Name
	join FinancialTransaction T on T.BatchID = B.ID
	where S.rownum=1
	and B.ID != S.ID
	and B.Status = 1
	and B.BatchStartDateTime >= @StartDate
	and B.BatchStartDateTime < DATEADD(DAY, 1, @EndDate);

	-- Delete empty batches
	delete B
	from FinancialBatch B
	where Name like 'Pushpay%'
	and not exists (select * from FinancialTransaction T where T.BatchID = B.ID)
	and B.Status = 1
	and B.BatchStartDateTime >= @StartDate
	and B.BatchStartDateTime < DATEADD(DAY, 1, @EndDate);
	
	-- Fix the control amounts
	with Amounts as (
		select T.BatchID, Amount = SUM(D.Amount) 
		from FinancialTransaction T
		join FinancialTransactionDetail D on D.TransactionId = T.ID
		group by T.BatchID
		)
	update B 
	set ControlAmount = A.Amount
		, ModifiedByPersonAliasId = @PersonAliasID
		, ModifiedDateTime = getdate()
	from FinancialBatch B
	join Amounts A on A.BatchID = B.ID
	where B.Name like 'Pushpay%'
	and B.Status = 1
	and B.BatchStartDateTime >= @StartDate
	and B.BatchStartDateTime < DATEADD(DAY, 1, @EndDate);
END



select * from FinancialTransaction