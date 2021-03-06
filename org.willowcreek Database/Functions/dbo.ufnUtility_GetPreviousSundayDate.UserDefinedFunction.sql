/****** Object:  UserDefinedFunction [dbo].[ufnUtility_GetPreviousSundayDate]    Script Date: 6/13/2016 11:25:28 AM ******/
DROP FUNCTION [dbo].[ufnUtility_GetPreviousSundayDate]
GO
/****** Object:  UserDefinedFunction [dbo].[ufnUtility_GetPreviousSundayDate]    Script Date: 6/13/2016 11:25:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

                    -- create function for attendance duration
                    /*
                    <doc>
	                    <summary>
 		                    This function returns the date of the previous Sunday.
	                    </summary>

	                    <returns>
		                    Datetime of the previous Sunday.
	                    </returns>
	                    <remarks>
		
	                    </remarks>
	                    <code>
		                    SELECT [dbo].[ufnUtility_GetPreviousSundayDate]()
	                    </code>
                    </doc>
                    */

                    CREATE FUNCTION [dbo].[ufnUtility_GetPreviousSundayDate]() 

                    RETURNS date AS

                    BEGIN

	                    RETURN DATEADD("day", -7, dbo.ufnUtility_GetSundayDate(getdate()))
                    END
            
GO
