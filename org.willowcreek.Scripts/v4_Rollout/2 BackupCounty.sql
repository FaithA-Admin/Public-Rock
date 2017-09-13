--USE [RockDemo]
--GO

/****** Object:  Table [dbo].[Location_BAK]    Script Date: 3/14/2016 10:35:23 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Location_BAK](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ParentLocationId] [int] NULL,
	[Name] [nvarchar](100) NULL,
	[IsActive] [bit] NOT NULL,
	[LocationTypeValueId] [int] NULL,
	[GeoPoint] [geography] NULL,
	[GeoFence] [geography] NULL,
	[Street1] [nvarchar](100) NULL,
	[Street2] [nvarchar](100) NULL,
	[City] [nvarchar](50) NULL,
	[State] [nvarchar](50) NULL,
	[Country] [nvarchar](50) NULL,
	[AssessorParcelId] [nvarchar](50) NULL,
	[StandardizeAttemptedDateTime] [datetime] NULL,
	[StandardizeAttemptedServiceType] [nvarchar](50) NULL,
	[StandardizeAttemptedResult] [nvarchar](50) NULL,
	[StandardizedDateTime] [datetime] NULL,
	[GeocodeAttemptedDateTime] [datetime] NULL,
	[GeocodeAttemptedServiceType] [nvarchar](50) NULL,
	[GeocodeAttemptedResult] [nvarchar](50) NULL,
	[GeocodedDateTime] [datetime] NULL,
	[PrinterDeviceId] [int] NULL,
	[Guid] [uniqueidentifier] NOT NULL,
	[CreatedDateTime] [datetime] NULL,
	[ModifiedDateTime] [datetime] NULL,
	[CreatedByPersonAliasId] [int] NULL,
	[ModifiedByPersonAliasId] [int] NULL,
	[IsGeoPointLocked] [bit] NULL,
	[ForeignId] [nvarchar](50) NULL,
	[ImageId] [int] NULL,
	[PostalCode] [nvarchar](50) NULL,
	[County] [nvarchar](50) NULL,
 CONSTRAINT [PK_dbo.Location_BAK] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[Location_BAK]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Location_BAK_dbo.BinaryFile_ImageId] FOREIGN KEY([ImageId])
REFERENCES [dbo].[BinaryFile] ([Id])
GO

ALTER TABLE [dbo].[Location_BAK] CHECK CONSTRAINT [FK_dbo.Location_BAK_dbo.BinaryFile_ImageId]
GO

ALTER TABLE [dbo].[Location_BAK]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Location_BAK_dbo.DefinedValue_LocationTypeValueId] FOREIGN KEY([LocationTypeValueId])
REFERENCES [dbo].[DefinedValue] ([Id])
GO

ALTER TABLE [dbo].[Location_BAK] CHECK CONSTRAINT [FK_dbo.Location_BAK_dbo.DefinedValue_LocationTypeValueId]
GO

ALTER TABLE [dbo].[Location_BAK]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Location_BAK_dbo.Device_PrinterDeviceId] FOREIGN KEY([PrinterDeviceId])
REFERENCES [dbo].[Device] ([Id])
GO

ALTER TABLE [dbo].[Location_BAK] CHECK CONSTRAINT [FK_dbo.Location_BAK_dbo.Device_PrinterDeviceId]
GO

ALTER TABLE [dbo].[Location_BAK]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Location_BAK_dbo.Location_ParentLocationId] FOREIGN KEY([ParentLocationId])
REFERENCES [dbo].[Location_BAK] ([Id])
GO

ALTER TABLE [dbo].[Location_BAK] CHECK CONSTRAINT [FK_dbo.Location_BAK_dbo.Location_ParentLocationId]
GO

ALTER TABLE [dbo].[Location_BAK]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Location_BAK_dbo.PersonAlias_CreatedByPersonAliasId] FOREIGN KEY([CreatedByPersonAliasId])
REFERENCES [dbo].[PersonAlias] ([Id])
GO

ALTER TABLE [dbo].[Location_BAK] CHECK CONSTRAINT [FK_dbo.Location_BAK_dbo.PersonAlias_CreatedByPersonAliasId]
GO

ALTER TABLE [dbo].[Location_BAK]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Location_BAK_dbo.PersonAlias_ModifiedByPersonAliasId] FOREIGN KEY([ModifiedByPersonAliasId])
REFERENCES [dbo].[PersonAlias] ([Id])
GO

ALTER TABLE [dbo].[Location_BAK] CHECK CONSTRAINT [FK_dbo.Location_BAK_dbo.PersonAlias_ModifiedByPersonAliasId]
GO


SET IDENTITY_INSERT dbo.Location_BAK ON
GO

INSERT INTO [dbo].[Location_BAK] 
([Id],	[ParentLocationId],	[Name] ,	[IsActive] ,	[LocationTypeValueId] ,	[GeoPoint] ,	[GeoFence] ,	[Street1] ,	[Street2] ,	[City] ,
	[State],	[Country],	[AssessorParcelId] ,	[StandardizeAttemptedDateTime] ,	[StandardizeAttemptedServiceType],	[StandardizeAttemptedResult] ,
	[StandardizedDateTime] ,	[GeocodeAttemptedDateTime] ,	[GeocodeAttemptedServiceType] ,	[GeocodeAttemptedResult] ,	[GeocodedDateTime],
	[PrinterDeviceId] ,	[Guid] ,	[CreatedDateTime] ,	[ModifiedDateTime] ,	[CreatedByPersonAliasId] ,	[ModifiedByPersonAliasId] ,	[IsGeoPointLocked],
	[ForeignId],	[ImageId],	[PostalCode],	[County])
SELECT 
[Id],	[ParentLocationId],	[Name] ,	[IsActive] ,	[LocationTypeValueId] ,	[GeoPoint] ,	[GeoFence] ,	[Street1] ,	[Street2] ,	[City] ,
	[State],	[Country],	[AssessorParcelId] ,	[StandardizeAttemptedDateTime] ,	[StandardizeAttemptedServiceType],	[StandardizeAttemptedResult] ,
	[StandardizedDateTime] ,	[GeocodeAttemptedDateTime] ,	[GeocodeAttemptedServiceType] ,	[GeocodeAttemptedResult] ,	[GeocodedDateTime],
	[PrinterDeviceId] ,	[Guid] ,	[CreatedDateTime] ,	[ModifiedDateTime] ,	[CreatedByPersonAliasId] ,	[ModifiedByPersonAliasId] ,	[IsGeoPointLocked],
	[ForeignId],	[ImageId],	[PostalCode],	[County]
FROM [dbo].[Location]

SET IDENTITY_INSERT dbo.Location_BAK OFF
GO

ALTER TABLE dbo.[Location]
DROP COLUMN [County]
GO
