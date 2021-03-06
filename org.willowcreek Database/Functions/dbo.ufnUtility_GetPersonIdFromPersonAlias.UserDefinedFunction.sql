/****** Object:  UserDefinedFunction [dbo].[ufnUtility_GetPersonIdFromPersonAlias]    Script Date: 6/13/2016 11:25:28 AM ******/
DROP FUNCTION [dbo].[ufnUtility_GetPersonIdFromPersonAlias]
GO
/****** Object:  UserDefinedFunction [dbo].[ufnUtility_GetPersonIdFromPersonAlias]    Script Date: 6/13/2016 11:25:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

    /*
    <doc>
	    <summary>
 		    This function returns the person id for the person alias given.
	    </summary>

	    <returns>
		    Int of the person id
	    </returns>
	    <remarks>
		
	    </remarks>
	    <code>
		    SELECT [dbo].[ufnUtility_GetPersonIdFromPersonAlias](1)
	    </code>
    </doc>
    */

    CREATE FUNCTION [dbo].[ufnUtility_GetPersonIdFromPersonAlias](@PersonAlias int) 

    RETURNS int AS

    BEGIN

	    RETURN (SELECT [PersonId] FROM PersonAlias WHERE [Id] = @PersonAlias)
    END

GO
