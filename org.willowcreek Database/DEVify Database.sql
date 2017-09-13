/*
If you get a runtime error when loading rock, check app_data/logs/RockExceptions.csv for details
If rock loads but has an error page, look for the most recent rows in the ExceptionLog table.
You may need to add the run.migrations file to the app_data folder and restart IIS, then refresh the page to get the database updated.
*/

-- STOP! Set the database first
USE SparkRock--RockTest/RockPlay/RockDemo/Rock6/SparkRock
GO

-- Do not use http or slashes in these urls
declare @sitename varchar(255) = case db_name() 
		when 'RockTest' then 'rocktest.willowcreek.org'
		when 'RockPlay' then 'rockplay.willowcreek.org'
		when 'RockDemo' then 'rockdemo.willowcreek.org'
		when 'Rock6' then 'rock6.willowcreek.org'
		when 'SparkRock' then 'sparkrock.willowcreek.org'
		end,

		@external varchar(255) = case db_name() 
		when 'RockTest' then 'portaltest.willowcreek.org'
		when 'RockPlay' then 'portalplay.willowcreek.org'
		when 'RockDemo' then 'portaldemo.willowcreek.org'
		when 'Rock6' then 'portal6.willowcreek.org'
		when 'SparkRock' then 'sparkportal.willowcreek.org'
		end,

		@carecentersite varchar(255) = case db_name()
		when 'RockTest' then 'rockcctest.willowcreek.org'
		when 'RockPlay' then 'rockccplay.willowcreek.org'
		when 'RockDemo' then 'rockccdemo.willowcreek.org'
		when 'Rock6' then 'rockcc6.willowcreek.org'
		when 'SparkRock' then 'sparkrockcc.willowcreek.org'
		end,

		@carecenterportal varchar(255) = case db_name()
		when 'RockTest' then 'rockcctest.willowcreek.org'	-- ******************************************************************
		when 'RockPlay' then 'rockccplay.willowcreek.org'	-- * don't have domain for these (just use main care center domain  *
		when 'RockDemo' then 'rockccdemo.willowcreek.org'	-- *    until domains are created for them).                        *
		when 'Rock6' then 'rockcc6.willowcreek.org'			-- ******************************************************************
		when 'SparkRock' then 'sparkrock-careportal.willowcreek.org'
		end,
		
		--@sitetitle varchar(255) = 'Test',
		@siteemail varchar(255) = 'noone@willowcreek.org',
		-- These use a test account so we're not doing real background checks in test
		@protectmyministryusername varchar(255) = 'JStone',
		@protectmyministrypassword varchar(255) = 'EAAAAGSNMnv6WhPogsYoiaOUdoqW/xpFOcI9I/BNZKDePbDP',
		@protectmyministrytestmode varchar(255) = 'True',
		-- Change the mail server to the internal one
		@emailserver varchar(255) = 'mailrelay.willowcreek.org',
		@emailport varchar(255) = '25',
		@emailusername varchar(255) = '',
		@emailpassword varchar(255) = '',
		@ProtectMyMinistryWebHook varchar(255) = 'https://portaltest.willowcreek.org/Webhooks/ProtectMyMinistry.ashx',
		@SignNowWebHook varchar(255) = 'https://portaltest.willowcreek.org/WebHooks/SignNow.ashx'

-- Add the Rock user needed for this environment

if db_name() = 'RockTest' and not Exists (SELECT * FROM sys.database_principals where name='RockTestUser')
begin
	create user RockTestUser for login RockTestUser
	EXEC sp_addrolemember N'db_owner', N'RockTestUser'
end

if db_name() = 'RockPlay' and not exists (SELECT * FROM sys.database_principals where name='RockPlayUser')
begin
	create user RockPlayUser for login RockPlayUser
	EXEC sp_addrolemember N'db_owner', N'RockPlayUser'
end

if db_name() = 'RockDemo' and not Exists (SELECT * FROM sys.database_principals where name='RockDemoUser')
begin
	create user RockDemoUser for login RockDemoUser
	EXEC sp_addrolemember N'db_owner', N'RockDemoUser'
