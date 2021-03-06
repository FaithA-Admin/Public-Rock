--USE [RockDev_v4_SC_3]
--GO
/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getLegacyEntityXRef]    Script Date: 2/9/2016 9:57:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[wcWrkFlw_getLegacyEntityXRef]
	@personId INT
AS
BEGIN

	SELECT CASE lexr.SourceSystemCode
				WHEN 'A' THEN 'Arena'
				WHEN 'C' THEN 'Chronicle'
				WHEN 'Pro' THEN 'Proclaim'
				ELSE lexr.SourceSystemCode
		   END Source,
		   CASE lexr.SourceSystemCode
				WHEN 'A' THEN '<a href=''https://arena.willowcreek.org/default.aspx?page=7&person=' + lexr.SourceID + ''' target=''_blank''>'+lexr.SourceID+'</a>'
				WHEN 'C' THEN '<a href=''https://kairos.willowcreek.org/Kairos/Person/PersonInfo.aspx?PersonID=' + lexr.SourceID + ''' target=''_blank''>'+lexr.SourceID+'</a>'
				WHEN 'Pro' THEN '<a href=''https://proclaim.willowcreek.org/main.aspx?etc=2&extraqs=%3f_gridType%3d2%26etc%3d2%26id%3d%257b' + lexr.SourceID + '%257d%26pagemode%3diframe%26preloadcache%3d1441130125093%26rskey%3d74555559&pagetype=entityrecord#' + ''' target=''_blank''>'+lexr.SourceID+'</a>'

				ELSE lexr.SourceID
		   END Link
	FROM Person p
	JOIN PersonAlias pa ON p.Id = pa.PersonId
	JOIN _org_willowcreek_LegacyEntityXRef lexr ON pa.Guid = lexr.PersonAliasGuid
	WHERE p.Id = @personId

END
