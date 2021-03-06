/****** Object:  UserDefinedFunction [dbo].[ufnUtility_GetLastDayOfMonth]    Script Date: 6/13/2016 11:25:28 AM ******/
DROP FUNCTION [dbo].[ufnUtility_GetLastDayOfMonth]
GO
/****** Object:  UserDefinedFunction [dbo].[ufnUtility_GetLastDayOfMonth]    Script Date: 6/13/2016 11:25:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/*
<doc>
	<summary>
 		This function returns the date of the last day of the month.
	</summary>

	<returns>
		Datetime of the last day of the month.
	</returns>
	<remarks>
		
	</remarks>
	<code>
		SELECT [dbo].[ufnUtility_GetLastDayOfMonth](getdate())
	</code>
</doc>
*/

CREATE FUNCTION [dbo].[ufnUtility_GetLastDayOfMonth](@InputDate datetime) 

RETURNS datetime AS

BEGIN

	RETURN DATEADD(d, -1, DATEADD(m, DATEDIFF(m, 0, @InputDate) + 1, 0))
END
GO