end

if db_name() = 'Rock6' and not Exists (SELECT * FROM sys.database_principals where name='Rock6User')
begin
	create user Rock6User for login Rock6User
	EXEC sp_addrolemember N'db_owner', N'Rock6User'
end

if db_name() = 'SparkRock' and not Exists (SELECT * FROM sys.database_principals where name='SparkRockUser')
begin
	create user SparkRockUser for login SparkRockUser
	EXEC sp_addrolemember N'db_owner', N'SparkRockUser'
end

update person
set Email = Email + '__test'
FROM Person p
WHERE p.Email IS NOT NULL AND p.Email <> ''
		
update AttributeValue set value = 'https://' + @sitename + '/'
from attribute a
join AttributeValue v on v.AttributeId = a.Id
where a.[key] = 'internalapplicationroot'

update AttributeValue set value = 'https://' + @external + '/' 
from attribute a
join AttributeValue v on v.AttributeId = a.Id
where a.[key] = 'publicapplicationroot'

update SiteDomain set Domain = @sitename
WHERE Domain = 'rock.willowcreek.org'

update SiteDomain set Domain = @external
WHERE Domain = 'portal.willowcreek.org'

update SiteDomain set Domain = @carecentersite
where Domain = 'rockcc.willowcreek.org'

update SiteDomain set Domain = @carecenterportal
where Domain = 'portal.willowcreekcarecenter.org '

update AttributeValue set value = @siteemail 
from attribute a
join AttributeValue v on v.AttributeId = a.Id
where a.[key] = 'OrganizationEmail'

update V set value = @protectmyministryusername
from AttributeValue V
join Attribute A on V.AttributeId = A.Id
join EntityType E on E.Id = A.EntityTypeId
where E.Name = 'Rock.Security.BackgroundCheck.ProtectMyMinistry'
and A.[Key] = 'UserName'

update V set value = @protectmyministrypassword
from AttributeValue V
join Attribute A on V.AttributeId = A.Id
join EntityType E on E.Id = A.EntityTypeId
where E.Name = 'Rock.Security.BackgroundCheck.ProtectMyMinistry'
and A.[Key] = 'Password'

update V set value = @protectmyministrytestmode
from AttributeValue V
join Attribute A on V.AttributeId = A.Id
join EntityType E on E.Id = A.EntityTypeId
where E.Name = 'Rock.Security.BackgroundCheck.ProtectMyMinistry'
and A.[Key] = 'TestMode'

update V set value = @emailserver
from AttributeValue V
join Attribute A on V.AttributeId = A.Id
join EntityType E on E.Id = A.EntityTypeId
where E.Name = 'Rock.Communication.Transport.SMTP'
and A.[Key] = 'Server'

update V set value = @emailport
from AttributeValue V
join Attribute A on V.AttributeId = A.Id
join EntityType E on E.Id = A.EntityTypeId
where E.Name = 'Rock.Communication.Transport.SMTP'
and A.[Key] = 'Port'

update V set value = @emailusername
from AttributeValue V
join Attribute A on V.AttributeId = A.Id
join EntityType E on E.Id = A.EntityTypeId
where E.Name = 'Rock.Communication.Transport.SMTP'
and A.[Key] = 'UserName'

update V set value = @emailpassword
from AttributeValue V
join Attribute A on V.AttributeId = A.Id
join EntityType E on E.Id = A.EntityTypeId
where E.Name = 'Rock.Communication.Transport.SMTP'
and A.[Key] = 'Password'

update V set value = @ProtectMyMinistryWebHook
from AttributeValue V
join Attribute A on v.AttributeId = A.Id
join EntityType E on E.Id = A.EntityTypeId
where E.Name = 'Rock.Security.BackgroundCheck.ProtectMyMinistry'
and A.[Key] = 'ReturnURL'

update V set value = @SignNowWebHook
from AttributeValue V
join Attribute A on v.AttributeId = A.Id
join EntityType E on E.Id = a.EntityTypeId
where E.Name = 'Rock.SignNow.SignNow'
And A.[Key] = 'WebhookUrl'

