/****** Object:  UserDefinedFunction [dbo].[wcfnReport_Badge]    Script Date: 6/13/2016 11:25:28 AM ******/
DROP FUNCTION [dbo].[wcfnReport_Badge]
GO
/****** Object:  UserDefinedFunction [dbo].[wcfnReport_Badge]    Script Date: 6/13/2016 11:25:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
CREATE FUNCTION [dbo].[wcfnReport_Badge](@BadgeType VARCHAR(1), @PID INT, @RetType VARCHAR(5)) 
 RETURNS VARCHAR(100) 
 AS 
 BEGIN 
	DECLARE @RetVal VARCHAR(200)
	DECLARE @MemberRestriction VARCHAR(200)

	IF (@BadgeType = 'P')	--Protection
	BEGIN
		SELECT --@RetVal = dvProt.Value
			@RetVal =  CASE @RetType
							WHEN 'Text' THEN dvProt.Value
							ELSE CASE dvProt.Value	--'Color
									WHEN 'Approved' THEN 'ForestGreen'
									WHEN 'Approved with restrictions' THEN 'Yellow'
									WHEN 'Declined' THEN 'Red'
									WHEN 'Expired' THEN 'Grey'
									WHEN 'In Progress' THEN 'Purple'
									WHEN 'Needs Review' THEN 'Purple'
									ELSE 'Black'
								END
						END
		FROM Person p
		LEFT OUTER JOIN AttributeValue avProt
			ON p.Id = avProt.EntityId
			AND avProt.Value IS NOT NULL
			AND avProt.Value <> ''
			AND avProt.AttributeId = 1731	 
		LEFT OUTER JOIN DefinedValue dvProt
			ON dvProt.Guid = CONVERT(UNIQUEIDENTIFIER, avProt.Value)
			AND dvProt.Value IS NOT NULL
			AND dvProt.Value <> ''
		WHERE p.id IN (@PID)
	END

	IF (@BadgeType = 'C')	--Campus Activity
	BEGIN
		SELECT @MemberRestriction = ISNULL(avMemberRestricted.Value, '')
		FROM Person p
		LEFT OUTER JOIN AttributeValue avMemberRestricted
			ON avMemberRestricted.AttributeId = 1720	--Campuses
			AND p.Id = avMemberRestricted.EntityId		
		WHERE p.id in (@PID)

		SELECT @MemberRestriction = @MemberRestriction + ISNULL(avMemberRestricted.Value, '')
		FROM Person p
		LEFT OUTER JOIN AttributeValue avMemberRestricted
			ON avMemberRestricted.AttributeId = 1721	--Service
			AND p.Id = avMemberRestricted.EntityId		
		WHERE p.id in (@PID)

		SELECT @MemberRestriction = @MemberRestriction + ISNULL(avMemberRestricted.Value, '')
		FROM Person p
		LEFT OUTER JOIN AttributeValue avMemberRestricted
			ON avMemberRestricted.AttributeId = 1722	--Sections
			AND p.Id = avMemberRestricted.EntityId		
		WHERE p.id in (@PID)

		SELECT @MemberRestriction = @MemberRestriction + ISNULL(avMemberRestricted.Value, '')
		FROM Person p
		LEFT OUTER JOIN AttributeValue avMemberRestricted
			ON avMemberRestricted.AttributeId = 1724	--Groups
			AND p.Id = avMemberRestricted.EntityId		
		WHERE p.id in (@PID)

		SELECT @MemberRestriction = @MemberRestriction + ISNULL(avMemberRestricted.Value, '')
		FROM Person p
		LEFT OUTER JOIN AttributeValue avMemberRestricted
			ON avMemberRestricted.AttributeId = 1725	--Care Center
			AND p.Id = avMemberRestricted.EntityId		
		WHERE p.id in (@PID)

		SELECT @MemberRestriction = @MemberRestriction + ISNULL(avMemberRestricted.Value, '')
		FROM Person p
		LEFT OUTER JOIN AttributeValue avMemberRestricted
			ON avMemberRestricted.AttributeId = 1726	--Other
			AND p.Id = avMemberRestricted.EntityId		
		WHERE p.id in (@PID)

		SELECT --@RetVal = avCampus.Value
			@RetVal = CASE @RetType
						WHEN 'Text' THEN CASE WHEN avCampus.Value <> '' THEN avCampus.Value WHEN avCampus.Value = '' AND @MemberRestriction <> '' THEN @MemberRestriction ELSE '' END
						ELSE CASE		--avCampus.Value	--'Color
								WHEN avCampus.Value = 'All on and off campus events' THEN 'Red'
								WHEN avCampus.Value = 'All on and off campus events except weekend services' OR @MemberRestriction <> '' THEN 'Yellow'
								ELSE ''
							END
					END
		FROM Person p
		LEFT OUTER JOIN AttributeValue avCampus
			ON avCampus.AttributeId = 1720
			AND p.Id = avCampus.EntityId		
		WHERE p.id in (@PID)
	END

	IF (@BadgeType = 'M')	--Membership
	BEGIN
		SELECT @MemberRestriction = ISNULL(avMemberRestricted.Value, '')
		FROM Person p
		LEFT OUTER JOIN AttributeValue avMemberRestricted
			ON avMemberRestricted.AttributeId = 2065
			AND p.Id = avMemberRestricted.EntityId
		WHERE p.id in (@PID)

		If @MemberRestriction = ''
		BEGIN
			SELECT --@RetVal = avCampus.Value
				@RetVal = CASE @RetType
							WHEN 'Text' THEN dvMember.Value
							ELSE CASE dvMember.Value	--'Color
									WHEN 'In Progress' THEN 'Purple'
									WHEN 'Ineligible' THEN 'Yellow'
									WHEN 'Member' THEN 'ForestGreen'
									WHEN 'Pending Baptism' THEN 'Purple'
									WHEN 'Pending under 18' THEN 'Purple'
									WHEN 'Previously a member' THEN 'Gray'
									ELSE 'Black'
								END
						END
			FROM Person p
			INNER JOIN AttributeValue avMember
				ON p.Id = avMember.EntityId
				AND avMember.Value IS NOT NULL
				AND avMember.Value <> ''
				AND avMember.AttributeId = 1672
			INNER JOIN DefinedValue dvMember
				ON dvMember.Guid = CONVERT(UNIQUEIDENTIFIER, avMember.Value)
				AND dvMember.Value IS NOT NULL
				AND dvMember.Value <> ''
				AND dvMember.DefinedTypeId =  53		
			WHERE p.id in (@PID)
		END
		ELSE
		BEGIN
			IF @RetType = 'Text'
				SELECT @RetVal = @MemberRestriction
			ELSE
				SELECT @RetVal = 'Red'
		END
	END

	IF (@BadgeType = 'L')	--Leadership
	BEGIN
		SELECT --@RetVal = avCampus.Value
			@RetVal = CASE @RetType
						WHEN 'Text' THEN dvLeadership.Value
						ELSE CASE dvLeadership.Value	--'Color
								WHEN 'Approved' THEN 'ForestGreen'
								WHEN 'Approved with restrictions' THEN 'Yellow'
								WHEN 'Expired' THEN 'Gray'
								WHEN 'In Progress' THEN 'Purple'
								WHEN 'Ineligible' THEN 'Red'
								ELSE 'Black'
							END
					END
		FROM Person p
		INNER JOIN AttributeValue avLeadership
			ON p.Id = avLeadership.EntityId
			AND avLeadership.Value IS NOT NULL
			AND avLeadership.Value <> ''
			AND avLeadership.AttributeId = 1728
		INNER JOIN DefinedValue dvLeadership
			ON dvLeadership.Guid = CONVERT(UNIQUEIDENTIFIER, avLeadership.Value)
			AND dvLeadership.Value IS NOT NULL
			AND dvLeadership.Value <> ''
			AND dvLeadership.DefinedTypeId =  61		
		WHERE p.id in (@PID)
	END



	 RETURN @RetVal 
 END; 


 
 

 

/*
 DECLARE @Ret VARCHAR(100)
exec @Ret = [dbo].[wcufn_ProtectionBadge] @PID = 51821

SELECT @Ret


SELECT [dbo].[wcufn_ProtectionBadge] (51821, 'Color') AS xxx





CASE dvProt.Value
		WHEN 'Approved' THEN 'ForestGreen'
		WHEN 'Approved with restrictions' THEN 'Yellow'
		WHEN 'Declined' THEN 'Red'
		WHEN 'Expired' THEN 'Grey'
		WHEN 'In Progress' THEN 'Purple'
		WHEN 'Needs Review' THEN 'Purple'
		ELSE 'Black'
	END AS ProtectionLevel,
	
	
SELECT avCapmpus.Value, p.id
 FROM Person p
LEFT OUTER JOIN AttributeValue avCapmpus
	ON avCapmpus.AttributeId = 1720
	AND p.Id = avCapmpus.EntityId		
 WHERE p.id in (@PID)



 SELECT p.id, dv.Value
 FROM Person p
INNER JOIN AttributeValue av
	ON p.Id = av.EntityId
	AND av.Value IS NOT NULL
	AND av.Value <> ''
	AND av.AttributeId = 1672
INNER JOIN DefinedValue dv
	ON dv.Guid = CONVERT(UNIQUEIDENTIFIER, av.Value)
	AND dv.Value IS NOT NULL
	AND dv.Value <> ''
	AND dv.DefinedTypeId =  53



--Leadership
 SELECT p.id, dv.Value
 FROM Person p
INNER JOIN AttributeValue av
	ON p.Id = av.EntityId
	AND av.Value IS NOT NULL
	AND av.Value <> ''
	AND av.AttributeId = 1728
INNER JOIN DefinedValue dv
	ON dv.Guid = CONVERT(UNIQUEIDENTIFIER, av.Value)
	AND dv.Value IS NOT NULL
	AND dv.Value <> ''
	AND dv.DefinedTypeId =  61
 WHERE p.id in (3)

 SELECT avCapmpus.Value, p.id
 FROM Person p
LEFT OUTER JOIN AttributeValue avCapmpus
	ON avCapmpus.AttributeId = 2065
	AND p.Id = avCapmpus.EntityId

	*/
GO
