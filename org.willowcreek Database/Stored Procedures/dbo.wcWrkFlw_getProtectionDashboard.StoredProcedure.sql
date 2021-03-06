DROP PROCEDURE [dbo].[wcWrkFlw_getProtectionDashboard]
GO
SET ANSI_NULLS on
GO
SET QUOTED_IDENTIFIER on
GO
-- [wcWrkFlw_getProtectionDashboard] 41119, ''
CREATE  PROCEDURE [dbo].[wcWrkFlw_getProtectionDashboard] 
	 @CurrentPersonId int,
     @LastName varchar(50)
as begin
	if @LastName = 'LastName'
		begin
			set @LastName ='';
		end;

	declare @YouthCovenantRequestWorkflowTypeID INT
		, @RequesterAttributeID INT
		, @ApplicantAttributeID INT

	select @YouthCovenantRequestWorkflowTypeID = id from workflowtype where Guid = 'DF7B43B6-E677-463D-8BD1-0DC50FF17D99'

	select @RequesterAttributeID = id from attribute where EntityTypeId = 113 and EntityTypeQualifierValue = @YouthCovenantRequestWorkflowTypeID and [Key] = 'Requester'
	select @ApplicantAttributeID = id from attribute where EntityTypeId = 113 and EntityTypeQualifierValue = @YouthCovenantRequestWorkflowTypeID and [Key] = 'Applicant'

	-- Admin Dashboard
	if (select count(distinct PersonId) from GroupMember where GroupId in (2, 137) and PersonId = @CurrentPersonId) = 1 begin

		-- Workflows that are waiting on the Protection Team
		with CTE as (
			select WorkflowId = w.Id
				, WorkflowTypeName = wt.Name
				, WorkflowTypeId = w.WorkflowTypeId
				, WorkflowName = w.Name
				, WorkflowStatus = w.[Status]
				, w.LastProcessedDateTime
				, pa.PersonId
				, rn = row_number() over (partition by pa.PersonId, w.WorkflowTypeId order by w.LastProcessedDateTime desc)	  
			from Workflow w
			join WorkflowType wt on wt.Id = w.WorkflowTypeId
			join AttributeValue av on av.EntityId = w.Id
			join Attribute a on av.AttributeId = a.Id and a.EntityTypeId = 113 and a.EntityTypeQualifierColumn = 'WorkflowTypeId' and a.EntityTypeQualifierValue in (50, 56) and a.[Key] = 'Applicant'
			join PersonAlias pa on convert(varchar(36), pa.[Guid]) = av.Value
			where w.[Status] = 'Waiting on Protection Team'
		)
		select * 
		into #WaitingOnProtectionTeamWorkflows
		from CTE
		where rn = 1
		
		create table #Workflows ([Date] datetime, WorkflowTypeID int, WorkflowID int, WorkflowStatus varchar(100), RequesterFirstName varchar(100), RequesterLastName varchar(100)
			, ApplicantPersonID int, ApplicantFirstName varchar(100), ApplicantLastName varchar(100), ProtectionStatusGuid uniqueidentifier)

		-- Outstanding Protection Requests
		insert into #Workflows
		select W.CreatedDateTime, W.WorkflowTypeID, W.ID, W.Status, ReqP.FirstName, ReqP.LastName, AppP.ID, AppP.FirstName, AppP.LastName, try_cast(ProtAV.Value as uniqueidentifier)
		from Workflow W
		join AttributeValue ReqAV on ReqAV.EntityId = W.ID and ReqAV.AttributeID in (2631)
		join PersonAlias ReqPA on try_cast(ReqAV.Value as uniqueidentifier) = ReqPA.Guid -- CONVERT(VARCHAR(36), ReqPA.Guid)
		join Person ReqP on ReqPA.PersonID = ReqP.ID
		join AttributeValue AppAV on AppAV.EntityId = W.ID and AppAV.AttributeID in (2632)
		join PersonAlias AppPA on try_cast(AppAV.Value as uniqueidentifier) = AppPA.Guid -- CONVERT(VARCHAR(36), AppPA.Guid)
		join Person AppP on AppPA.PersonID = AppP.ID
		join AttributeValue ProtAV on ProtAV.EntityId = AppPA.PersonId and ProtAV.AttributeId = 1731
		where W.WorkflowTypeId = 52 
		--and W.CreatedDateTime > dateadd(day, -45, getdate()) 
		and W.Status = 'Completed'
		and ProtAV.Value IN ('E4A8815E-6300-45D0-A07F-068698A39638', '787DFC12-FB8B-4CEB-B98A-FE8125CBB665', 'D6751337-E6A8-4C32-9B40-270C50A561EA') -- Needs Review, In Progress, Process Initiated

		-- Outstanding automated background checks
		insert into #Workflows
		select W.CreatedDateTime, W.WorkflowTypeID, W.ID, W.Status, ReqP.FirstName, ReqP.LastName, AppP.ID, AppP.FirstName, AppP.LastName, try_cast(ProtAV.Value as uniqueidentifier)
		from Workflow W
		join AttributeValue ReqAV on ReqAV.EntityId = W.ID and ReqAV.AttributeID in (2704)
		join PersonAlias ReqPA on try_cast(ReqAV.Value as uniqueidentifier) = ReqPA.Guid -- CONVERT(VARCHAR(36), ReqPA.Guid)
		join Person ReqP on ReqPA.PersonID = ReqP.ID
		join AttributeValue AppAV on AppAV.EntityId = W.ID and AppAV.AttributeID in (2715)
		join PersonAlias AppPA on try_cast(AppAV.Value as uniqueidentifier) = AppPA.Guid -- CONVERT(VARCHAR(36), AppPA.Guid)
		join Person AppP on AppPA.PersonID = AppP.ID
		join AttributeValue ProtAV on ProtAV.EntityId = AppPA.PersonId and ProtAV.AttributeId = 1731
		where W.WorkflowTypeId = 56 
		and W.completedDateTime is null 
		and AppAV.Value is not null 
		and W.Status in ('Waiting On Protection Team', 'Waiting for Review') 
		and ReqAV.[Value] = '6E90CAA2-7C84-4084-BE4B-5AC149EE0475' --Administrator (Batch Background checks have Administrator set as the requester)
		
		-- Outstanding Youth Covenant Requests
		insert into #Workflows
		select W.CreatedDateTime, W.WorkflowTypeID, W.ID, W.Status, ReqP.FirstName, ReqP.LastName, AppP.ID, AppP.FirstName, AppP.LastName, try_cast(YouthAV.Value as uniqueidentifier)
		from Workflow W
		join AttributeValue ReqAV on ReqAV.EntityId = W.ID and ReqAV.AttributeID in (@RequesterAttributeID)
		join PersonAlias ReqPA on try_cast(ReqAV.Value as uniqueidentifier) = ReqPA.Guid -- CONVERT(VARCHAR(36), ReqPA.Guid)
		join Person ReqP on ReqPA.PersonID = ReqP.ID
		join AttributeValue AppAV on AppAV.EntityId = W.ID and AppAV.AttributeID in (@ApplicantAttributeID)
		join PersonAlias AppPA on try_cast(AppAV.Value as uniqueidentifier) = AppPA.Guid -- CONVERT(VARCHAR(36), AppPA.Guid)
		join Person AppP on AppPA.PersonID = AppP.ID
		join AttributeValue YouthAV on YouthAV.EntityId = AppPA.PersonId and YouthAV.AttributeId = 2395
		where W.WorkflowTypeId = @YouthCovenantRequestWorkflowTypeID
		and W.Status = 'Completed'
		and YouthAV.Value IN ('3B2FB5F8-CE29-4005-A89A-B3DBAF726E1F', '9FC53424-7719-4EA3-BBE8-4393CEE91C96', '523D007B-7591-4852-828C-9A6F4470E4E8'); -- Process Initiated, In Progress, Needs Review
			
		-- All protection attributes for each person in the query above
		with CTE as (
			select Value = case when ValueAsDateTime is not null then convert(varchar(10), ValueAsDateTime, 101) else Value end
				, PersonId = EntityId
				, AttributeKey = [Key]
			from Attribute A
			join AttributeValue V on V.AttributeId = A.ID and len(V.Value) > 0
			where EntityTypeId = 15
			and [Key] in ('ProtectionApplicationDate', 'ProtectionApplication', 'BackgroundCheckDate', 'BackgroundCheckDocument', 'ProtectionReference1Date', 'ProtectionReference1', 
							'ProtectionReference2Date', 'ProtectionReference2', 'ProtectionReference3Date', 'ProtectionReference3', 'PolicyAcknowledgmentDate',
							'YouthCovenantDate', 'YouthCovenant', 'YouthCovenantReference1Date', 'YouthCovenantReference1')
			and exists (select * from #Workflows W where W.ApplicantPersonID = V.EntityID)
			)
		select *
		into #ProtectionAttributes
		from CTE 
		pivot(max(Value) for AttributeKey in (ProtectionApplicationDate, ProtectionApplication, BackgroundCheckDate, BackgroundCheckDocument, ProtectionReference1Date, ProtectionReference1, 
												ProtectionReference2Date, ProtectionReference2, ProtectionReference3Date, ProtectionReference3, PolicyAcknowledgmentDate,
												YouthCovenantDate, YouthCovenant, YouthCovenantReference1Date, YouthCovenantReference1)) P;

		-- All the outstanding protection apps 
		select Date
			, Applicant = '<a href=''/Person/' + cast(ApplicantPersonId as varchar) + '/ExtendedAttributes'' target="_blank">' + ApplicantLastName + ', ' + ApplicantFirstName + '</a>'
			, Status = ProtDV.Value
			, [B/C] = case when W.WorkflowTypeID = @YouthCovenantRequestWorkflowTypeID then ''
							when PR.BackgroundCheckDocument is not null then isnull('<a href=''/GetFile.ashx?guid=' + PR.BackgroundCheckDocument + + ''' target="_blank">' + isnull(PR.BackgroundCheckDate,'Review') + '</a>','')
							else isnull('<a href=''/WorkflowEntry/' + cast(wotw.WorkflowTypeId as nvarchar(max)) + '/' + cast(wotw.WorkflowId as nvarchar(max)) + ''' target="_blank">' + wotw.WorkflowStatus + '</a>','') end
			, [Ref 1] = case when W.WorkflowTypeID = @YouthCovenantRequestWorkflowTypeID then
								case when pr.YouthCovenantReference1 is not null then isnull('<a href=''/GetFile.ashx?guid=' + pr.YouthCovenantReference1 + + ''' target="_blank">' + isnull(pr.YouthCovenantReference1Date,'Review') + '</a>','')
								else isnull(pr.YouthCovenantReference1Date, '') end
							else
								case when pr.ProtectionReference1 is not null then isnull('<a href=''/GetFile.ashx?guid=' + pr.ProtectionReference1 + + ''' target="_blank">' + isnull(pr.ProtectionReference1Date,'Review') + '</a>','')
								else isnull(pr.ProtectionReference1Date, '') end
							end
			, [Ref 2] = case when W.WorkflowTypeID = @YouthCovenantRequestWorkflowTypeID then ''
							when pr.ProtectionReference2 is not null then isnull('<a href=''/GetFile.ashx?guid=' + pr.ProtectionReference2 + + ''' target="_blank">' + isnull(pr.ProtectionReference2Date,'Review') + '</a>','')
							else isnull(pr.ProtectionReference2Date, '') end
			, [Ref 3] = case when W.WorkflowTypeID = @YouthCovenantRequestWorkflowTypeID then ''
							when pr.ProtectionReference3 is not null then isnull('<a href=''/GetFile.ashx?guid=' + pr.ProtectionReference3 + + ''' target="_blank">' + isnull(pr.ProtectionReference3Date,'Review') + '</a>','')
							else isnull(pr.ProtectionReference3Date, '') end
			, [App] = case when W.WorkflowTypeID = @YouthCovenantRequestWorkflowTypeID then 
								case when PR.YouthCovenant is not null then isnull('<a href=''/GetFile.ashx?guid=' + PR.YouthCovenant + + ''' target="_blank">' + isnull(PR.YouthCovenantDate,'Review') + '</a>','')
								else isnull(PR.YouthCovenantDate, '') end
							else
								case when PR.ProtectionApplication is not null then isnull('<a href=''/GetFile.ashx?guid=' + PR.ProtectionApplication + + ''' target="_blank">' + isnull(PR.ProtectionApplicationDate,'Review') + '</a>','')
								else isnull(PR.ProtectionApplicationDate, '') end
							end
			, [Policy] = case when W.WorkflowTypeID = @YouthCovenantRequestWorkflowTypeID then '' else isnull(PR.PolicyAcknowledgmentDate,'') end
			, Requester = RequesterLastName + ', ' + RequesterFirstName
			, WrkFlw = '<a href=''/page/291?workflowId=' + cast(W.WorkflowID as varchar) + ''' target="_blank">' + W.WorkflowStatus + '</a>' 
		into #AdminDashboard
		from #Workflows W
		join DefinedValue ProtDV on cast(ProtDV.[Guid] as nvarchar(max)) = ProtectionStatusGuid
		left join #ProtectionAttributes PR on PR.PersonId = W.ApplicantPersonID
		left join #WaitingOnProtectionTeamWorkflows wotw on wotw.PersonId = W.ApplicantPersonID
		union all 
		-- HR Background Checks that need review
		select	Date = w.ActivatedDateTime
				, Applicant = '<a href=''/Person/' + cast(p_app.Id as varchar) + '/ExtendedAttributes'' target="_blank">' + p_app.LastName + ', ' + p_app.FirstName + '</a>'
				, Status = 'HR BC Needs Review'
				, [B/C] = isnull('<a href=''/WorkflowEntry/' + cast(wt.id as nvarchar(max)) + '/' + cast(w.Id as nvarchar(max)) + ''' target="_blank">' + w.Status + '</a>','')
				, [Ref 1] = ''
				, [Ref 2] = ''
				, [Ref 3] = ''
				, [App] = ''
				, [Policy] = ''
				, Requester = p_req.LastName + ', ' + p_req.FirstName
				, WrkFlw = '<a href=''/page/291?workflowId=' + cast(w.id as varchar) + ''' target="_blank">' + w.[Status] + '</a>'
		from Workflow w
		join WorkflowType wt on w.WorkflowTypeId = wt.Id
		join PersonAlias pa_req on pa_req.id = w.InitiatorPersonAliasId
		join Person p_req on p_req.id = pa_req.PersonId
		join AttributeValue av on av.EntityId = w.Id and av.AttributeId = 9956 --HR BC Applicant Attribute ID
		join PersonAlias pa_app on cast(pa_app.Guid as varchar(max)) = av.Value
		join Person p_app on p_app.Id = pa_app.PersonId
		where w.WorkflowTypeId = 78
		and w.Status = 'Waiting for Review'

		select * from #AdminDashboard
		where Applicant like @LastName + '%'
		order by case when Status in ('Needs Review', 'HR BC Needs Review') or Requester = 'Batch Process' then 1 else 2 end, Status, [Date] desc

		drop table #AdminDashboard
		drop table #WaitingOnProtectionTeamWorkflows
		drop table #Workflows
		drop table #ProtectionAttributes
	end
	-- Standard User Dashboard
	else begin
		-- Get all ministry/campus combinations that this person has made requests for since we started the new workflows (April 2016)
		select distinct Ministry = M.Value, Campus = C.Value
		into #MinistryCampus
		from Workflow W
		join AttributeValue M on M.EntityId = W.ID and M.AttributeID = 2634 -- Ministry
		join AttributeValue C on C.EntityId = W.ID and C.AttributeID = 2633 -- Campus
		join AttributeValue R on R.EntityId = W.ID and R.AttributeID = 2631 -- Requester
		join PersonAlias PA on R.Value = PA.Guid
		where W.WorkflowTypeId = 52
		and W.Status = 'Completed'
		and PA.PersonID = @CurrentPersonId

		create table #Workflows2 ([Date] datetime, WorkflowTypeID int, WorkflowID int, WorkflowStatus varchar(100), RequesterPersonID int, RequesterFirstName varchar(100), RequesterLastName varchar(100)
			, ApplicantPersonID int, ApplicantFirstName varchar(100), ApplicantLastName varchar(100), ProtectionStatusGuid uniqueidentifier)

		-- Outstanding Protection Requests
		insert into #Workflows2
		select W.CreatedDateTime, W.WorkflowTypeID, W.ID, W.Status, ReqP.ID, ReqP.FirstName, ReqP.LastName, AppP.ID, AppP.FirstName, AppP.LastName, try_cast(ProtAV.Value as uniqueidentifier)
		from Workflow W
		join AttributeValue ReqAV on ReqAV.EntityId = W.ID and ReqAV.AttributeID in (2631)
		join PersonAlias ReqPA on try_cast(ReqAV.Value as uniqueidentifier) = ReqPA.Guid -- CONVERT(VARCHAR(36), ReqPA.Guid)
		join Person ReqP on ReqPA.PersonID = ReqP.ID
		join AttributeValue AppAV on AppAV.EntityId = W.ID and AppAV.AttributeID in (2632)
		join PersonAlias AppPA on try_cast(AppAV.Value as uniqueidentifier) = AppPA.Guid -- CONVERT(VARCHAR(36), AppPA.Guid)
		join Person AppP on AppPA.PersonID = AppP.ID
		join AttributeValue ProtAV on ProtAV.EntityId = AppPA.PersonId and ProtAV.AttributeId = 1731 
		left join AttributeValue M on M.EntityId = W.ID and M.AttributeID = 2634 -- Ministry (left join because full join performs poorly)
		left join AttributeValue C on C.EntityId = W.ID and C.AttributeID = 2633 -- Campus (left join because full join performs poorly)
		left join PersonAlias paini on paini.Id = w.InitiatorPersonAliasId
		where W.WorkflowTypeId = 52 
		and W.CreatedDateTime > dateadd(day, -45, getdate()) 
		and W.Status = 'Completed'
		and (ReqPA.PersonId = @CurrentPersonId 
			OR paini.PersonId = @CurrentPersonId
			OR exists (select * from #MinistryCampus mc where mc.Campus = C.Value and mc.Ministry = M.Value) 					
			);

		-- Outstanding Youth Covenant Requests
		insert into #Workflows2
		select W.CreatedDateTime, W.WorkflowTypeID, W.ID, W.Status, ReqP.ID, ReqP.FirstName, ReqP.LastName, AppP.ID, AppP.FirstName, AppP.LastName, try_cast(YouthAV.Value as uniqueidentifier)
		from Workflow W
		join AttributeValue ReqAV on ReqAV.EntityId = W.ID and ReqAV.AttributeID in (@RequesterAttributeID)
		join PersonAlias ReqPA on try_cast(ReqAV.Value as uniqueidentifier) = ReqPA.Guid -- CONVERT(VARCHAR(36), ReqPA.Guid)
		join Person ReqP on ReqPA.PersonID = ReqP.ID
		join AttributeValue AppAV on AppAV.EntityId = W.ID and AppAV.AttributeID in (@ApplicantAttributeID)
		join PersonAlias AppPA on try_cast(AppAV.Value as uniqueidentifier) = AppPA.Guid -- CONVERT(VARCHAR(36), AppPA.Guid)
		join Person AppP on AppPA.PersonID = AppP.ID
		join AttributeValue YouthAV on YouthAV.EntityId = AppPA.PersonId and YouthAV.AttributeId = 2395
		left join AttributeValue M on M.EntityId = W.ID and M.AttributeID = 13831 -- Ministry (left join because full join performs poorly)
		left join AttributeValue C on C.EntityId = W.ID and C.AttributeID = 13830 -- Campus (left join because full join performs poorly)
		left join PersonAlias paini on paini.Id = w.InitiatorPersonAliasId
		where W.WorkflowTypeId = @YouthCovenantRequestWorkflowTypeID
		and W.CreatedDateTime > dateadd(day, -45, getdate()) 
		and W.Status = 'Completed'
		and (ReqPA.PersonId = @CurrentPersonId 
			OR paini.PersonId = @CurrentPersonId
			OR exists (select * from #MinistryCampus mc where mc.Campus = C.Value and mc.Ministry = M.Value) 					
			);

		-- All protection attributes for each person in the query above
		with CTE as (
			select Value = case when ValueAsDateTime is not null then convert(varchar(10), ValueAsDateTime, 101) else Value end
				, PersonId = EntityId
				, AttributeKey = [Key]
			from Attribute A
			join AttributeValue V on V.AttributeId = A.ID and len(V.Value) > 0
			where EntityTypeId = 15
			and [Key] in ('ProtectionStatus', 'ProtectionApplicationDate', 'ProtectionApplication', 'BackgroundCheckDate', 'BackgroundCheckDocument', 'ProtectionReference1Date', 'ProtectionReference1', 
							'ProtectionReference2Date', 'ProtectionReference2', 'ProtectionReference3Date', 'ProtectionReference3', 'PolicyAcknowledgmentDate',
							'YouthCovenantStatus', 'YouthCovenantDate', 'YouthCovenant', 'YouthCovenantReference1Date', 'YouthCovenantReference1')
			and exists (select * from #Workflows2 W where W.ApplicantPersonID = V.EntityID)
			)
		select *
		into #ProtectionAttributes2
		from CTE 
		pivot(max(Value) for AttributeKey in (ProtectionStatus, ProtectionApplicationDate, ProtectionApplication, BackgroundCheckDate, BackgroundCheckDocument, ProtectionReference1Date, ProtectionReference1, 
												ProtectionReference2Date, ProtectionReference2, ProtectionReference3Date, ProtectionReference3, PolicyAcknowledgmentDate,
												YouthCovenantStatus, YouthCovenantDate, YouthCovenant, YouthCovenantReference1Date, YouthCovenantReference1)) P;

		select W.Date
			, Applicant = '<a href=''/Person/' + cast(ApplicantPersonID as varchar) + '/ExtendedAttributes'' target="_blank">' + ApplicantLastName + ', ' + ApplicantFirstName + '</a>'
			, Requester = RequesterLastName + ', ' + RequesterFirstName
			, Status = ProtDV.Value
			, [Background Check] = case when W.WorkflowTypeID = @YouthCovenantRequestWorkflowTypeID then '' 
										else isnull(pr.BackgroundCheckDate, case when PR.BackgroundCheckDocument is not null then 'Review' else '' end)
										end
			, [Reference 1] = case when W.WorkflowTypeID = @YouthCovenantRequestWorkflowTypeID then isnull(pr.YouthCovenantReference1Date, case when pr.YouthCovenantReference1 is not null then 'Review' else '' end)
										else isnull(pr.ProtectionReference1Date, case when PR.ProtectionReference1 is not null then 'Review' else '' end)
										end
			, [Reference 2] = case when W.WorkflowTypeID = @YouthCovenantRequestWorkflowTypeID then ''
										else isnull(pr.ProtectionReference2Date, case when PR.ProtectionReference2 is not null then 'Review' else '' end)
										end
			, [Reference 3] = case when W.WorkflowTypeID = @YouthCovenantRequestWorkflowTypeID then ''
										else isnull(pr.ProtectionReference3Date, case when PR.ProtectionReference3 is not null then 'Review' else '' end)
										end
			, Application = case when W.WorkflowTypeID = @YouthCovenantRequestWorkflowTypeID then isnull(pr.YouthCovenantDate, case when pr.YouthCovenant is not null then 'Review' else '' end)
										else isnull(pr.ProtectionApplicationDate, case when PR.ProtectionApplication is not null then 'Review' else '' end)
										end
			, Policy = case when W.WorkflowTypeID = @YouthCovenantRequestWorkflowTypeID then '' else isnull(PR.PolicyAcknowledgmentDate,'') end
		from #Workflows2 W
		join DefinedValue ProtDV on cast(ProtDV.[Guid] as nvarchar(max)) = ProtectionStatusGuid
		left join #ProtectionAttributes2 PR on PR.PersonId = W.ApplicantPersonID
		order by case when W.RequesterPersonID = @CurrentPersonId then 1 else 2 end, W.Date desc															

		drop table #MinistryCampus
		drop table #Workflows2
		drop table #ProtectionAttributes2
	end
end
go