-- Update html content links
update htmlcontent
set content = replace(content, 'https://rock.willowcreek.org', 'http://' + @sitename )
where content like '%https://rock.willowcreek.org%'

update htmlcontent
set content = replace(content, 'https://rockcc.willowcreek.org', 'http://' + @carecentersite )
where content like '%https://rockcc.willowcreek.org%'

update htmlcontent
set content = replace(content, 'https://portal.willowcreekcarecenter.org', 'http://' + @carecenterportal )
where content like '%https://portal.willowcreekcarecenter.org%'

-- Update any attribute values with fully qualified domain
update attributevalue
set [value] = replace([value], 'https://rock.willowcreek.org', 'http://' + @sitename )
where [value] like '%https://rock.willowcreek.org%'

update attributevalue
set [value] = replace([value], 'https://rockcc.willowcreek.org', 'http://' + @carecentersite )
where [value] like '%https://rockcc.willowcreek.org%'

update attributevalue
set [value] = replace([value], 'https://portal.willowcreekcarecenter.org', 'http://' + @carecenterportal)
where [value] like '%https://portal.willowcreekcarecenter.org%'

-- Update any email templates with fully qualified domain
update CommunicationTemplate
set [MediumDataJson] = replace([MediumDataJson], 'https://rock.willowcreek.org', 'http://' + @sitename )
where [MediumDataJson] like '%https://rock.willowcreek.org%'

update CommunicationTemplate
set [MediumDataJson] = replace([MediumDataJson], 'https://rockcc.willowcreek.org', 'http://' + @carecentersite )
where [MediumDataJson] like '%https://rockcc.willowcreek.org%'

update CommunicationTemplate
set [MediumDataJson] = replace([MediumDataJson], 'https://portal.willowcreekcarecenter.org', 'http://' + @carecenterportal)
where [MediumDataJson] like '%https://portal.willowcreekcarecenter.org%'

-- Update any system templates with fully qualified domain
update SystemEmail
set [Body] = replace([Body], 'https://rock.willowcreek.org', 'http://' + @sitename )
where [Body] like '%https://rock.willowcreek.org%'

update SystemEmail
set [Body] = replace([Body], 'https://rockcc.willowcreek.org', 'http://' + @carecentersite )
where [Body] like '%https://rockcc.willowcreek.org%'

update SystemEmail
set [Body] = replace([Body], 'https://portal.willowcreekcarecenter.org', 'http://' + @carecenterportal)
where [Body] like '%https://portal.willowcreekcarecenter.org%'

-- Update any workflow form fields pre/post html with fully qualified domain
update WorkflowActionFormAttribute set 
[PreHtml] = replace([PreHtml], 'https://rock.willowcreek.org', 'http://' + @sitename ),
[PostHtml] = replace([PostHtml], 'https://rock.willowcreek.org', 'http://' + @sitename )
where [PreHtml] like '%https://rock.willowcreek.org%'
or [PostHtml] like '%https://rock.willowcreek.org%'

update WorkflowActionFormAttribute set
[PreHtml] = replace([PreHtml], 'https://rockcc.willowcreek.org', 'http://' + @carecentersite ),
[PostHtml] = replace([PostHtml], 'https://rockcc.willowcreek.org', 'http://' + @carecentersite )
where [PreHtml] like '%https://rockcc.willowcreek.org%'
or [PostHtml] like '%https://rockcc.willowcreek.org%'

update WorkflowActionFormAttribute set
[PreHtml] = replace([PreHtml], 'https://portal.willowcreekcarecenter.org', 'http://' + @carecenterportal ),
[PostHtml] = replace([PostHtml], 'https://portal.willowcreekcarecenter.org', 'http://' + @carecenterportal )
where [PreHtml] like '%https://portal.willowcreekcarecenter.org%'
or [PostHtml] like '%https://portal.willowcreekcarecenter.org%'


-- Make sure Sql can be run in a block so the header will work
update V
set Value = Value + ',Sql'
from Attribute A 
join AttributeValue V on A.ID = V.AttributeId
where [Key] = 'DefaultEnabledLavaCommands' and Value not like '%Sql%'

