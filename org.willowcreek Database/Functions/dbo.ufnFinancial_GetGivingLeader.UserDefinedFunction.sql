/****** Object:  UserDefinedFunction [dbo].[ufnFinancial_GetGivingLeader]    Script Date: 6/13/2016 11:25:28 AM ******/
DROP FUNCTION [dbo].[ufnFinancial_GetGivingLeader]
GO
/****** Object:  UserDefinedFunction [dbo].[ufnFinancial_GetGivingLeader]    Script Date: 6/13/2016 11:25:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- create function for giving lead
                    /*
                    <doc>
	                    <summary>
 		                    This function returns the lead giving person id for a given person id and giving group id.
	                    </summary>

	                    <returns>
		                    Person Id.
	                    </returns>
	                    <remarks>
		
	                    </remarks>
	                    <code>
		                    SELECT [dbo].[ufnFinancial_GetGivingLeader]( 3, 53 )
	                    </code>
                    </doc>
                    */

                    CREATE FUNCTION [dbo].[ufnFinancial_GetGivingLeader]( @PersonId int, @GivingGroupId int ) 

                    RETURNS int AS

                    BEGIN
	
	                    DECLARE @GivingLeaderPersonId int = @PersonId

	                    IF @GivingGroupId IS NOT NULL 
	                    BEGIN
		                    SET @GivingLeaderPersonId = (
			                    SELECT TOP 1 p.[Id]
			                    FROM [GroupMember] gm
			                    INNER JOIN [GroupTypeRole] r on r.[Id] = gm.[GroupRoleId]
			                    INNER JOIN [Person] p ON p.[Id] = gm.[PersonId]
			                    WHERE gm.[GroupId] = @GivingGroupId
			                    AND p.[IsDeceased] = 0
			                    AND p.[GivingGroupId] = @GivingGroupId
			                    ORDER BY r.[Order], p.[Gender], p.[BirthYear], p.[BirthMonth], p.[BirthDay]
		                    )
	                    END

	                    RETURN COALESCE( @GivingLeaderPersonId, @PersonId )

                    END

GO
