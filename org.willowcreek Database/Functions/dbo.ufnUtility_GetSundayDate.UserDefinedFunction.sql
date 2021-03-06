/****** Object:  UserDefinedFunction [dbo].[ufnUtility_GetSundayDate]    Script Date: 6/13/2016 11:25:28 AM ******/
DROP FUNCTION [dbo].[ufnUtility_GetSundayDate]
GO
/****** Object:  UserDefinedFunction [dbo].[ufnUtility_GetSundayDate]    Script Date: 6/13/2016 11:25:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
<doc>
	<summary>
 		This function returns the Sunday date of a given date.
	</summary>

	<returns>
		The Sunday of the date given with Sunday being the last day of the week.
	</returns>
	<remarks>
		
	</remarks>
	<code>
		SELECT [dbo].[ufnUtility_GetSundayDate](getdate())
	</code>
</doc>
*/

CREATE FUNCTION [dbo].[ufnUtility_GetSundayDate](@InputDate datetime) 

RETURNS DATE WITH SCHEMABINDING AS

BEGIN
	DECLARE @DayOfWeek int
	DECLARE @DaysToAdd int
	DECLARE @SundayDate datetime
	
	-- from http://stackoverflow.com/a/5109557/1755417 to get the day of week deterministically, but changed so that Monday is the first day of week
	SET @DayOfWeek = ((datediff(day, convert(datetime, '19000101', 112), @InputDate) % 7) + 1)

	-- calculate days to add to get to Sunday
	SET @DaysToAdd = 7 - @DayOfWeek

	SET @SundayDate = DATEADD(day, @DaysToAdd, @InputDate)

	RETURN @SundayDate
END
GO
