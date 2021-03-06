ALTER TABLE [dbo].[_org_willowcreek_CampusMinistryPerson]
DROP COLUMN [ForeignId]
GO

ALTER TABLE [dbo].[_org_willowcreek_CampusMinistryPerson]
ADD [ForeignKey] [nvarchar](100) NULL,
	[ForeignGuid] [uniqueidentifier] NULL,
	[ForeignId] [int] NULL
GO

ALTER TABLE [dbo].[_org_willowcreek_LeadershipInterview]
DROP COLUMN	[ForeignId]
GO

ALTER TABLE [dbo].[_org_willowcreek_LeadershipInterview]
ADD [ForeignKey] [nvarchar](100) NULL,
	[ForeignGuid] [uniqueidentifier] NULL,
	[ForeignId] [int] NULL
GO

ALTER TABLE [dbo].[_org_willowcreek_LegacyEntityXRef]
DROP COLUMN	[ForeignId]
GO

ALTER TABLE [dbo].[_org_willowcreek_LegacyEntityXRef]
ADD [ForeignKey] [nvarchar](100) NULL,
	[ForeignGuid] [uniqueidentifier] NULL,
	[ForeignId] [int] NULL
GO

ALTER TABLE [dbo].[_org_willowcreek_MinistryDefaults]
DROP COLUMN	[ForeignId]
GO

ALTER TABLE [dbo].[_org_willowcreek_MinistryDefaults]
ADD [ForeignKey] [nvarchar](100) NULL,
	[ForeignGuid] [uniqueidentifier] NULL,
	[ForeignId] [int] NULL
GO

ALTER TABLE [dbo].[_org_willowcreek_PersonDocument]
DROP COLUMN	[ForeignId]
GO

ALTER TABLE [dbo].[_org_willowcreek_PersonDocument]
ADD [ForeignKey] [nvarchar](100) NULL,
	[ForeignGuid] [uniqueidentifier] NULL,
	[ForeignId] [int] NULL
GO

ALTER TABLE [dbo].[_org_willowcreek_ProtectionApp_Archive]
DROP COLUMN	[ForeignId]
GO

ALTER TABLE [dbo].[_org_willowcreek_ProtectionApp_Archive]
ADD [ForeignKey] [nvarchar](100) NULL,
	[ForeignGuid] [uniqueidentifier] NULL,
	[ForeignId] [int] NULL
GO

ALTER TABLE [dbo].[_org_willowcreek_ProtectionApp_Questionnaire]
DROP COLUMN	[ForeignId]
GO

ALTER TABLE [dbo].[_org_willowcreek_ProtectionApp_Questionnaire]
ADD [ForeignKey] [nvarchar](100) NULL,
	[ForeignGuid] [uniqueidentifier] NULL,
	[ForeignId] [int] NULL
GO

ALTER TABLE [dbo].[_org_willowcreek_ProtectionApp_Queue]
DROP COLUMN	[ForeignId]
GO

ALTER TABLE [dbo].[_org_willowcreek_ProtectionApp_Queue]
ADD [ForeignKey] [nvarchar](100) NULL,
	[ForeignGuid] [uniqueidentifier] NULL,
	[ForeignId] [int] NULL
GO

ALTER TABLE [dbo].[_org_willowcreek_ProtectionApp_Reference]
DROP COLUMN	[ForeignId]
GO

ALTER TABLE [dbo].[_org_willowcreek_ProtectionApp_Reference]
ADD [ForeignKey] [nvarchar](100) NULL,
	[ForeignGuid] [uniqueidentifier] NULL,
	[ForeignId] [int] NULL
GO

ALTER TABLE [dbo].[_org_willowcreek_UserDefaults]
DROP COLUMN	[ForeignId]
GO

ALTER TABLE [dbo].[_org_willowcreek_UserDefaults]
ADD [ForeignKey] [nvarchar](100) NULL,
	[ForeignGuid] [uniqueidentifier] NULL,
	[ForeignId] [int] NULL
GO





