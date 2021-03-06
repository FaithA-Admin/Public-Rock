/****** Object:  StoredProcedure [dbo].[spRock_PLAttendance]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[spRock_PLAttendance]
GO
/****** Object:  StoredProcedure [dbo].[spRock_PLAttendance]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spRock_PLAttendance]
	@Campus INT, 
	@Year nVARCHAR(4)
AS
BEGIN

WITH parentgroup AS (
	SELECT ParentGroupId, Name, GroupTypeID
	FROM [Group]
	WHERE ParentGroupId IN (@Campus)	-- SB PL | Grades	-- (53, 66, 67, 58, 26948, 68)	-- All regional PL | Grades
), Tree AS (
	SELECT x.ParentGroupId, x.Id, x.Name, x.GroupTypeID
	FROM [Group] x
	INNER JOIN parentgroup pg
		ON x.ParentGroupId = pg.ParentGroupId
	WHERE x.ParentGroupId NOT IN (25228)	-- SB PL | Grades | Holiday Gold Teams		-- (25228, 25329)	-- Exclude PL | Grades |
	UNION ALL
	SELECT y.ParentGroupId, y.Id, y.Name, y.GroupTypeID
	FROM [Group] y
	INNER JOIN Tree t
		ON y.ParentGroupId = t.id
	WHERE y.ParentGroupId NOT IN (25228)	-- SB PL | Grades | Holiday Gold Teams		-- (25228, 25329)	-- Exclude PL | Grades |
)

SELECT DISTINCT * 
INTO #TempPL
FROM tree 
ORDER BY 1, 2, 3

DECLARE @AttendedStartDate DATETIME
DECLARE @AttendedEndDate DATETIME

DECLARE @AttendedStartDateSat DATETIME
DECLARE @AttendedStartDateSun DATETIME

DECLARE @AttendedStartDateHold DATETIME

DECLARE @CampusCheckIn INT

SELECT @AttendedStartDate = '01/01/' + @Year + ' 00:00:00'
SELECT @AttendedEndDate = '12/31/' + @Year + ' 23:59:59'



IF @Campus = 47	--SB
BEGIN
	SELECT @AttendedStartDate = CONVERT(DATETIME, '01/01/' + CAST(DATEPART(yyyy, @AttendedStartDate) AS VARCHAR(4)) + ' 17:30:00', 101)
	SELECT @AttendedStartDateSat = @AttendedStartDate
	
--	SELECT @AttendedStartDate = CONVERT(DATETIME, '01/01/' + CAST(DATEPART(yyyy, @AttendedStartDate) AS VARCHAR(4)) + ' 09:00:00', 101)
--	SELECT @AttendedStartDateSun = @AttendedStartDate

--	SELECT @AttendedStartDate = CONVERT(DATETIME, '01/01/' + CAST(DATEPART(yyyy, @AttendedStartDate) AS VARCHAR(4)) + ' 11:15:00', 101)
--	SELECT @AttendedStartDateSun = @AttendedStartDate

	SELECT @CampusCheckIn = 1
END

IF @Campus = 59	--NSH
BEGIN	
	SELECT @AttendedStartDate = CONVERT(DATETIME, '01/01/' + CAST(DATEPART(yyyy, @AttendedStartDate) AS VARCHAR(4)) + ' 08:30:00', 101)
	SELECT @AttendedStartDateSun = @AttendedStartDate

--	SELECT @AttendedStartDate = CONVERT(DATETIME, '01/01/' + CAST(DATEPART(yyyy, @AttendedStartDate) AS VARCHAR(4)) + ' 10:00:00', 101)
--	SELECT @AttendedStartDateSun = @AttendedStartDate

--	SELECT @AttendedStartDate = CONVERT(DATETIME, '01/01/' + CAST(DATEPART(yyyy, @AttendedStartDate) AS VARCHAR(4)) + ' 11:30:00', 101)
--	SELECT @AttendedStartDateSun = @AttendedStartDate

	SELECT @CampusCheckIn = 4
END


IF @Campus = 56	--DuP
BEGIN	
	SELECT @AttendedStartDate = CONVERT(DATETIME, '01/01/' + CAST(DATEPART(yyyy, @AttendedStartDate) AS VARCHAR(4)) + ' 09:00:00', 101)
	SELECT @AttendedStartDateSun = @AttendedStartDate

--	SELECT @AttendedStartDate = CONVERT(DATETIME, '01/01/' + CAST(DATEPART(yyyy, @AttendedStartDate) AS VARCHAR(4)) + ' 11:15:00', 101)
--	SELECT @AttendedStartDateSun = @AttendedStartDate

--	SELECT @AttendedStartDate = CONVERT(DATETIME, '01/01/' + CAST(DATEPART(yyyy, @AttendedStartDate) AS VARCHAR(4)) + ' 23:59:59', 101)
--	SELECT @AttendedStartDateSun = @AttendedStartDate

	SELECT @CampusCheckIn = 2
END


IF @Campus = 64	--Chi
BEGIN	
	SELECT @AttendedStartDate = CONVERT(DATETIME, '01/01/' + CAST(DATEPART(yyyy, @AttendedStartDate) AS VARCHAR(4)) + ' 10:00:00', 101)
	SELECT @AttendedStartDateSun = @AttendedStartDate

--	SELECT @AttendedStartDate = CONVERT(DATETIME, '01/01/' + CAST(DATEPART(yyyy, @AttendedStartDate) AS VARCHAR(4)) + ' 23:59:59', 101)
--	SELECT @AttendedStartDateSun = @AttendedStartDate

--	SELECT @AttendedStartDate = CONVERT(DATETIME, '01/01/' + CAST(DATEPART(yyyy, @AttendedStartDate) AS VARCHAR(4)) + ' 23:59:59', 101)
--	SELECT @AttendedStartDateSun = @AttendedStartDate

	SELECT @CampusCheckIn = 32
END


IF @Campus = 61	--CLK
BEGIN	

	SELECT @AttendedStartDate = CONVERT(DATETIME, '01/01/' + CAST(DATEPART(yyyy, @AttendedStartDate) AS VARCHAR(4)) + ' 17:30:00', 101)
	SELECT @AttendedStartDateSat = @AttendedStartDate

--	SELECT @AttendedStartDate = CONVERT(DATETIME, '01/01/' + CAST(DATEPART(yyyy, @AttendedStartDate) AS VARCHAR(4)) + ' 09:00:00', 101)
--	SELECT @AttendedStartDateSun = @AttendedStartDate

--	SELECT @AttendedStartDate = CONVERT(DATETIME, '01/01/' + CAST(DATEPART(yyyy, @AttendedStartDate) AS VARCHAR(4)) + ' 10:45:00', 101)
--	SELECT @AttendedStartDateSun = @AttendedStartDate

	SELECT @CampusCheckIn = 8
END

/*
IF @Campus = 69	--HNT
BEGIN	
	SELECT @AttendedStartDate = CONVERT(DATETIME, '01/01/' + CAST(DATEPART(yyyy, @AttendedStartDate) AS VARCHAR(4)) + ' 09:00:00', 101)
	SELECT @AttendedStartDateSun = @AttendedStartDate

	SELECT @AttendedStartDate = CONVERT(DATETIME, '01/01/' + CAST(DATEPART(yyyy, @AttendedStartDate) AS VARCHAR(4)) + ' 11:00:00', 101)
	SELECT @AttendedStartDateSun = @AttendedStartDate

	SELECT @CampusCheckIn = 64
END


IF @Campus = 70	--SLC
BEGIN	
	SELECT @AttendedStartDate = CONVERT(DATETIME, '01/01/' + CAST(DATEPART(yyyy, @AttendedStartDate) AS VARCHAR(4)) + ' 08:30:00', 101)
	SELECT @AttendedStartDateSun = @AttendedStartDate

	SELECT @AttendedStartDate = CONVERT(DATETIME, '01/01/' + CAST(DATEPART(yyyy, @AttendedStartDate) AS VARCHAR(4)) + ' 10:00:00', 101)
	SELECT @AttendedStartDateSun = @AttendedStartDate

	SELECT @CampusCheckIn = 
END


IF @Campus = 63	--CDL
BEGIN	
	SELECT @AttendedStartDate = CONVERT(DATETIME, '01/01/' + CAST(DATEPART(yyyy, @AttendedStartDate) AS VARCHAR(4)) + ' 09:00:00', 101)
	SELECT @AttendedStartDateSun = @AttendedStartDate

	SELECT @AttendedStartDate = CONVERT(DATETIME, '01/01/' + CAST(DATEPART(yyyy, @AttendedStartDate) AS VARCHAR(4)) + ' 11:15:00', 101)
	SELECT @AttendedStartDateSun = @AttendedStartDate

	SELECT @CampusCheckIn = 
END
*/



