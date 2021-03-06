/****** Object:  StoredProcedure [dbo].[wcUtil_rebuildDatabaseIndexes]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcUtil_rebuildDatabaseIndexes]
GO
/****** Object:  StoredProcedure [dbo].[wcUtil_rebuildDatabaseIndexes]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Jorge Recio
-- Create date: 5/18/2016
-- Description:	Rebuild database indexes
-- =============================================
CREATE PROCEDURE [dbo].[wcUtil_rebuildDatabaseIndexes] 
	@Database as varchar(255),	@Fillfactor INT = 80
AS
BEGIN	
	--Declare helper parameters
	DECLARE @Table VARCHAR(255), @cmd NVARCHAR(500) 
	--Declare Table cursor that creates rebuild index comand for every table in the database 
	SET @cmd = 'DECLARE TableCursor CURSOR FOR SELECT ''['' + table_catalog + ''].['' + table_schema + ''].['' + table_name + '']'' as tableName 
				 FROM [' + @Database + '].INFORMATION_SCHEMA.TABLES 
				 WHERE table_type = ''BASE TABLE'' ORDER BY table_catalog, table_schema, table_name'   
		print  @cmd
		EXEC (@cmd)
	OPEN TableCursor   
		FETCH NEXT FROM TableCursor INTO @Table   
		WHILE @@FETCH_STATUS = 0   
			BEGIN       
				SET @cmd = 'ALTER INDEX ALL ON ' + @Table + 
						   ' REBUILD WITH (FILLFACTOR = ' + CONVERT(VARCHAR(3),@fillfactor) + ', SORT_IN_TEMPDB = ON);' 
					print @cmd
					EXEC (@cmd)      
				FETCH NEXT FROM TableCursor INTO @Table   
			END   
	CLOSE TableCursor   
	DEALLOCATE TableCursor
END

GO
