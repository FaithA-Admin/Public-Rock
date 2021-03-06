/****** Object:  UserDefinedFunction [dbo].[wcfnReport_YouthSummaryGuardianPhoneCount]    Script Date: 6/13/2016 11:25:28 AM ******/
DROP FUNCTION [dbo].[wcfnReport_YouthSummaryGuardianPhoneCount]
GO
/****** Object:  UserDefinedFunction [dbo].[wcfnReport_YouthSummaryGuardianPhoneCount]    Script Date: 6/13/2016 11:25:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

 
CREATE FUNCTION [dbo].[wcfnReport_YouthSummaryGuardianPhoneCount](@PID INT) 
 RETURNS INT 
 AS 
 BEGIN 
	DECLARE @PhoneCnt INT

	DECLARE @Temp TABLE (ChildId INT, GuardianId INT, MobilPhone VARCHAR(20), HomePhone VARCHAR(20))

	INSERT INTO @Temp (ChildId, GuardianId, MobilPhone, HomePhone)
	SELECT gmCnt.PersonId, pCnt.Id,
		ISNULL((SELECT TOP 1 pn.NumberFormatted FROM PhoneNumber pn WHERE pCnt.id = pn.PersonId AND NumberTypeValueId IN (12) ORDER BY NumberTypeValueId), '') AS MobilPhone,
		ISNULL((SELECT TOP 1 pn.NumberFormatted FROM PhoneNumber pn WHERE pCnt.id = pn.PersonId AND NumberTypeValueId IN (13) ORDER BY NumberTypeValueId), '') AS HomePhone
	FROM GroupMember gmCnt
	INNER JOIN [Group] gCnt
		ON gmCnt.GroupId = gCnt.id
		AND gCnt.GroupTypeId = 10	--Family household
	INNER JOIN GroupType gtCnt
		ON gCnt.GroupTypeId = gtCnt.id
	LEFT OUTER JOIN GroupMember gmCntGuardian
		ON gmCnt.GroupId = gmCntGuardian.GroupId
		AND gmCntGuardian.GroupRoleId = 3
	INNER JOIN PersonAlias paCnt
		ON gmCntGuardian.PersonId = paCnt.AliasPersonId
	INNER JOIN Person pCnt
		ON paCnt.PersonId = pCnt.Id
		AND (pCnt.BirthDate IS NULL OR
		 YEAR(CURRENT_TIMESTAMP) - YEAR(pCnt.BirthDate) -
      		(CASE WHEN (MONTH(pCnt.BirthDate) > MONTH	(CURRENT_TIMESTAMP))
				   OR (MONTH(pCnt.BirthDate) = MONTH(CURRENT_TIMESTAMP)
				  AND DAY(pCnt.BirthDate) > DAY(CURRENT_TIMESTAMP))
				 THEN 1 ELSE 0 END) >= 18)
	WHERE gmCnt.PersonId IN (@PID)

	SELECT @PhoneCnt = COUNT(ChildId) 
	FROM @Temp
	WHERE (MobilPhone IS NOT NULL 
	AND MobilPhone <> '')
	OR (HomePhone IS NOT NULL 
	AND HomePhone <> '')

	 RETURN @PhoneCnt 
 END; 


GO
