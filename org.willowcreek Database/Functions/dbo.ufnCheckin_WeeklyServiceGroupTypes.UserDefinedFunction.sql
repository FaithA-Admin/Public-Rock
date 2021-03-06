/****** Object:  UserDefinedFunction [dbo].[ufnCheckin_WeeklyServiceGroupTypes]    Script Date: 6/13/2016 11:25:28 AM ******/
DROP FUNCTION [dbo].[ufnCheckin_WeeklyServiceGroupTypes]
GO
/****** Object:  UserDefinedFunction [dbo].[ufnCheckin_WeeklyServiceGroupTypes]    Script Date: 6/13/2016 11:25:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

    /*
    <doc>
	    <summary>
 		    This function returns all group types that are used to denote 
		    groups that are for tracking attendance for weekly services
	    </summary>

	    <returns>
		    * GroupTypeId
		    * Guid
		    * Name
	    </returns>

	    <code>
		    SELECT * FROM [dbo].[ufnCheckin_WeeklyServiceGroupTypes]()
	    </code>
    </doc>
    */


    CREATE FUNCTION [dbo].[ufnCheckin_WeeklyServiceGroupTypes]()
    RETURNS TABLE AS

    RETURN ( 
	    SELECT [Id], [Guid], [Name]
	    FROM [GroupType] 
	    WHERE [AttendanceCountsAsWeekendService] = 1
    )

GO
