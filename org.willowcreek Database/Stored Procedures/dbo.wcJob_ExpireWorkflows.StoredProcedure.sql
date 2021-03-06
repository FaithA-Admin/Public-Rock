USE [Rock]
GO
/****** Object:  StoredProcedure [dbo].[wcJob_ExpireWorkflows]    Script Date: 4/25/2017 2:05:23 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[wcJob_ExpireWorkflows]
AS
BEGIN

	DECLARE @Status VARCHAR(MAX) = ''
	DECLARE @ProtectionWorkflows INT = 0

	-- Protection Requests complete on their own. Anything older than 12 hours should be expired
	update w
	set status = 'Expired', CompletedDateTime = GETDATE()
	from workflow w
	join Attribute WA on WA.EntityTypeID = 113 and WA.[Key] = 'Applicant'
	join AttributeValue WAV on WAV.AttributeID = WA.ID and WAV.EntityID = w.ID
	join PersonAlias PA on CAST(WAV.[Value] as varchar(max)) = CAST(PA.[Guid] as varchar(max))
	join Person P on PA.PersonID = P.ID
	where w.workflowtypeid = 52 
	and w.CompletedDateTime is null
	and DATEADD(hour, 12, ActivatedDateTime) < GETDATE()

	SET @ProtectionWorkflows = @ProtectionWorkflows + @@ROWCOUNT

	-- Most protection workflows for approved/rejected/expired people should be expired
	-- Updated written this way to avoid "Conversion failed when converting from a character string to uniqueidentifier."
	update Workflow
	set status = 'Expired', CompletedDateTime = GETDATE()
	where id in (
		select w.id
		from workflow w
		join Attribute WA on WA.EntityTypeID = 113 and WA.[Key] = 'Applicant'
		join AttributeValue WAV on WAV.AttributeID = WA.ID and WAV.EntityID = w.ID
		join PersonAlias PA on CAST(WAV.[Value] as varchar(max)) = CAST(PA.[Guid] as varchar(max))
		join Person P on PA.PersonID = P.ID
		left join (select PSV.* from Attribute PSA
			join AttributeValue PSV on PSV.AttributeId = PSA.Id
			where PSA.EntityTypeID = 15 and PSA.[Key] = 'ProtectionStatus') ps on ps.EntityID = P.ID
		where (w.workflowtypeid in (53, 54, 55, 57) or (w.WorkflowTypeId = 56 and w.CreatedByPersonAliasId is not null)) -- Ignore batch background checks
		and w.CompletedDateTime is null
		and (ps.value is null or (
			PS.Value not in ('d6751337-e6a8-4c32-9b40-270c50a561ea' -- Process Initated
							, 'E4A8815E-6300-45D0-A07F-068698A39638' -- In Progress
							, '787DFC12-FB8B-4CEB-B98A-FE8125CBB665') -- Needs review
							))
						)

	SET @ProtectionWorkflows = @ProtectionWorkflows + @@ROWCOUNT

	-- Batch background checks should only be expired if the activated date is before the applicant's BC date
	update w
	set status = 'Expired', CompletedDateTime = GETDATE()
	from workflow w
	join Attribute WA on WA.EntityTypeID = 113 and WA.[Key] = 'Applicant'
	join AttributeValue WAV on WAV.AttributeID = WA.ID and WAV.EntityID = w.ID
	join PersonAlias PA on CAST(WAV.[Value] as varchar(max)) = CAST(PA.[Guid] as varchar(max))
	join Person P on PA.PersonID = P.ID
	left join (select PSV.* from Attribute PSA
		join AttributeValue PSV on PSV.AttributeId = PSA.Id
		where PSA.EntityTypeID = 15 and PSA.[Key] = 'ProtectionStatus') ps on ps.EntityID = P.ID
	left join (select PSV.* from Attribute PSA
		join AttributeValue PSV on PSV.AttributeId = PSA.Id
		where PSA.EntityTypeID = 15 and PSA.[Key] = 'BackgroundCheckDate') bcd on bcd.EntityID = P.ID
	where w.workflowtypeid = 56
	and w.CompletedDateTime is null
	and w.CreatedByPersonAliasId is null -- Batch
	and (ps.value is null or (
		PS.Value not in ('d6751337-e6a8-4c32-9b40-270c50a561ea' -- Process Initated
						, 'E4A8815E-6300-45D0-A07F-068698A39638' -- In Progress
						, '787DFC12-FB8B-4CEB-B98A-FE8125CBB665') -- Needs review
						))
	and w.ActivatedDateTime is not null
	and bcd.ValueAsDateTime is not null
	and CONVERT(DATE, w.ActivatedDateTime) <= CONVERT(DATE, bcd.ValueAsDateTime) -- Activated date is earlier than the background check approved date

	SET @ProtectionWorkflows = @ProtectionWorkflows + @@ROWCOUNT

	SET @Status = @Status + CASE WHEN LEN(@Status) = 0 THEN 'Expired ' ELSE ', ' END + CONVERT(VARCHAR(MAX), @ProtectionWorkflows) + ' Protection Workflows'

	update ServiceJob set LastStatusMessage = @Status where Name = 'Willow Expire Workflows'

END
