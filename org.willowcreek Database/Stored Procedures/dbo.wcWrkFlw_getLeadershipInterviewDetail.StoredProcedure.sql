/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getLeadershipInterviewDetail]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcWrkFlw_getLeadershipInterviewDetail]
GO
/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getLeadershipInterviewDetail]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Terry Rugg
-- Create date: 5/12/15
-- Description:	Gets Leadership Interview detail, 
--     returning more data for Protection Director
--     group members, less for anyone else.
-- =============================================
CREATE PROCEDURE [dbo].[wcWrkFlw_getLeadershipInterviewDetail]

(
	@currentPersonId INT,
	@lqid INT
)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @currentPersonIsProtDir VARCHAR(5)

	SET @currentPersonIsProtDir = 'False'

	SELECT @currentPersonIsProtDir = 'True'
	FROM PersonAlias pa
	JOIN GroupMember gm ON pa.AliasPersonId = gm.PersonId
	JOIN [Group] g ON gm.GroupId = g.Id
	AND g.[Guid] = '084F144C-D239-4878-A2CF-4567DEBD1939'--Director
	WHERE pa.PersonId = @currentPersonId

	IF @currentPersonIsProtDir = 'True'

		SELECT CAST(REPLACE([InterviewDate],'1900-01-01','') AS DATE) [InterviewDate]
		  ,[ApplicantName]
		  ,[ApplicantBirthDate]
		  ,[SponsorName]
		  ,[SponsorEmail]
		  ,[InitiatorName]
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
		  ,CAST(REPLACE([CreatedDateTime],'1900-01-01','') AS DATE) [CreatedDateTime]
		FROM _org_willowcreek_LeadershipInterview lq
		WHERE @lqid = lq.Id


	ELSE

		SELECT CAST(REPLACE([InterviewDate],'1900-01-01','') AS DATE) [InterviewDate]
		  ,[ApplicantName]
		  ,[SponsorName]
		  ,[InitiatorName]
		  ,[Campus]
		  ,[Ministry]
		  ,[LeadershipStatus]
		  ,CAST(REPLACE([CreatedDateTime],'1900-01-01','') AS DATE) [CreatedDateTime]
		FROM _org_willowcreek_LeadershipInterview lq
		WHERE @lqid = lq.Id


END




GO