UPDATE [Block]  
SET PreHtml =  '<h4 style="position:absolute;left:80px;top:6px; color: black;">You are connected to the <strong>{% sql %}SELECT db = DB_NAME(){% endsql %}{% for item in results %}{{ item.db }}{% endfor %}</strong> database.</h4><script>$(".navbar-static-top").css("background-color","burlywood");</script>'
, ModifiedDateTime = GETDATE()
WHERE [BlockTypeId] = 254 
and Zone = 'Header'

UPDATE [Block]  
SET PreHtml =  '<style>
    body:after{
        background-color: #ca4040;
        color: #fff;
        content: ''Caution: You are on Care Center Test'';
        width: 100%;
        padding: 6px 24px;
        font-size: 16px;
        position: absolute;
        top: 0;
        left: 0;
        right: 0;
        text-align: center;
    }
</style>', ModifiedDateTime = GETDATE()
WHERE [Id] IN ( 926, 924, 928) 

-- This does not seem to have any impact on the document's title, see if we can find where that is getting populated from
--update site set name = @sitetitle where Guid = 'C2D29296-6A87-47A9-A753-EE4E9159C4C4' --Site Title

-- disable all jobs, except:
update ServiceJob set isactive = 0 where [guid] not in ('CB24FF2A-5AD3-4976-883F-DAF4EFC1D7C7', '1A8238B1-038A-4295-9FDE-C6D93002A5D7', '35EABBDB-1EFA-46F1-86D4-4199FFA2D9A7', 'F1FB3E17-93A6-42E5-BE1A-8FA3AAD2B3AB')

-- delete privacy data
delete av
from category c
join AttributeCategory ac on c.id = ac.CategoryId
join Attribute a on ac.AttributeId = a.id
join AttributeValue av on a.id = av.AttributeId
 where c.name in ('Pastoral Concerns','Protection Restrictions')

 /*
  -- delete privacy leadership and protection questions
 delete from _org_willowcreek_LeadershipInterview
 delete from _org_willowcreek_PersonDocument
 delete from _org_willowcreek_ProtectionApp_Reference
 delete from _org_willowcreek_ProtectionApp_Questionnaire
 
 -- delete privacy documents
 update person set photoid = null

 delete from binaryfiledata

--Delete binary file data
delete from binaryfile



 --select * 
 --from BinaryFileType bft
 --join BinaryFile bf on bft.Id = bf.BinaryFileTypeId
 --join BinaryFileData bfd on bf.Id = bfd.
 --where [guid] in ('408069CD-D061-4DE9-AA7B-1A73FCDA915D','F7D64DA5-F0E8-4231-A4FF-59CA2420E9B8','1C62A2FD-EE43-433A-9601-5719BC4C9C3F','1D99AD26-FCAF-48D4-8508-4F5307F4A7F9','1609EB30-5234-4BAA-9B8F-A8966839029D')
 */
 -- disable all workflows
 update workflow
 set completeddatetime = getdate(), [status] = 'disabled'
 where completeddatetime is null

 -- delete other tables
delete from _org_willowcreek_IMPORT_person
delete from _org_willowcreek_IMPORT_grouprole_map
delete from _org_willowcreek_IMPORT_group_exclude
delete from _org_willowcreek_IMPORT_group
delete from _org_willowcreek_IMPORT_family_member
delete from _org_willowcreek_IMPORT_family_address
delete from _org_willowcreek_IMPORT_family

--TODO:
--disable communications transports Mandrill
--disable trilio
--activate smtp

--disable communication mediums SMS
--change email transport container to SMTP

--delete just privacy documents (instead of all)

--shrink database

--adjust Internal Application Root vs Organization Website vs Public Application Root

--admin email addresses


--Add Care Center Test Accounts
DECLARE @FamilyName varchar(max) = 'Test_CareCenter';

IF OBJECT_ID('tempdb..#NewUsers') IS NOT NULL
	DROP TABLE #NewUsers

--Create New Users
CREATE TABLE #NewUsers
( FirstName varchar(max),
  LastName varchar(max),
  [Password] varchar(max),
  [role] varchar(max)
)

