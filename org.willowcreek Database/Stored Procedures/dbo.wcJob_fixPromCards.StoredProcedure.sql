/****** Object:  StoredProcedure [dbo].[wcJob_fixPromCards]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcJob_fixPromCards]
GO
/****** Object:  StoredProcedure [dbo].[wcJob_fixPromCards]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[wcJob_fixPromCards]
AS
BEGIN

	DECLARE @now DATETIME = GETDATE()

	--POST MERGE FIX: PEOPLE MERGED ONTO A NEW FAMILY
	--select where householdid is no longer valid
	--find family with most kids
	--update householdid in prom cards to match

	DECLARE @households TABLE ( HouseHoldId INT, PHouseHoldId VARCHAR(4), RockAliasPersonId INT, PersonId INT, NewFamilyId INT, processed INT )
	INSERT INTO @households
	SELECT HouseHoldId, PHouseHoldId, RockAliasPersonId, PersonId, 
		   ( SELECT TOP 1 gm.GroupId
			   FROM GroupMember gm
			   JOIN [Group] g ON gm.GroupId = g.Id AND g.GroupTypeId = 10
			  WHERE gm.PersonId = pa.PersonId
			  ORDER BY (SELECT COUNT(*) 
						  FROM groupmember
						 WHERE groupid = g.id 
						   AND grouproleid = 4) DESC
		   ) NewFamilyId, 0
	FROM checkin.dbo.prom_cards kpc
	JOIN PersonAlias pa on kpc.RockAliasPersonId = pa.AliasPersonId
	WHERE kpc.active = 1
	AND kpc.RockAliasPersonId IS NOT NULL
	AND NOT EXISTS (SELECT gm.GroupId
					  FROM GroupMember gm
					  JOIN [Group] g ON gm.GroupId = g.Id AND g.GroupTypeId = 10
					 WHERE gm.PersonId = pa.PersonId
					  AND gm.GroupId = kpc.HouseHoldId)
	ORDER BY PHouseHoldId

	--SELECT * FROM @households

	UPDATE kpc
	SET kpc.HouseHoldId = h.NewFamilyId, kpc.Lupd_DateTime = @now, kpc.Lupd_User = 'rockjob'
	FROM checkin.dbo.Prom_Cards kpc
	JOIN @households h ON kpc.HouseHoldId = h.HouseHoldId
	                  AND kpc.PHouseHoldId = h.PHouseHoldId
					  AND kpc.RockAliasPersonId = h.RockAliasPersonId
	WHERE kpc.Active = 1

	
	
	--FIX: PEOPLE IN TWO FAMILES WITH WRONG HOUSEHOLD ID IN PROM CARDS
	
	--get list of card that are assigned to more than one family
	DECLARE @cards TABLE ( lastCampus INT, lastAttended DATETIME, cardId VARCHAR(4), familyId INT, personId INT, lastName NVARCHAR(50), firstName NVARCHAR(50), kids INT, link NVARCHAR(200), aliasId INT, processed INT )
	INSERT INTO @cards
	select 
			(select top 1 campus from checkin.dbo.Prom_Attendance where PHouseHoldId = pc.phouseholdid order by attended desc) LastCampus,
			(select top 1 attended from checkin.dbo.Prom_Attendance where PHouseHoldId = pc.phouseholdid order by attended desc) LastAttended,
			pc.phouseholdid CardId, g.Id FamilyId, p.id PersonId, p.LastName, p.FirstName,
		   (select count(*) from groupmember where groupid = g.id and grouproleid = 4) Kids,
		   '=HYPERLINK("https://rock.willowcreek.org/person/' + CAST(p.id AS VARCHAR) + '","' + CAST(p.id AS VARCHAR) + '")' Link, 
		   pc.RockAliasPersonId, 0
	   
	from (

		select pc.phouseholdid
		from checkin.dbo.prom_cards pc
		join personalias pa on pc.RockAliasPersonId = pa.aliaspersonid
		join groupmember gm on pa.PersonId = gm.PersonId
		join [group] g on gm.groupid = g.id and grouptypeid = 10
		where pc.RockAliasPersonId is not null
		and pc.active = 1
		group by pc.phouseholdid
		having count(distinct gm.groupid) > 1

	) x
	join checkin.dbo.prom_cards pc on x.phouseholdid = pc.phouseholdid and pc.active = 1
	join personalias pa on pc.RockAliasPersonId = pa.aliaspersonid
	join person p on pa.personid = p.id
	join groupmember gm on p.id = gm.PersonId
	join [group] g on gm.groupid = g.id and grouptypeid = 10
	order by LastCampus, LastAttended desc, pc.phouseholdid, kids desc, p.lastname

	--SELECT * FROM @cards ORDER BY cardId, kids desc
	
	--fix household id or inactive card
	DECLARE @aliasId INT = NULL
	DECLARE @personId INT = NULL
	DECLARE @familyId INT = NULL
	DECLARE @cardId VARCHAR(4) = NULL
	DECLARE @prevAliasId INT = NULL
	DECLARE @prevPersonId INT = NULL
	DECLARE @prevFamilyId INT = NULL
	DECLARE @prevCardId VARCHAR(4) = NULL
	WHILE (SELECT Count(*) FROM @cards WHERE processed = 0) > 0
	BEGIN
		SELECT TOP 1 @personId = personId, @familyId = familyId, @cardId = cardId, @aliasId = aliasId FROM @cards WHERE processed = 0 ORDER BY cardId, kids desc
		--if same card id, different family id
		IF @cardId = @prevCardId AND @familyId <> @prevFamilyId AND @prevCardId IS NOT NULL AND @prevFamilyId IS NOT NULL
		BEGIN
			--if i'm in previous family
			DECLARE @c INT = (SELECT COUNT(*) FROM GroupMember gm WHERE gm.PersonId = @personId AND gm.GroupId = @prevFamilyId)
			IF @c > 0
				UPDATE checkin.dbo.Prom_Cards SET HouseHoldId = @prevFamilyId, lupd_datetime = @now, Lupd_User = 'rockjob'  WHERE PHouseHoldId = @cardId AND RockAliasPersonId = @aliasId AND Active = 1
			--ELSE
			--	UPDATE checkin.dbo.Prom_Cards SET active = 0, ended = @now, lupd_datetime = @now, Lupd_User = 'rockjob' WHERE PHouseHoldId = @cardId AND RockAliasPersonId = @aliasId AND Active = 1
				
		END
		UPDATE @cards set processed = 1 WHERE personId = @personId AND familyId = @familyId AND cardId = @cardId
		SET @prevAliasId = @aliasId
		SET @prevPersonId = @personId
		SET @prevFamilyId = @familyId
		SET @prevCardId = @cardId
	END

	--FIX: IF NO MORE CARD, REMOVE ATTR
	UPDATE av
	SET av.Value = NULL					--removes kids OOPS!
	FROM AttributeValue av
	WHERE av.AttributeId = 2234
	AND NOT EXISTS ( SELECT NULL
					   FROM PersonAlias pa
					   JOIN checkin.dbo.Prom_Cards kpc ON pa.AliasPersonId = kpc.RockAliasPersonId AND kpc.PHouseHoldId = av.Value AND kpc.Active = 1
					  WHERE pa.PersonId = av.EntityId )

	--FIX: POPULATE ATTR FROM PROM CARD
	DECLARE @cardAssignment TABLE ( personId INT, cardId VARCHAR(4), processed INT )
	INSERT INTO @cardAssignment
	SELECT pa.PersonId, kpc.PHouseHoldId, 0
	FROM checkin.dbo.Prom_Cards kpc
	JOIN PersonAlias pa ON kpc.RockAliasPersonId = pa.AliasPersonId
	--JOIN GroupMember gm ON pa.PersonId = gm.PersonId AND	--TODO: add family members
	WHERE kpc.Active = 1

	--DECLARE @personId INT
	--DECLARE @cardId INT
	SET @personId = NULL
	SET @cardId = NULL
	WHILE (SELECT Count(*) FROM @cardAssignment WHERE processed = 0) > 0
	BEGIN
		SELECT TOP 1 @personId = personId, @cardId = cardId FROM @cardAssignment WHERE processed = 0
		EXEC wcUtil_saveAttributeValue 'FamilyIdCard', 'Person', @personId, @cardId
		UPDATE @cardAssignment set processed = 1 WHERE personId = @personId
	END

END
GO
