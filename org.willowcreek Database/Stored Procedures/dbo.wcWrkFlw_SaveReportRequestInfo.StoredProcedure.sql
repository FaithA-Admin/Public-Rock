/****** Object:  StoredProcedure [dbo].[wcWrkFlw_SaveReportRequestInfo]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcWrkFlw_SaveReportRequestInfo]
GO
/****** Object:  StoredProcedure [dbo].[wcWrkFlw_SaveReportRequestInfo]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Terry Schonbachler
-- Create date: 10/12/2015
-- Description:	Insert report request information to determine how often reports are being requested.
-- =============================================
CREATE PROCEDURE [dbo].[wcWrkFlw_SaveReportRequestInfo]
	-- Add the parameters for the stored procedure here
	@ReportName nVARCHAR(100), 
	@ReportRequestor nVARCHAR(50)
	--@ReportDateRequested DATETIME
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    INSERT INTO [dbo].[_org_willowcreek_Report_RequestInfo] (ReportName, ReportRequestor, ReportDateRequested)
	VALUES (@ReportName, @ReportRequestor, GETDATE())
END

GO
