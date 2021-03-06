/****** Object:  UserDefinedFunction [dbo].[wcfnBackgroundcheckReportId]    Script Date: 6/13/2016 11:25:28 AM ******/
DROP FUNCTION [dbo].[wcfnBackgroundcheckReportId]
GO
/****** Object:  UserDefinedFunction [dbo].[wcfnBackgroundcheckReportId]    Script Date: 6/13/2016 11:25:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Jorge Recio
-- Create date: 05/13/2016
-- Description:	Returns the report id from a backgroundcheck report link.
-- =============================================
CREATE FUNCTION [dbo].[wcfnBackgroundcheckReportId] (@ReportLink as varchar(max))
RETURNS VARCHAR(MAX)
AS
BEGIN
	DECLARE @ReportIdStartIndex as int = CHARINDEX('ReportID=',@ReportLink) + 9;
	DECLARE @ReportIdEndIndex as Int = CHARINDEX('&', @ReportLink);
	
	IF @ReportIdEndIndex > 0
		BEGIN
			RETURN SUBSTRING(@ReportLink,@ReportIdStartIndex, @ReportIdEndIndex - @ReportIdStartIndex) 
		END
	 
	RETURN NULL
	
END
GO
