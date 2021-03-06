/****** Object:  UserDefinedFunction [dbo].[ufnUtility_CsvToTable]    Script Date: 6/13/2016 11:25:28 AM ******/
DROP FUNCTION [dbo].[ufnUtility_CsvToTable]
GO
/****** Object:  UserDefinedFunction [dbo].[ufnUtility_CsvToTable]    Script Date: 6/13/2016 11:25:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

/*
<doc>
	<summary>
 		This function converts a comma-delimited string of ints into a table of ints
        It comes from http://www.sql-server-helper.com/functions/comma-delimited-to-table
	</summary>
	<returns>
		* id
	</returns>
	<remarks>
		Used by spFinance_ContributionStatementQuery
	</remarks>
	<code>
		SELECT * FROM [dbo].[ufnUtility_CsvToTable]('1,3,7,11,13') 
	</code>
</doc>
*/
CREATE FUNCTION [dbo].[ufnUtility_CsvToTable] 
( 
    @Input varchar(max) 
)
RETURNS @OutputTable table ( [id] int )
AS
BEGIN
    DECLARE @Numericstring varchar(10)

    WHILE LEN(@Input) > 0
    BEGIN
        SET @Numericstring= LEFT(@input, ISNULL(NULLIF(CHARINDEX(',', @Input) - 1, -1), LEN(@Input)))
        SET @Input = SUBSTRING(@input,ISNULL(NULLIF(CHARINDEX(',', @Input), 0), LEN(@Input)) + 1, LEN(@Input))

        INSERT INTO @OutputTable ( [id] )
        VALUES ( CAST(@Numericstring as int) )
    END
    
    RETURN
END

GO
