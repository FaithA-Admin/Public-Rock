/****** Object:  UserDefinedFunction [dbo].[wcfniCal_GetDateTime]    Script Date: 6/13/2016 11:25:28 AM ******/
DROP FUNCTION [dbo].[wcfniCal_GetDateTime]
GO
/****** Object:  UserDefinedFunction [dbo].[wcfniCal_GetDateTime]    Script Date: 6/13/2016 11:25:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



 
CREATE FUNCTION [dbo].[wcfniCal_GetDateTime](@EventiCalendarContent nvarchar(max), @DateField varchar(20)) 
 RETURNS datetime
 AS 
 BEGIN 
    SET @DateField = @Datefield + ':';
	RETURN CAST(REPLACE(STUFF(STUFF(SUBSTRING(@EventiCalendarContent, CHARINDEX(@DateField, @EventiCalendarContent)+LEN(@DateField), 15), 14, 0, ':'), 12, 0, ':'), 'T', ' ') AS datetime)
 END; 




GO
