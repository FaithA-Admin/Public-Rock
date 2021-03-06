/****** Object:  StoredProcedure [dbo].[wcWrkFlw_saveProtectionToArchive]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcWrkFlw_saveProtectionToArchive]
GO
/****** Object:  StoredProcedure [dbo].[wcWrkFlw_saveProtectionToArchive]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[wcWrkFlw_saveProtectionToArchive]
	@aliasPersonGuid VARCHAR(36),
	@bc VARCHAR(3),
	@ref VARCHAR(3),
	@app VARCHAR(3),
	@policy VARCHAR(3)
AS
BEGIN

	DECLARE @result INT = 0

	IF LEN(@aliasPersonGuid) > 0
	BEGIN

		DECLARE @now DATETIME = GETDATE()
		DECLARE @personId INT = (SELECT PersonId FROM PersonAlias WHERE [Guid] = @aliasPersonGuid)

		--RETRIEVE

		DECLARE @t VARCHAR(MAX)	--temporary bug fix

		DECLARE @protStatus UNIQUEIDENTIFIER
		DECLARE @protStatusId INT
		SELECT @t = av.Value,
			   @protStatusId = av.Id
		FROM PersonAlias pa
		JOIN AttributeValue av ON pa.PersonId = av.EntityId
		JOIN Attribute a ON av.AttributeId = a.Id
						AND a.[Key] = 'ProtectionStatus'
		WHERE pa.[Guid] = @aliasPersonGuid

		IF LEN(@t) > 0
			SET @protStatus = CAST(@t AS UNIQUEIDENTIFIER)


		DECLARE @bcDate DATE
		DECLARE @bcId INT
		SELECT @bcDate = CAST(av.Value AS DATE),
			   @bcId = av.Id
		FROM PersonAlias pa
		JOIN AttributeValue av ON pa.PersonId = av.EntityId
		JOIN Attribute a ON av.AttributeId = a.Id
						AND a.[Key] = 'BackgroundCheckDate'
		WHERE pa.[Guid] = @aliasPersonGuid

		DECLARE @bcResult NVARCHAR(25)
		DECLARE @bcResultId INT
		SELECT @bcResult = av.Value,
			   @bcResultId = av.Id
		FROM PersonAlias pa
		JOIN AttributeValue av ON pa.PersonId = av.EntityId
		JOIN Attribute a ON av.AttributeId = a.Id
						AND a.[Key] = 'BackgroundCheckResult'
		WHERE pa.[Guid] = @aliasPersonGuid

		DECLARE @bcDoc UNIQUEIDENTIFIER
		DECLARE @bcDocId INT
		SELECT @bcDoc = CAST(av.Value AS UNIQUEIDENTIFIER),
			   @bcDocId = av.Id
		FROM PersonAlias pa
		JOIN AttributeValue av ON pa.PersonId = av.EntityId
		JOIN Attribute a ON av.AttributeId = a.Id
						AND a.[Key] = 'BackgroundCheckDocument'
		WHERE pa.[Guid] = @aliasPersonGuid

		DECLARE @ref1Date DATE
		DECLARE @ref1Id INT
		SELECT @ref1Date = CAST(av.Value AS DATE),
			   @ref1Id = av.Id
		FROM PersonAlias pa
		JOIN AttributeValue av ON pa.PersonId = av.EntityId
		JOIN Attribute a ON av.AttributeId = a.Id
						AND a.[Key] = 'ProtectionReference1Date'
		WHERE pa.[Guid] = @aliasPersonGuid

		DECLARE @ref2Date DATE
		DECLARE @ref2Id INT
		SELECT @ref2Date = CAST(av.Value AS DATE),
			   @ref2Id = av.Id
		FROM PersonAlias pa
		JOIN AttributeValue av ON pa.PersonId = av.EntityId
		JOIN Attribute a ON av.AttributeId = a.Id
						AND a.[Key] = 'ProtectionReference2Date'
		WHERE pa.[Guid] = @aliasPersonGuid

		DECLARE @ref3Date DATE
		DECLARE @ref3Id INT
		SELECT @ref3Date = CAST(av.Value AS DATE),
			   @ref3Id = av.Id
		FROM PersonAlias pa
		JOIN AttributeValue av ON pa.PersonId = av.EntityId
		JOIN Attribute a ON av.AttributeId = a.Id
						AND a.[Key] = 'ProtectionReference3Date'
		WHERE pa.[Guid] = @aliasPersonGuid

				DECLARE @ref1Doc UNIQUEIDENTIFIER
		DECLARE @ref1DocId INT
		SELECT @ref1Doc = CAST(av.Value AS UNIQUEIDENTIFIER),
			   @ref1DocId = av.Id
		FROM PersonAlias pa
		JOIN AttributeValue av ON pa.PersonId = av.EntityId
		JOIN Attribute a ON av.AttributeId = a.Id
						AND a.[Key] = 'ProtectionReference1'
		WHERE pa.[Guid] = @aliasPersonGuid

		DECLARE @ref2Doc UNIQUEIDENTIFIER
		DECLARE @ref2DocId INT
		SELECT @ref2Doc = CAST(av.Value AS UNIQUEIDENTIFIER),
			   @ref2DocId = av.Id
		FROM PersonAlias pa
		JOIN AttributeValue av ON pa.PersonId = av.EntityId
		JOIN Attribute a ON av.AttributeId = a.Id
						AND a.[Key] = 'ProtectionReference2'
		WHERE pa.[Guid] = @aliasPersonGuid

		DECLARE @ref3Doc UNIQUEIDENTIFIER
		DECLARE @ref3DocId INT
		SELECT @ref3Doc = CAST(av.Value AS UNIQUEIDENTIFIER),
			   @ref3DocId = av.Id
		FROM PersonAlias pa
		JOIN AttributeValue av ON pa.PersonId = av.EntityId
		JOIN Attribute a ON av.AttributeId = a.Id
						AND a.[Key] = 'ProtectionReference3'
		WHERE pa.[Guid] = @aliasPersonGuid

		DECLARE @appDate DATE
		DECLARE @appId INT
		SELECT @appDate = CAST(av.Value AS DATE),
			   @appId = av.Id
		FROM PersonAlias pa
		JOIN AttributeValue av ON pa.PersonId = av.EntityId
		JOIN Attribute a ON av.AttributeId = a.Id
						AND a.[Key] = 'ProtectionApplicationDate'
		WHERE pa.[Guid] = @aliasPersonGuid

		DECLARE @appDoc UNIQUEIDENTIFIER
		DECLARE @appDocId INT
		SELECT @appDoc = CAST(av.Value AS UNIQUEIDENTIFIER),
			   @appDocId = av.Id
		FROM PersonAlias pa
		JOIN AttributeValue av ON pa.PersonId = av.EntityId
		JOIN Attribute a ON av.AttributeId = a.Id
						AND a.[Key] = 'ProtectionApplication'
		WHERE pa.[Guid] = @aliasPersonGuid

		DECLARE @policyDate DATE
		DECLARE @policyId INT
		SELECT @policyDate = CAST(av.Value AS DATE),
			   @policyId = av.Id
		FROM PersonAlias pa
		JOIN AttributeValue av ON pa.PersonId = av.EntityId
		JOIN Attribute a ON av.AttributeId = a.Id
						AND a.[Key] = 'PolicyAcknowledgmentDate'
		WHERE pa.[Guid] = @aliasPersonGuid

		--ARCHIVE

		DECLARE @archiveGuid UNIQUEIDENTIFIER = NEWID()
		INSERT INTO _org_willowcreek_ProtectionApp_Archive
		([Guid], PersonAliasGuid, ArchiveDateTime, ProtectionStatus, 
		 BackgroundCheckDate, Reference1Date, Reference2Date, Reference3Date, ApplicationDate, PolicyAcknowledgmentDate,
		 BackgroundCheckBinaryFileId, BackgroundCheckResult, Reference1BinaryFileId, Reference2BinaryFileId, Reference3BinaryFileId, ApplicationBinaryFileId)
		VALUES
		(@archiveGuid,@aliasPersonGuid,@now,@protStatus,
		 @bcDate,@ref1Date,@ref2Date,@ref3Date,@appDate,@policyDate,
		 @bcDoc,@bcResult,@ref1Doc,@ref2Doc,@ref3Doc,@appDoc)

		--DELETE & BinaryFileTypeId

		DECLARE @archiveTypeId INT = (SELECT Id FROM BinaryFileType WHERE [Guid] = '1609EB30-5234-4BAA-9B8F-A8966839029D') --protection archive

		IF @bc = 'Yes'
		BEGIN
			DELETE AttributeValue WHERE Id IN (@bcId,@bcResultId,@bcDocId)
			UPDATE BinaryFile SET BinaryFileTypeId = @archiveTypeId WHERE [Guid] = @bcDoc
			IF NOT EXISTS(SELECT NULL FROM _org_willowcreek_PersonDocument WHERE PersonAliasGuid = @aliasPersonGuid AND BinaryFileId = @bcDoc)
			BEGIN
				IF @bcDoc IS NOT NULL
					INSERT INTO _org_willowcreek_PersonDocument
					(PersonAliasGuid,BinaryFileId,CreatedDateTime,ModifiedDateTime,CreatedByPersonAliasId,ModifiedByPersonAliasId,[Guid])
					VALUES
					(@aliasPersonGuid,@bcDoc,@now,@now,1,1,NEWID())
			END
		END

		IF @ref = 'Yes'
		BEGIN
			DELETE AttributeValue WHERE Id IN (@ref1Id,@ref1DocId,@ref2Id,@ref2DocId,@ref3Id,@ref3DocId)
			UPDATE BinaryFile SET BinaryFileTypeId = @archiveTypeId WHERE [Guid] IN (@ref1Doc,@ref2Doc,@ref3Doc)
			IF NOT EXISTS(SELECT NULL FROM _org_willowcreek_PersonDocument WHERE PersonAliasGuid = @aliasPersonGuid AND BinaryFileId IN (@ref1Doc,@ref2Doc,@ref3Doc))
			BEGIN
				IF @ref1Doc IS NOT NULL
					INSERT INTO _org_willowcreek_PersonDocument
					(PersonAliasGuid,BinaryFileId,CreatedDateTime,ModifiedDateTime,CreatedByPersonAliasId,ModifiedByPersonAliasId,[Guid])
					VALUES
					(@aliasPersonGuid,@ref1Doc,@now,@now,1,1,NEWID())
				IF @ref2Doc IS NOT NULL
					INSERT INTO _org_willowcreek_PersonDocument
					(PersonAliasGuid,BinaryFileId,CreatedDateTime,ModifiedDateTime,CreatedByPersonAliasId,ModifiedByPersonAliasId,[Guid])
					VALUES
					(@aliasPersonGuid,@ref2Doc,@now,@now,1,1,NEWID())
				IF @ref3Doc IS NOT NULL
					INSERT INTO _org_willowcreek_PersonDocument
					(PersonAliasGuid,BinaryFileId,CreatedDateTime,ModifiedDateTime,CreatedByPersonAliasId,ModifiedByPersonAliasId,[Guid])
					VALUES
					(@aliasPersonGuid,@ref3Doc,@now,@now,1,1,NEWID())
			END
		END

		IF @app = 'Yes'
		BEGIN
			DELETE AttributeValue WHERE Id IN (@appId,@appDocId)
			UPDATE BinaryFile SET BinaryFileTypeId = @archiveTypeId WHERE [Guid] = @appDoc
			IF NOT EXISTS(SELECT NULL FROM _org_willowcreek_PersonDocument WHERE PersonAliasGuid = @aliasPersonGuid AND BinaryFileId = @appDoc)
			BEGIN
				IF @appDoc IS NOT NULL
					INSERT INTO _org_willowcreek_PersonDocument
					(PersonAliasGuid,BinaryFileId,CreatedDateTime,ModifiedDateTime,CreatedByPersonAliasId,ModifiedByPersonAliasId,[Guid])
					VALUES
					(@aliasPersonGuid,@appDoc,@now,@now,1,1,NEWID())
			END
		END

		IF @policy = 'Yes'
		BEGIN
			DELETE AttributeValue WHERE Id IN (@policyId)
		END

		--RESET

		IF @protStatusId IS NULL
		BEGIN
			DECLARE @attrId INT = (SELECT Id FROM Attribute WHERE [Key] = 'ProtectionStatus')
			INSERT INTO AttributeValue
			(IsSystem, AttributeId, EntityId, [Value], [Guid], CreatedDateTime, ModifiedDateTime, CreatedByPersonAliasId, ModifiedByPersonAliasId)
			VALUES
			(0, @attrId, @personId, 'E4A8815E-6300-45D0-A07F-068698A39638', NEWID(), @now, @now, 1, 1)
		END
		ELSE
		BEGIN
			UPDATE AttributeValue
			SET [Value] = 'E4A8815E-6300-45D0-A07F-068698A39638' --In Progress
			WHERE Id = @protStatusId
		END

		SET @result = 1

	END

	SELECT @result

END

GO
