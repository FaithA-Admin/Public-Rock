/****** Object:  StoredProcedure [dbo].[wcWrkFlw_saveLeadershipInterview]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcWrkFlw_saveLeadershipInterview]
GO
/****** Object:  StoredProcedure [dbo].[wcWrkFlw_saveLeadershipInterview]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Terry Rugg
-- Create date: 7/7/15
-- Description:	Saves initial state of Leadership
--              application to LeadershipInterview
--              table.
--10/7/2015		TH		Column name changes
-- =============================================
CREATE PROCEDURE [dbo].[wcWrkFlw_saveLeadershipInterview]  
	-- Add the parameters for the stored procedure here
	@applicantGuid VARCHAR(50),
	@applicantName VARCHAR(100),
	@sponsorGuid VARCHAR(50),
	@sponsorName VARCHAR(100),
	@initiatorGuid VARCHAR(50),
	@initiatorName VARCHAR(100),
	@campus VARCHAR(100),
	@ministry VARCHAR(100),
	@leadershipPosition VARCHAR(100),
	@surrenderedLife VARCHAR(15),
	@relationalHealth VARCHAR(15),
	@representsBeliefs VARCHAR(15),
	@aboveReproach VARCHAR(15),
	@ministryFit VARCHAR(15),
	@qualifies VARCHAR(15),
	@additionalComments  VARCHAR(2000),
	@signedCovenant VARCHAR(15),
	@applicationStatus VARCHAR(30),
	@initiatorId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

DECLARE @now DATETIME
SET @now = GETDATE()

BEGIN TRAN

	INSERT INTO [dbo].[_org_willowcreek_LeadershipInterview]
	(
		   [ApplicantPersonAliasGuid]
		  ,[ApplicantName]
		  ,[SponsorPersonAliasGuid]
		  ,[SponsorName]
		  ,[InitiatorPersonAliasGuid]
		  ,[InitiatorName]
		  ,[InterviewDate]
		  ,[Campus]
		  ,[Ministry]
		  ,[LeadershipPosition]
		  ,[SpirituallySurrendered]
		  ,[RelationalHealth]
		  ,[ValuesBeliefs]
		  ,[AboveReproach]
		  ,[MinistryFit]
		  ,[QualifiedForPosition]
		  ,[AdditionalComments]
		  ,[SignedCovenant]
		  ,[LeadershipStatus]
		  ,[Guid]
		  ,[CreatedByPersonAliasId]
		  ,[CreatedDateTime]
		  ,[ModifiedByPersonAliasId]
		  ,[ModifiedDateTime]
	)
	VALUES (
		   CAST(@applicantGuid AS uniqueidentifier)
		  ,@applicantName
		  ,CAST(@sponsorGuid AS uniqueidentifier)
		  ,@sponsorName
		  ,CAST(@initiatorGuid AS uniqueidentifier)
		  ,@initiatorName
		  ,@now
		  ,@campus
		  ,@ministry
		  ,@leadershipPosition
		  ,@surrenderedLife
		  ,@relationalHealth
		  ,@representsBeliefs
		  ,@aboveReproach
		  ,@ministryFit
		  ,@qualifies
		  ,@additionalComments
		  ,@signedCovenant
		  ,@applicationStatus
		  ,NEWID()
		  ,@initiatorId
		  ,@now
		  ,@initiatorId
		  ,@now
	)

	SELECT SCOPE_IDENTITY()

COMMIT

END



GO
