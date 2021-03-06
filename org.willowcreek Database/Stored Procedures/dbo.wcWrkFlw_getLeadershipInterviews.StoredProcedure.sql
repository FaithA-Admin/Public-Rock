/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getLeadershipInterviews]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcWrkFlw_getLeadershipInterviews]
GO
/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getLeadershipInterviews]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Terry Rugg
-- Create date: 5/12/15
-- Description:	Gets Leadership Interview data
--10/7/2015		TH		Updated column names
--12/16/2015	Tom Helms	Updated for GUID
-- =============================================
CREATE PROCEDURE [dbo].[wcWrkFlw_getLeadershipInterviews]

	@personID INT 

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT CAST(REPLACE(InterviewDate,'1900-01-01','') AS DATE) InterviewDate,
	Campus,
	Ministry, 
	SponsorName,
	LeadershipStatus as 'Interview Status',
	SignedCovenant,
	lq.Id AS 'Interview ID'
	FROM _org_willowcreek_LeadershipInterview lq
	JOIN PersonAlias pa on @personId = pa.PersonId
	WHERE lq.ApplicantPersonAliasGuid = pa.[Guid]
	ORDER BY lq.Id DESC

END




GO
