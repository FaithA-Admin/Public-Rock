/****** Object:  UserDefinedFunction [dbo].[ufnUtility_GetSundaysBetweenDates]    Script Date: 6/13/2016 11:25:28 AM ******/
DROP FUNCTION [dbo].[ufnUtility_GetSundaysBetweenDates]
GO
/****** Object:  UserDefinedFunction [dbo].[ufnUtility_GetSundaysBetweenDates]    Script Date: 6/13/2016 11:25:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
<doc>
	<summary>
 		This function returns a list of Sundays between two dates
	</summary>

	<returns>
		* SundayDate - datetime
	</returns>
	<remarks>
		WARNING: Depending if you are asking for more than 100 weeks you'll need to add OPTION (MAXRECURSION 1000) to your call 
	</remarks>
	<code>
		SELECT * FROM [dbo].[ufnUtility_GetSundaysBetweenDates](DATEADD(week, -24, getdate()), getdate())
	</code>
</doc>
*/


CREATE FUNCTION [dbo].[ufnUtility_GetSundaysBetweenDates](@StartDate datetime, @EndDate datetime)
RETURNS TABLE AS

RETURN ( WITH
	
	cteAllDates AS
	(
		SELECT dbo.ufnUtility_GetSundayDate(@StartDate) AS DateOf
			UNION ALL
			SELECT DATEADD(day, 7, DateOf)
				FROM cteAllDates
				WHERE
				DATEADD(day, 7, DateOf) <= @EndDate
	)

	-- select out Sundays in a way that works across SQL Server setups
	SELECT DateOf AS [SundayDate]
		FROM cteAllDates  ) 
GO
