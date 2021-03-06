/****** Object:  StoredProcedure [dbo].[wcJob_saveFixes]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcJob_saveFixes]
GO
/****** Object:  StoredProcedure [dbo].[wcJob_saveFixes]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[wcJob_saveFixes]
AS
BEGIN

	-- Set NickName equal to FirstName if NickName is empty

       UPDATE person
       SET NickName = FirstName
       WHERE NickName = ''



	--populate FIRST VISIT DATE for Badge

	BEGIN TRANSACTION

	DECLARE @person TABLE (id INT, firstVisit DATE, createdDate DATE, processed INT)

	INSERT INTO @person
	SELECT p.Id, CAST(av.ValueAsDateTime AS DATE) FirstVisit, CAST(p.CreatedDateTime AS DATE) CreatedDate, 0
	FROM Person p
	LEFT JOIN AttributeValue av ON p.Id = av.EntityId AND av.AttributeId =  717 --FirstVisit
	WHERE p.CreatedDateTime IS NOT NULL
	AND (av.Value IS NULL)
	    --TR 5/5/16 removed next line. It was resetting manually entered dates to the createddatetime of the person record,
		-- destroying any manually changed entries that were earlier than the createddatetime (reported by Matt Sundstedt
		-- who was trying to change his first visit date but said it was getting reset every night).
		--OR CAST(av.ValueAsDateTime AS DATE) < CAST(p.CreatedDateTime AS DATE))

	DECLARE @id INT
	DECLARE @createdDate DATE
	WHILE (SELECT Count(*) FROM @person WHERE processed = 0) > 0
	BEGIN
		SELECT TOP 1 
			@id = id,
			
			@createdDate = createdDate
		FROM @person 
		WHERE processed = 0

		--switch id for guid?
		EXEC wcUtil_saveAttributeValue 'FirstVisit', 'Person', @id, @createdDate
		UPDATE @person set processed = 1 WHERE id = @id 
	END

	COMMIT


	--populate Rock ATTENDANCE from kairos/check-in

	--TRUNCATE TABLE rock.dbo.attendance
	--9/22/15 TR: changed TRUNCATE to selective delete, in order to preserve attendance for non check-in group types
	Delete a 
	FROM dbo.attendance a
	JOIN [group] g ON a.GroupId = g.Id
	WHERE g.GroupTypeId = 29 --(Self Check-In Small Group)

	--Updated column name and set to Guid instead of id
	INSERT INTO dbo.attendance
	(groupid,startdatetime,[guid],createddatetime,modifieddatetime,createdbypersonaliasid,modifiedbypersonaliasid,CampusId,PersonAliasId)
	select sg.Id, a.Attended, NEWID(), a.crtd_datetime, a.lupd_datetime, 1, 1, sg.CampusId, pa.Id
	from checkin.dbo.Prom_Attendance a
	join dbo.PersonAlias pa on a.entityid = pa.AliasPersonId
	join dbo.Person p on pa.PersonId = p.Id
	join checkin.dbo.wsrock_smallgroup sg on a.attendednodenum = sg.id
	where a.entityid > 0
	and a.Attended > DATEADD(MONTH,-25,GETDATE())


	--update CONNECTION STATUS based on membership block info

	UPDATE p
	SET ConnectionStatusValueId = 65
	--SELECT p.Id, p.ConnectionStatusValueId, av2.ValueAsDateTime membership, av3.value [status], 0
	FROM Person p
	JOIN AttributeValue av2 ON p.Id = av2.EntityId AND av2.AttributeId =  906 --MembershipDate
	JOIN AttributeValue av3 ON p.Id = av3.EntityId AND av3.AttributeId =  1672 --MembershipStatus
	WHERE av2.Value > ''
	AND av3.Value = '7B5751CC-38E2-41F6-85A7-BBA0FC5D51BA'	--member
	AND p.ConnectionStatusValueId <> 65


	--fixes for 'server error 500' when users try to submit protection reference or questionnaire
	--	log an exception with count of Reference table records to be affected
	--TH switched column names
	--DECLARE @fixRefCount INT
	--SELECT @fixRefCount = COUNT(r.ReferencePersonAliasGuid)
	--FROM dbo._org_willowcreek_ProtectionApp_Reference r
	--JOIN dbo.PersonAlias pa ON r.ReferencePersonAliasGuid = pa.[Guid]
	--WHERE pa.AliasPersonId <> pa.PersonId

	--IF @fixRefCount > 0
	--BEGIN	
	--	INSERT INTO [dbo].[ExceptionLog] ([ParentId],[SiteId],[PageId],[HasInnerException],[StatusCode],[ExceptionType],[Description],[Source],[StackTrace],[PageUrl],[ServerVariables],[QueryString],[Form],[Cookies],[Guid],[CreatedDateTime],[ModifiedDateTime],[CreatedByPersonAliasId],[ModifiedByPersonAliasId],[ForeignId])
	--	VALUES (NULL,NULL,NULL,0,NULL,'System.Exception',CONCAT('Updating ', @fixRefCount, ' record(s) in ProtectionApp_Reference table, setting ReferencedPersonAliasGuid for merged Person records to current PersonId from PersonAlias table.'),'Stored Procedure: wcJob_saveFixes',NULL,NULL,NULL,NULL,NULL,NULL,NEWID(),SYSDATETIME(),SYSDATETIME(),NULL,NULL,NULL)

	--	--	update Rock Reference RereferencedPersonID with new Person.Id that resulted from Person merges
	--	--TH Column names changed and already updated reference table to have GUIDS
	--	--UPDATE r
	--	--SET r.ReferencedPersonAliasGuid = pa.PersonId
	--	--FROM rock.dbo._org_willowcreek_ProtectionApp_Reference r
	--	--JOIN rock.dbo.PersonAlias pa ON r.ReferencedPersonAliasGuid = pa.AliasPersonId
	--	--WHERE pa.AliasPersonId <> pa.PersonId
	--END

	----	log an exception with count of Questionnaire table records to be affected
	--DECLARE @fixProtCount INT
	--SELECT @fixProtCount = COUNT(q.[ApplicantPersonAliasGuid])
	--FROM [dbo].[_org_willowcreek_ProtectionApp_Questionnaire] q
	--join dbo.PersonAlias pa on q.[ApplicantPersonAliasGuid] = pa.[Guid]
	--where pa.AliasPersonId <> pa.PersonId

	----	update Rock Protection Questionnaire ApplicantPersonId with new Person.Id that resulted from Person merges
	--IF @fixProtCount > 0
	--BEGIN
	--	INSERT INTO [dbo].[ExceptionLog] ([ParentId],[SiteId],[PageId],[HasInnerException],[StatusCode],[ExceptionType],[Description],[Source],[StackTrace],[PageUrl],[ServerVariables],[QueryString],[Form],[Cookies],[Guid],[CreatedDateTime],[ModifiedDateTime],[CreatedByPersonAliasId],[ModifiedByPersonAliasId],[ForeignId])
	--	VALUES (NULL,NULL,NULL,0,NULL,'System.Exception',CONCAT('Updating ', @fixProtCount, ' record(s) in ProtectionApp_Questionnaire table, setting ApplicantPersonId for merged Person records to current PersonId from PersonAlias table.'),'Stored Procedure: wcJob_saveFixes',NULL,NULL,NULL,NULL,NULL,NULL,NEWID(),SYSDATETIME(),SYSDATETIME(),NULL,NULL,NULL)
	--	--TH Column names changed and already updated questionnaire table to have GUIDS
	--	--UPDATE q
	--	--SET q.ApplicantPersonAliasGuid = pa.[Guid]
	--	--FROM Rock.dbo._org_willowcreek_ProtectionApp_Questionnaire q
	--	--JOIN rock.dbo.PersonAlias pa ON q.ApplicantPersonAliasGuid = pa.AliasPersonId
	--	--WHERE pa.AliasPersonId <> pa.PersonId
	--END



	--FIX Prom Cards after merge
	EXEC wcJob_fixPromCards

	
	
/*
--TR 5/6/16 - disabled this fix.  It was referencing an old copy of the database (Rock_BeforeMerge).  This was 
-- useful after the initial auto-merge ran and removed logins in June of 2015.  But I tested it now and it's trying to add
-- user logins for people who have since left staff.  

	--FIX logins after merge
	INSERT INTO userlogin
	(UserName,[Password],IsConfirmed,LastActivityDateTime,LastLoginDateTime,LastPasswordChangedDateTime,
	 IsOnLine,IsLockedOut,LastLockedOutDateTime,FailedPasswordAttemptCount,FailedPasswordAttemptWindowStartDateTime,
	 LastPasswordExpirationWarningDateTime,ApiKey,PersonId,[Guid],EntityTypeId,CreatedDateTime,ModifiedDateTime,
	 CreatedByPersonAliasId,ModifiedByPersonAliasId,ForeignId)
	select old.UserName,old.[Password],old.IsConfirmed,old.LastActivityDateTime,old.LastLoginDateTime,old.LastPasswordChangedDateTime,
		   old.IsOnLine,old.IsLockedOut,old.LastLockedOutDateTime,old.FailedPasswordAttemptCount,old.FailedPasswordAttemptWindowStartDateTime,
		   old.LastPasswordExpirationWarningDateTime,old.ApiKey,pa.PersonId,old.[Guid],old.EntityTypeId,old.CreatedDateTime,old.ModifiedDateTime,
		   old.CreatedByPersonAliasId,old.ModifiedByPersonAliasId,old.ForeignId
	from Rock_BeforeMerge.dbo.userlogin old
	join PersonAlias pa on old.PersonId = pa.AliasPersonId
	left join userlogin new on old.username = new.username
	where new.id is null
	and old.[Guid] not in (select [guid]
							 from userlogin)
*/


	----FIX Protection Status
	--exec wcJob_fixProtectionStatus

	----Expire outdated workflows
	--exec wcJob_ExpireWorkflows

END


GO
