/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getProtectionDashboardAdv]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcWrkFlw_getProtectionDashboardAdv]
GO
/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getProtectionDashboardAdv]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[wcWrkFlw_getProtectionDashboardAdv]
AS
BEGIN

	select w.Id, w.[Status] WStatus, w.CreatedDateTime, w.CompletedDateTime, p.LastName + ', ' + p.FirstName CreatedBy, pApp.Id ApplicantId, pApp.LastName + ', ' + pApp.FirstName Applicant,
		dvPS.Value PStatus, avSt.Value Steps,
		dbo.wcfnUtil_getPersonAttributeValueByKey('BackgroundCheckDate',pApp.Id,'DATE',NULL) [B/C],
		dbo.wcfnUtil_getPersonAttributeValueByKey('ProtectionReference1Date',pApp.Id,'DATE',NULL)  [Ref 1],
		dbo.wcfnUtil_getPersonAttributeValueByKey('ProtectionReference2Date',pApp.Id,'DATE',NULL)  [Ref 2],
		dbo.wcfnUtil_getPersonAttributeValueByKey('ProtectionReference3Date',pApp.Id,'DATE',NULL)  [Ref 3],
		dbo.wcfnUtil_getPersonAttributeValueByKey('ProtectionApplicationDate',pApp.Id,'DATE',NULL) [App],
		dbo.wcfnUtil_getPersonAttributeValueByKey('PolicyAcknowledgmentDate',pApp.Id,'DATE',NULL) [Policy],
		avWABC.Value BCWrkFlwId, wBC.[Status] BCWStatus, wBC.CompletedDateTime BCWComplete, 
		avWARL.Value RLWrkFlwId, wRL.[Status] RLWStatus, wRL.CompletedDateTime RLWComplete, 
		avWAApp.Value AppWrkFlwId, wApp.[Status] AppWStatus, wApp.CompletedDateTime AppWComplete,
		avWAPA.Value PAWrkFlwId, wPA.[Status] PAWStatus, wPA.CompletedDateTime PAWComplete
	from workflow w
	--creator
	join PersonAlias pa on w.CreatedByPersonAliasId = pa.Id
	join Person p on pa.PersonId = p.Id
	--applicant
	join attributevalue avApp on avApp.AttributeId = 2632 and avApp.EntityId = w.Id
	join PersonAlias paApp on avApp.Value = paApp.[Guid]
	join Person pApp on paApp.PersonId = pApp.Id
	--protection status
	left join attributevalue avPS on avPS.AttributeId = 1731 and avPS.EntityId = pApp.Id
	left join DefinedValue dvPS ON avPS.Value = CAST(dvPS.[Guid] AS nvarchar(MAX))
	--steps
	join attributevalue avSt on avSt.AttributeId = 2646 and avSt.EntityId = w.Id
	--backgroundcheckworkflowid
	left join WorkflowActivity waBC on waBC.ActivityTypeId = 160 and w.Id = waBC.WorkflowId
	left join AttributeValue avWABC on avWABC.AttributeId = 2653 and waBC.Id = avWABC.EntityId
	--bc status
	left join workflow wBC on avWABC.Value = wBC.Id
	--referencelistworkflowid
	left join WorkflowActivity waRL on waRL.ActivityTypeId = 160 and w.Id = waRL.WorkflowId
	left join AttributeValue avWARL on avWARL.AttributeId = 2652 and waRL.Id = avWARL.EntityId
	--rl status
	left join workflow wRL on avWARL.Value = wRL.Id
	--applicationworkflowid
	left join WorkflowActivity waApp on waApp.ActivityTypeId = 160 and w.Id = waApp.WorkflowId
	left join AttributeValue avWAApp on avWAApp.AttributeId = 2651 and waApp.Id = avWAApp.EntityId
	--app status
	left join workflow wApp on avWAApp.Value = wApp.Id
	--policyacknowledgmentworkflowid
	left join WorkflowActivity waPA on waPA.ActivityTypeId = 160 and w.Id = waPA.WorkflowId
	left join AttributeValue avWAPA on avWAPA.AttributeId = 2654 and waPA.Id = avWAPA.EntityId
	--pa status
	left join workflow wPA on avWAPA.Value = wPA.Id
	--
	where w.CreatedDateTime > '04-11-2016'
	and w.WorkflowTypeId = 52
	order by w.CreatedDateTime Desc

end
GO
