--USING [RockDev_v4_SC_3]
--GO

--Add ApplicantPersonAliasGuid
ALTER TABLE [_org_willowcreek_ProtectionApp_Reference]
ADD [ApplicantPersonAliasGuid] UNIQUEIDENTIFIER NULL
GO

--Update existing records to include ApplicantPersonAliasGuid
UPDATE [_org_willowcreek_ProtectionApp_Reference]
SET ApplicantPersonAliasGuid = q.ApplicantPersonAliasGuid
FROM [dbo].[_org_willowcreek_ProtectionApp_Questionnaire] q
JOIN [_org_willowcreek_ProtectionApp_Reference] r ON r.[QuestionnaireId] = q.Id
GO

--Update column to not null (Won't work because some questionnaires missing - possibly delete all references that are still null?)
--ALTER TABLE [_org_willowcreek_ProtectionApp_Reference]
--ALTER COLUMN [ApplicantPersonAliasGuid] UNIQUEIDENTIFIER NOT NULL
--GO

--Add Email column
ALTER TABLE [_org_willowcreek_ProtectionApp_Reference]
ADD [Email] VARCHAR(100) NULL
GO

--Update existing records to include ApplicantPersonAliasGuid
UPDATE [_org_willowcreek_ProtectionApp_Reference]
SET Email = p.Email
FROM [dbo].[_org_willowcreek_ProtectionApp_Reference] r
JOIN PersonAlias pa ON pa.[Guid] = r.ReferencePersonAliasGuid
JOIN Person p ON p.[Guid] = pa.AliasPersonGuid
GO

--Add NatureOfRelationshipApplicant 
ALTER TABLE [_org_willowcreek_ProtectionApp_Reference]
ADD [NatureOfRelationshipApplicant] VARCHAR(100) NULL
GO

--Update existing records to include ApplicantPersonAliasGuid
--Ref 1
UPDATE [_org_willowcreek_ProtectionApp_Reference]
SET NatureOfRelationshipApplicant = q.Reference1NatureOfAssociation
FROM [dbo].[_org_willowcreek_ProtectionApp_Questionnaire] q
JOIN [_org_willowcreek_ProtectionApp_Reference] r ON r.[QuestionnaireId] = q.Id AND r.RefNumber = 1
GO
--Ref 2
UPDATE [_org_willowcreek_ProtectionApp_Reference]
SET NatureOfRelationshipApplicant = q.Reference2NatureOfAssociation
FROM [dbo].[_org_willowcreek_ProtectionApp_Questionnaire] q
JOIN [_org_willowcreek_ProtectionApp_Reference] r ON r.[QuestionnaireId] = q.Id AND r.RefNumber = 2
GO
--Ref 3
UPDATE [_org_willowcreek_ProtectionApp_Reference]
SET NatureOfRelationshipApplicant = q.Reference3NatureOfAssociation
FROM [dbo].[_org_willowcreek_ProtectionApp_Questionnaire] q
JOIN [_org_willowcreek_ProtectionApp_Reference] r ON r.[QuestionnaireId] = q.Id AND r.RefNumber = 3
GO

--Update Recommend column to int from bit
ALTER TABLE [_org_willowcreek_ProtectionApp_Reference]
ALTER COLUMN [WouldRecommend] INT NULL
GO


