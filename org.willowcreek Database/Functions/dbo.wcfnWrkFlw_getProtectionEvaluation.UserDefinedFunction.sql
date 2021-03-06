/****** Object:  UserDefinedFunction [dbo].[wcfnWrkFlw_getProtectionEvaluation]    Script Date: 6/13/2016 11:25:28 AM ******/
DROP FUNCTION [dbo].[wcfnWrkFlw_getProtectionEvaluation]
GO
/****** Object:  UserDefinedFunction [dbo].[wcfnWrkFlw_getProtectionEvaluation]    Script Date: 6/13/2016 11:25:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[wcfnWrkFlw_getProtectionEvaluation]
(
	@status VARCHAR(36),
	@bc DATETIME NULL,
	@ref1 DATETIME NULL,
	@ref2 DATETIME NULL,
	@ref3 DATETIME NULL,
	@app DATETIME NULL,
	@policy DATETIME NULL,
	@isRestricted BIT
) 

RETURNS VARCHAR(36) AS

BEGIN

	DECLARE @inProgress VARCHAR(36) = 'E4A8815E-6300-45D0-A07F-068698A39638'
	DECLARE @approved VARCHAR(36) = '74603219-8636-4166-A684-AB7621419723'
	DECLARE @approvedWithRestrictions VARCHAR(36) = '51DCE5F0-4FDF-4A5A-86AD-93E25A899593'
	DECLARE @expired VARCHAR(36) = '9A71A268-5150-4560-A151-C364C21B63BA'
	DECLARE @needsReview VARCHAR(36) = '787DFC12-FB8B-4CEB-B98A-FE8125CBB665'

	DECLARE @now DATETIME = GETDATE()

	DECLARE @newStatus VARCHAR(36) = @status

	DECLARE @ok INT = 0

	DECLARE @refsOk INT = 0
	DECLARE @ref1Ok INT = 0
	DECLARE @ref2Ok INT = 0
	DECLARE @ref3Ok INT = 0
	IF @ref1 > DATEADD(YEAR,-5,@now)
		SET @ref1Ok = 1
	IF @ref2 > DATEADD(YEAR,-5,@now)
		SET @ref2Ok = 1
	IF @ref3 > DATEADD(YEAR,-5,@now)
		SET @ref3Ok = 1
	IF @ref1Ok + @ref2Ok + @ref3Ok >= 2
		SET @refsOk = 1

	IF @bc > DATEADD(YEAR,-1,@now)
		AND @refsOk = 1
		AND @app > DATEADD(YEAR,-5,@now)
		AND @policy > DATEADD(YEAR,-5,@now)
			SET @ok = 1

	IF @ok = 1 AND (@status = @inProgress OR @status = @expired) AND @isRestricted != 1
		SET @newStatus = @approved
	ELSE IF @ok = 1 AND @status = @inProgress AND @isRestricted = 1
		SET @newStatus = @needsReview
	ELSE IF @ok = 0 AND (@status = @approved OR @status = @approvedWithRestrictions)
		SET @newStatus = @expired

	RETURN @newStatus
END


GO