--Get the first Sunday of the year
WHILE (DATENAME(dw, @AttendedStartDate) <> 'Sunday' AND @AttendedStartDateSun IS NOT NULL)
BEGIN
	IF (DATENAME(dw, @AttendedStartDate) = 'Sunday')
	BEGIN
		SELECT @AttendedStartDateSun = @AttendedStartDate
		BREAK
	END
	SELECT @AttendedStartDate = DATEADD(day, 1, @AttendedStartDate)
END

--Getthe first Saturday of the year
WHILE (DATENAME(dw, @AttendedStartDate) <> 'Saturday' AND @AttendedStartDateSat IS NOT NULL)
BEGIN
	IF (DATENAME(dw, @AttendedStartDate) = 'Saturday')
	BEGIN		
		SELECT @AttendedStartDateSat = @AttendedStartDate
		BREAK
	END
	SELECT @AttendedStartDate = DATEADD(day, 1, @AttendedStartDate)
END


--Build the table with the weekly dates
DECLARE @DatesAttended TABLE (DateAttended DATETIME, Cnt INT, Attended DATETIME, ServiceDay VARCHAR(6), ServiceTime VARCHAR(4))

SELECT @AttendedStartDateHold = @AttendedStartDate

WHILE DATEPART(yyyy, @AttendedStartDateHold) = DATEPART(yyyy, @AttendedStartDate)
BEGIN
	INSERT INTO @DatesAttended VALUES (@AttendedStartDateHold, NULL, NULL, NULL, NULL)

	SELECT @AttendedStartDateHold = DATEADD(day, 7, @AttendedStartDateHold)
