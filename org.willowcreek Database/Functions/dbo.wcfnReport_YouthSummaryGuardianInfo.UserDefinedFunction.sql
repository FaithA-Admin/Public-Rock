/****** Object:  UserDefinedFunction [dbo].[wcfnReport_YouthSummaryGuardianInfo]    Script Date: 6/13/2016 11:25:28 AM ******/
DROP FUNCTION [dbo].[wcfnReport_YouthSummaryGuardianInfo]
GO
/****** Object:  UserDefinedFunction [dbo].[wcfnReport_YouthSummaryGuardianInfo]    Script Date: 6/13/2016 11:25:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[wcfnReport_YouthSummaryGuardianInfo]
(
	@PersonId INT
)
RETURNS @GuardianInfoTable TABLE (LineCnt INT, GuardianInfo VARCHAR(MAX))

BEGIN
	DECLARE @iTotalRowCnt INT
	DECLARE @iLineCnt INT
	DECLARE @RetValue NVARCHAR(500)

	DECLARE @Temp TABLE (LastName VARCHAR(50), FirstName VARCHAR(50), EMail VARCHAR(75), MobilPhone VARCHAR(20), HomePhone VARCHAR(20))

	INSERT INTO @Temp (LastName, FirstName, Email, MobilPhone, HomePhone)
	SELECT p.LastName, p.FirstName, 
		ISNULL(p.Email, '') AS EMail,
		ISNULL((SELECT TOP 1 pn.NumberFormatted FROM PhoneNumber pn WHERE p.id = pn.PersonId AND NumberTypeValueId IN (12) ORDER BY NumberTypeValueId), '') AS MobilPhone,
		ISNULL((SELECT TOP 1 pn.NumberFormatted FROM PhoneNumber pn WHERE p.id = pn.PersonId AND NumberTypeValueId IN (13) ORDER BY NumberTypeValueId), '') AS HomePhone
	FROM GroupMember gm
	INNER JOIN [Group] g
		ON gm.GroupId = g.id
		AND g.GroupTypeId = 10	--Family household
	INNER JOIN GroupType gt
		ON g.GroupTypeId = gt.id
	LEFT OUTER JOIN GroupMember gmGuardian
		ON gm.GroupId = gmGuardian.GroupId
		--AND gmGuardian.PersonId <> @PersonId
		AND gmGuardian.GroupRoleId = 3
	INNER JOIN PersonAlias pa
		ON gmGuardian.PersonId = pa.AliasPersonId
	INNER JOIN Person p
		ON pa.PersonId = p.Id
		AND (p.BirthDate IS NULL OR
		 YEAR(CURRENT_TIMESTAMP) - YEAR(p.BirthDate) -
      		(CASE WHEN (MONTH(p.BirthDate) > MONTH	(CURRENT_TIMESTAMP))
				   OR (MONTH(p.BirthDate) = MONTH(CURRENT_TIMESTAMP)
				  AND DAY(p.BirthDate) > DAY(CURRENT_TIMESTAMP))
				 THEN 1 ELSE 0 END) >= 18)
	WHERE gm.PersonId IN (@PersonId)
	ORDER BY p.Birthdate

	SELECT @iTotalRowCnt = COUNT(*) FROM @Temp
	SELECT @iLineCnt = 0
	SELECT @RetValue = ''

	WHILE @iTotalRowCnt > 0
	BEGIN
		SELECT @iLineCnt = @iLineCnt + 
				CASE ISNULL(LastName, '') WHEN '' THEN 0 ELSE 1 END +
				CASE ISNULL(Email, '') WHEN '' THEN 0 ELSE 1 END +
				CASE WHEN ISNULL(MobilPhone, '') = '' AND ISNULL(HomePhone, '') = '' THEN 0 ELSE 1 END,

				@RetValue = @RetValue + 
							LastName + ', ' + FirstName +
							CASE ISNULL(Email, '') 
								WHEN '' THEN CASE ISNULL(MobilPhone, '')
												WHEN '' THEN CASE ISNULL(HomePhone, '') WHEN '' THEN '' ELSE '!' + HomePhone + ' (Home)' END
												ELSE '!' + MobilPhone + ' (Cell)'  
											 END 
								ELSE  '!' + EMail + 
									CASE ISNULL(MobilPhone, '')
										WHEN '' THEN CASE ISNULL(HomePhone, '') WHEN '' THEN '' ELSE '!' + HomePhone + ' (Home)' END
										ELSE '!' + MobilPhone + ' (Cell)' 
									END 
							END + '!!' 
		FROM @Temp

		BREAK
	END
/*
	WHILE @iTotalRowCnt > 0
	BEGIN
		SELECT @RetValue = @RetValue + 
							LastName + ', ' + FirstName + '!' + 
							CASE ISNULL(Email, '') 
								WHEN CASE ISNULL(MobilPhone, '')
										WHEN '' THEN HomePhone
										ELSE MobilPhone  
									 END THEN HomePhone 
								ELSE EMail + 
									CASE ISNULL(MobilPhone, '')
										WHEN '' THEN HomePhone
										ELSE MobilPhone  
									END 
							END + '!!' 
		FROM @Temp

		BREAK
	END
*/

	INSERT INTO @GuardianInfoTable VALUES (@iLineCnt + @iTotalRowCnt - 1, SUBSTRING(@RetValue, 1, LEN(@RetValue) - 2))

	RETURN

END


--  EXEC dbo.wcufn_YouthSummaryGuardianInfo 66781

GO
