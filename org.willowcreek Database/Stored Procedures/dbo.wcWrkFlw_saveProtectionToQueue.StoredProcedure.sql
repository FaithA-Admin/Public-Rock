/****** Object:  StoredProcedure [dbo].[wcWrkFlw_saveProtectionToQueue]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcWrkFlw_saveProtectionToQueue]
GO
/****** Object:  StoredProcedure [dbo].[wcWrkFlw_saveProtectionToQueue]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[wcWrkFlw_saveProtectionToQueue]
	@requesterGuid VARCHAR(36),
	@campus VARCHAR(100),
	@ministry VARCHAR(100),
	@applicantGuid VARCHAR(36),
	@workflowId INT
AS
BEGIN

	DECLARE @queueGuid UNIQUEIDENTIFIER = NEWID()

	DECLARE @now DATETIME = GETDATE()

	DECLARE @requesterFirstName VARCHAR(50)
	DECLARE @requesterLastName VARCHAR(50)

	SELECT @requesterFirstName = p.FirstName, @requesterLastName = p.LastName
	FROM PersonAlias pa
	JOIN Person p ON pa.PersonId = p.Id
	WHERE pa.[Guid] = @requesterGuid

	DECLARE @ApplicantFirstName VARCHAR(50)
	DECLARE @ApplicantLastName VARCHAR(50)

	SELECT @ApplicantFirstName = p.FirstName, @ApplicantLastName = p.LastName
	FROM PersonAlias pa
	JOIN Person p ON pa.PersonId = p.Id
	WHERE pa.[Guid] = @applicantGuid

	INSERT INTO _org_willowcreek_ProtectionApp_Queue
	([Guid], CreatedDateTime, RequesterPersonAliasGuid, RequesterFirstName, RequesterLastName, Campus, Ministry, ApplicantPersonAliasGuid, ApplicantFirstName, ApplicantLastName, WorkflowId)
	VALUES
	(@queueGuid, @now, @requesterGuid, @requesterFirstName, @requesterLastName, @campus, @ministry, @applicantGuid, @ApplicantFirstName, @ApplicantLastName, @workflowId)

	SELECT @queueGuid

END


GO