END

--Insert the attendance count into the table of dates
DECLARE @Attendance TABLE (Cnt INT, Attended DATETIME, ServiceDay VARCHAR(6), ServiceTime VARCHAR(4))
INSERT INTO @Attendance
SELECT --prom.Entityid AS PersonId,
		COUNT(prom.EntityId) AS Cnt,
		prom.Attended,
		SUBSTRING(DATENAME(mm, prom.Attended), 1, 3) + ' ' + DATENAME(d, prom.Attended) AS ServiceDay,
		--REPLACE(FORMAT(prom.Attended,'hh'), '0', '') + LOWER(FORMAT(prom.Attended,'tt')) AS ServiceTime
		SUBSTRING(FORMAT(prom.Attended,'hh'), PATINDEX('%[^0 ]%', FORMAT(prom.Attended,'hh') + ' '), LEN(FORMAT(prom.Attended,'hh'))) + LOWER(FORMAT(prom.Attended,'tt')) AS ServiceTime
--INTO #TempAttended
FROM #TempPL tpl
INNER JOIN CheckIn.dbo.Prom_Attendance prom
	ON tpl.id = prom.AttendedNodeNum
	AND prom.Attended BETWEEN @AttendedStartDate AND DATEADD(day, 1, @AttendedEndDate)
	AND prom.EntityId <> 0
	AND prom.Campus = @CampusCheckIn
INNER JOIN @DatesAttended da
	ON prom.Attended = da.DateAttended
--WHERE tpl.ParentGroupId IN (25348, 25349, 25350)
GROUP BY prom.Attended--, tpl.Name, prom.AttendedNodeNum
ORDER BY --REPLACE(FORMAT(prom.Attended,'hh'), '0 ', '') + LOWER(FORMAT(prom.Attended,'tt')),
	  SUBSTRING(FORMAT(prom.Attended,'hh'), PATINDEX('%[^0 ]%', FORMAT(prom.Attended,'hh') + ' '), LEN(FORMAT(prom.Attended,'hh'))) + LOWER(FORMAT(prom.Attended,'tt')),
		prom.Attended--, tpl.Name, prom.AttendedNodeNum


--Update the table of dates with the cattendance counts and info
UPDATE @DatesAttended
SET Cnt = ta.Cnt,
	Attended = ta.Attended,
	ServiceDay = ta.ServiceDay,
	ServiceTime = ta.ServiceTime
FROM @DatesAttended da
INNER JOIN @Attendance ta
	ON da.DateAttended = ta.Attended

--Query the table for use in the report
SELECT DateAttended AS DateAttended,
	ISNULL(Cnt, 0) AS Cnt,
	ISNULL(Attended, '') AS Attended,
	ISNULL(ServiceDay, '') AS ServiceDay,
	ISNULL(ServiceTime, '') AS ServiceTime
FROM @DatesAttended
WHERE DATEPART(hour, DateAttended) = CASE WHEN DATEPART(hour, @AttendedStartDateSat) IS NULL THEN DATEPART(hour, @AttendedStartDateSun) ELSE DATEPART(hour, @AttendedStartDateSat) END
ORDER BY DateAttended

END

GO