INSERT INTO #NewUsers VALUES('Admin_CareCenter',@FamilyName, '$2a$11$5A77gc2ToCkgrxiXzexw1eql1yWKk7.QRR2ohsF2ah4u4g90HcHzC', 'Care Center: Admin')
INSERT INTO #NewUsers VALUES('BasicCC',@FamilyName, '$2a$11$jkoCV68qWSd0VidwpOPIzObdyBgdcCDCZ.Ha/UTHAGyr7smD/7tw.', 'Care Center: Basic')
INSERT INTO #NewUsers VALUES('CareTeamCC',@FamilyName, '$2a$11$75PeYmUxDHbiio74WVs8leH38Wb.LBsXaW30JxiIjIGB1RrdDy5Ou', 'Care Center: Care Team')
INSERT INTO #NewUsers VALUES('CTLeaderCC',@FamilyName, '$2a$11$A/LZZBLn9j6HaViVb4ZL9uBGbQBq2XOytqrCm3lAdOg3wQoEj3dAG', 'Care Center: Care Team Leader')
INSERT INTO #NewUsers VALUES('Cars_CareCenter',@FamilyName, '$2a$11$/CjcrRAE0i6yuSijAd9snOEdvSy9.4FAihVV8YCE.jF.xcIDAgu3G', 'Care Center: Cars')

