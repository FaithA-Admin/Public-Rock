/****** Object:  UserDefinedFunction [dbo].[ufnUtility_GetFirstDayOfMonth]    Script Date: 6/13/2016 11:25:28 AM ******/
DROP FUNCTION [dbo].[ufnUtility_GetFirstDayOfMonth]
GO
/****** Object:  UserDefinedFunction [dbo].[ufnUtility_GetFirstDayOfMonth]    Script Date: 6/13/2016 11:25:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



/*
<doc>
	<summary>
 		This function returns the date of the first of the month.
	</summary>

	<returns>
		Datetime of the first of the month.
	</returns>
	<remarks>
		
	</remarks>
	<code>
		SELECT [dbo].[ufnUtility_GetFirstDayOfMonth](getdate())
	</code>
</doc>
*/

CREATE FUNCTION [dbo].[ufnUtility_GetFirstDayOfMonth](@InputDate datetime) 

RETURNS datetime AS

BEGIN

	RETURN DATEADD(month, DATEDIFF(month, 0, getdate()), 0)
END
GO
