ALTER PROCEDURE [dbo].[wcWrkFlw_exportFinancialBatches] 
	@BatchStartDateTime DATETIME,
	@BatchEndDateTime DATETIME,
	@PersonAliasID INT
AS BEGIN

	-- Get the Attribute that stores the date a batch was exported
	DECLARE @ExportDateAttributeID INT
	SELECT @ExportDateAttributeID = ID FROM Attribute WHERE [Key] = 'DateUploadedtoMSDynamicsSL' and EntityTypeId = 77

	-- Use a consistent datetime value for all records created
	DECLARE @ExportDate DATETIME
	SET @ExportDate = GETDATE()

	-- Get all the batch data for batches in the specified date range that have not been exported yet
	SELECT Company = ca.value
		, GLAccount = LEFT(dvat.Value, 6)
        , CRProject = CASE WHEN LEN(fa.GLCode) >= 8 THEN SUBSTRING(fa.GlCode, 8, LEN(fa.GlCode) - 7) ELSE '' END
		, CRTask = '000'
		, ProjectSub = CASE WHEN LEN(fa.GLCode) >= 12 THEN SUBSTRING(fa.GlCode, 12, LEN(fa.GlCode) - 6) ELSE '' END
		, ARDist = 'A/R Dist'
		, DRAmount = CONVERT(DECIMAL(20, 2), 0)
		, CRAmount = CONVERT(DECIMAL(20, 2), ftd.Amount)
		, DepositDate = CONVERT(VARCHAR(10), CAST(fb.BatchStartDateTime AS DATE), 101)
		, Batch = fb.Name
		, BatchId = fb.Id
	INTO #Batches
	FROM FinancialBatch fb
	INNER JOIN FinancialTransaction ft ON fb.Id = ft.BatchId AND ft.TransactionTypeValueId = 53 --Contributions
	INNER JOIN FinancialTransactionDetail ftd ON ft.Id = ftd.TransactionId
	INNER JOIN FinancialAccount fa ON ftd.AccountId = fa.id
	LEFT JOIN AttributeValue c on c.AttributeId = 8510 and c.entityid = fa.id
	LEFT JOIN DefinedValue dvc on convert(varchar(max), dvc.guid) = c.Value
	LEFT JOIN AttributeValue ca on ca.AttributeId = 8509 and ca.entityid = dvc.id
	INNER JOIN DefinedValue dvat ON fa.AccountTypeValueId = dvat.id AND dvat.DefinedTypeId = 24 -- AccountType
	WHERE fb.Status = 2 --Closed
	AND fb.BatchStartDateTime >= @BatchStartDateTime 
	AND fb.BatchStartDateTime < DATEADD(DD, 1, @BatchEndDateTime)
	AND NOT EXISTS (
		select * FROM AttributeValue V 
		where V.AttributeID = @ExportDateAttributeID
		and V.ValueAsDateTime IS NOT NULL
		and V.EntityID = fb.Id
		)

	-- Consolidate batches into a single line per batch
	SELECT Company,
		GLAccount,
		CRProject,
		CRTask,
		ProjectSub,
		ARDist,
		DRAmount,
		SUM(CRAmount) AS CRAmount,
		DepositDate,
		Batch,
		BatchId
	INTO #Results
	FROM #Batches
	GROUP BY Company, DepositDate, Batch, GLAccount, CRProject, CRTask, ProjectSub, ARDist, DRAmount, BatchId

	-- Add an offsetting transaction if there are any results. This way the results will be empty if there are no real batches.
	IF (@@ROWCOUNT > 0) BEGIN
		INSERT INTO #Results (Company, GLAccount, CRProject, CRTask, ProjectSub, ARDist, DRAmount, CRAmount, DepositDate, Batch, BatchId)
		SELECT 'WCC', '111000', 'WCC-BAL-000-0000', '000', 'BAL-000-0000', 'A/R DIST', SUM(CRAmount), 0, CONVERT(VARCHAR(10), @ExportDate, 101), 'WCC OFFSETTING TRAN', -1
		FROM #Results
	END

	SELECT Company,
		GLAccount,
		CRProject,
		CRTask,
		ProjectSub,
		ARDist,
		DRAmount,
		CRAmount,
		DepositDate,
		Batch
	FROM #Results
	ORDER BY Company, DepositDate, Batch, GLAccount, CRProject

	-- Get all the batches with their associated export date attribute value
	select distinct B.BatchID, AV.ID, AV.ValueAsDateTime 
	into #AttributeValues
	from #Results B
	left join AttributeValue AV on AV.EntityID = B.BatchID and AV.AttributeID = @ExportDateAttributeID
	WHERE B.BatchId > 0
	
	-- Create the Exported attribute so that these batches will not get exported again, where any of the batches don't already have an attribute value row
	INSERT INTO AttributeValue (IsSystem, AttributeId, EntityID, Value, ValueAsDateTime, Guid, CreatedDateTime, ModifiedDateTime, CreatedByPersonAliasId, ModifiedByPersonAliasId)
	select 0, @ExportDateAttributeID, BatchId, @ExportDate, @ExportDate, NEWID(), @ExportDate, @ExportDate, @PersonAliasID, @PersonAliasID
	FROM #AttributeValues
	where ID is null

	-- Update the Exported attribute so that these batches will not get exported again, where any of the batches already have an attribute value row
	update AV
	set Value = @ExportDate
		, ValueAsDateTime = @ExportDate
		, ModifiedDateTime = @ExportDate
		, ModifiedByPersonAliasId = @PersonAliasID
	from #AttributeValues B
	join AttributeValue AV on B.ID = AV.ID
	where B.ID is not null

	DROP TABLE #Batches
	DROP TABLE #Results
	DROP TABLE #AttributeValues
END