IF((SELECT COUNT(*) FROM UserLogin WHERE UserName IN (SELECT FirstName FROM #NewUsers)) = 0)
BEGIN
	--Create Family
	INSERT [dbo].[Group]([IsSystem], [ParentGroupId], [GroupTypeId], [CampusId], [ScheduleId], [Name], [Description], [IsSecurityRole], [IsActive], [Order], [AllowGuests], [WelcomeSystemEmailId], [ExitSystemEmailId], [SyncDataViewId], [AddUserAccountsDuringSync], [MustMeetRequirementsToAddMember], [IsPublic], [GroupCapacity], [RequiredSignatureDocumentTemplateId], [CreatedDateTime], [ModifiedDateTime], [CreatedByPersonAliasId], [ModifiedByPersonAliasId], [Guid], [ForeignId], [ForeignGuid], [ForeignKey])
	VALUES (0, NULL, 10, 1, NULL, @FamilyName + ' Family', NULL, 0, 1, 0, NULL, NULL, NULL, NULL, NULL, NULL, 1, NULL, NULL, GetDate(), GETDATE(), 1, 1, NEWID(), NULL, NULL, NULL)
	DECLARE @FamilyGroupId INT = (SELECT SCOPE_IDENTITY());
	SELECT @FamilyGroupId;

	--Create Person Records
	DECLARE @FirstName varchar(max), @LastName varchar(max), @Password varchar(max), @role varchar(max);
	DECLARE db_cursor CURSOR FOR 
	SELECT FirstName, LastName, [Password], [role]
	FROM #NewUsers

	OPEN db_cursor
	FETCH NEXT FROM db_Cursor INTO @FirstName, @LastName, @Password, @role

	WHILE @@FETCH_STATUS = 0
	BEGIN
		INSERT [dbo].[Person]([IsSystem], [RecordTypeValueId], [RecordStatusValueId], [RecordStatusLastModifiedDateTime], [RecordStatusReasonValueId], [ConnectionStatusValueId], [ReviewReasonValueId], [IsDeceased], [TitleValueId], [FirstName], [NickName], [MiddleName], [LastName], [SuffixValueId], [PhotoId], [BirthDay], [BirthMonth], [BirthYear], [Gender], [MaritalStatusValueId], [AnniversaryDate], [GraduationYear], [GivingGroupId], [Email], [IsEmailActive], [EmailNote], [EmailPreference], [ReviewReasonNote], [InactiveReasonNote], [SystemNote], [ViewedCount], [CreatedDateTime], [ModifiedDateTime], [CreatedByPersonAliasId], [ModifiedByPersonAliasId], [Guid], [ForeignId], [ForeignGuid], [ForeignKey])
		VALUES (0, 1, 3, NULL, NULL, 146, NULL, 0, NULL, @FirstName, @FirstName, '', @LastName, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, 'noone@willowcreek.org', 1, NULL, 0, NULL, NULL, NULL, NULL, GETDATE(), GETDATE(), 1, 1, NEWID(), NULL, NULL, NULL)

		DECLARE @PersonId INT = (SELECT SCOPE_IDENTITY());
		DECLARE @PersonGuid UNIQUEIDENTIFIER = (SELECT GUID FROM PERSON WHERE ID = @PersonId);

		INSERT [dbo].[GroupMember]([IsSystem], [GroupId], [PersonId], [GroupRoleId], [Note], [GroupMemberStatus], [GuestCount], [DateTimeAdded], [IsNotified], [CreatedDateTime], [ModifiedDateTime], [CreatedByPersonAliasId], [ModifiedByPersonAliasId], [Guid], [ForeignId], [ForeignGuid], [ForeignKey])
		VALUES (0, @FamilyGroupId, @PersonId, 3, NULL, 1, NULL, GETDATE(), 0, GETDATE(), GETDATE(), 1, 1, NEWID(), NULL, NULL, NULL)

		INSERT [dbo].[PersonAlias]([Name], [PersonId], [AliasPersonId], [AliasPersonGuid], [Guid], [ForeignId], [ForeignGuid], [ForeignKey])
		VALUES (NULL, @PersonId, @PersonId, @PersonGuid, NEWID(), NULL, NULL, NULL)
	
		INSERT [dbo].[UserLogin]([EntityTypeId], [UserName], [Password], [IsConfirmed], [LastActivityDateTime], [LastLoginDateTime], [LastPasswordChangedDateTime], [IsOnLine], [IsLockedOut], [IsPasswordChangeRequired], [LastLockedOutDateTime], [FailedPasswordAttemptCount], [FailedPasswordAttemptWindowStartDateTime], [LastPasswordExpirationWarningDateTime], [ApiKey], [PersonId], [CreatedDateTime], [ModifiedDateTime], [CreatedByPersonAliasId], [ModifiedByPersonAliasId], [Guid], [ForeignId], [ForeignGuid], [ForeignKey])
		VALUES (27, @FirstName, @Password, 1, NULL, NULL, GETDATE(), NULL, 0, 0, NULL, NULL, NULL, NULL, NULL, @PersonId, GETDATE(), GETDATE(), 1, 1, NEWID(), NULL, NULL, NULL)

		INSERT [dbo].[GroupMember]([IsSystem], [GroupId], [PersonId], [GroupRoleId], [Note], [GroupMemberStatus], [GuestCount], [DateTimeAdded], [IsNotified], [CreatedDateTime], [ModifiedDateTime], [CreatedByPersonAliasId], [ModifiedByPersonAliasId], [Guid], [ForeignId], [ForeignGuid], [ForeignKey])
		VALUES (0, (SELECT TOP 1 Id FROM [Group] WHERE Name = @role), @PersonId, 1, '', 1, NULL, GETDATE(), 0, GETDATE(), GETDATE(), 1, 1, NEWID(), NULL, NULL, NULL)

		--Add all test users to the basic role.
		IF(@role <> 'Care Center: Basic')
		BEGIN
			INSERT [dbo].[GroupMember]([IsSystem], [GroupId], [PersonId], [GroupRoleId], [Note], [GroupMemberStatus], [GuestCount], [DateTimeAdded], [IsNotified], [CreatedDateTime], [ModifiedDateTime], [CreatedByPersonAliasId], [ModifiedByPersonAliasId], [Guid], [ForeignId], [ForeignGuid], [ForeignKey])
			VALUES (0, (SELECT TOP 1 Id FROM [Group] WHERE Name = 'Care Center: Basic'), @PersonId, 1, '', 1, NULL, GETDATE(), 0, GETDATE(), GETDATE(), 1, 1, NEWID(), NULL, NULL, NULL)
		END

		FETCH NEXT FROM db_cursor INTO @FirstName, @LastName, @Password, @role
	END
	CLOSE db_cursor
	DEALLOCATE db_cursor
END