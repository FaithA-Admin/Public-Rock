/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getProtectionArchive]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcWrkFlw_getProtectionArchive]
GO
/****** Object:  StoredProcedure [dbo].[wcWrkFlw_getProtectionArchive]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[wcWrkFlw_getProtectionArchive]
	@personId INT
AS
BEGIN

	SELECT CAST(paa.ArchiveDateTime AS DATE) [Archive Date], 
				dv.Value [Status], 
			    REPLACE(paa.BackgroundCheckDate,'1900-01-01','') BackgroundCheckDate, 
				REPLACE(paa.Reference1Date,'1900-01-01','') Reference1Date, 
				REPLACE(paa.Reference2Date,'1900-01-01','') Reference2Date, 
				REPLACE(paa.Reference3Date,'1900-01-01','') Reference3Date,
				REPLACE(paa.ApplicationDate,'1900-01-01','') ApplicationDate,
				REPLACE(paa.PolicyAcknowledgmentDate,'1900-01-01','') PolicyAcknowledgmentDate,
				'<a href=''/GetFile.ashx?guid='+ CAST(paa.BackgroundCheckBinaryFileId AS NVARCHAR(36)) +'''>view</a>' BackgroundCheck, 
				'<a href=''/GetFile.ashx?guid='+ CAST(paa.Reference1BinaryFileId AS NVARCHAR(36)) +'''>view</a>' Reference1, 
				'<a href=''/GetFile.ashx?guid='+ CAST(paa.Reference2BinaryFileId AS NVARCHAR(36)) +'''>view</a>' Reference2, 
				'<a href=''/GetFile.ashx?guid='+ CAST(paa.Reference3BinaryFileId AS NVARCHAR(36)) +'''>view</a>' Reference3, 
				'<a href=''/GetFile.ashx?guid='+ CAST(paa.ApplicationBinaryFileId AS NVARCHAR(36)) +'''>view</a>' [Application]
	FROM PersonAlias pa
	JOIN _org_willowcreek_ProtectionApp_Archive paa ON pa.[Guid] = paa.PersonAliasGuid
	LEFT JOIN DefinedValue dv ON paa.ProtectionStatus = dv.[Guid]
	WHERE pa.PersonId = @personId
	and  (paa.BackgroundCheckDate is not null 
			or paa.Reference1Date is not null 
			or paa.Reference2Date is not null
			or paa.Reference3Date is not null 
			or paa.ApplicationDate is not null
			or paa.PolicyAcknowledgmentDate is not null 
			or paa.BackgroundCheckBinaryFileId is not null
			or paa.Reference1BinaryFileId is not null 
			or paa.Reference2BinaryFileId is not null 
			or paa.Reference3BinaryFileId is not null
			or paa.ApplicationBinaryFileId is not null)
	ORDER BY paa.ArchiveDateTime DESC

END

GO
