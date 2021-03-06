/****** Object:  StoredProcedure [dbo].[wcsp_FindInvalidEmails]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcsp_FindInvalidEmails]
GO
/****** Object:  StoredProcedure [dbo].[wcsp_FindInvalidEmails]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[wcsp_FindInvalidEmails]
AS

DECLARE @Id INT
DECLARE @LastName VARCHAR(255)
DECLARE @FirstName VARCHAR(255)
DECLARE @Email VARCHAR(255)

DECLARE @result VARCHAR(1)
DECLARE @Cnt INT

DECLARE curs1 CURSOR DYNAMIC FOR
SELECT Id,
		LastName, 
		FirstName, 
		Email
FROM Person 
WHERE Email IS NOT NULL
AND Email <> ''
ORDER BY 1

CREATE TABLE #Temp (Id INT, Email VARCHAR(255), LastName VARCHAR(255), FirstName VARCHAR(255))

SELECT @Cnt = 1

OPEN curs1

FETCH FIRST FROM curs1 INTO @Id,
							@LastName,
							@FirstName,
                            @Email 

WHILE @@FETCH_status <> -1
	BEGIN

		SELECT @result =  dbo.wcfnUtil_TestStringWithRegularExpression(@Email, 1, 'T', 'F')
		IF (@result = 'F')
		BEGIN 
			--PRINT convert(char(6), @Id) + '   ' + @Email + '   ' + @LastName + '   ' + @FirstName
			INSERT INTO #Temp (Id , Email , LastName , FirstName )
			 SELECT @Id, @Email, @LastName, @FirstName
		END


		FETCH NEXT FROM curs1 INTO @Id,
							@LastName,
							@FirstName,
                            @Email 

		SELECT @Cnt = @Cnt + 1
 	END

CLOSE curs1
DEALLOCATE curs1

--PRINT 'FETCHCOUNT=' + convert(char(6), @fetchcount) 
--PRINT 'TOT=' + convert(char(6), @tot) 
--PRINT 'gid=' + convert(char(20), @givingentityid)
--PRINT 'pledge=' + convert(char(9), @pledgecommitment)
--PRINT 'cont=' + convert(char(9), @contributions)
--PRINT 'diff=' + convert(char(9), @difference)
--PRINT 'gidlist =' + convert(char(6), @gidlist)
PRINT 'Cnt=' + convert(char(6), @Cnt) 

SELECT * FROM #Temp
DROP TABLE #Temp

GO
