/****** Object:  View [dbo].[wcView_WillowEventItemOccurrence]    Script Date: 6/13/2016 11:20:36 AM ******/
DROP VIEW [dbo].[wcView_WillowEventItemOccurrence]
GO
/****** Object:  View [dbo].[wcView_WillowEventItemOccurrence]    Script Date: 6/13/2016 11:20:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[wcView_WillowEventItemOccurrence]
AS
SELECT eo.Id
	  ,ei.Id AS EventItemId	
      ,ei.Name 
	  ,ei.Summary 
	  ,ei.Description
	  ,ei.DetailsUrl
	  ,c.Id AS CampusId 
      ,ISNULL(c.Name,'') AS CampusName
      ,eo.[Location]
      ,ISNULL(p.FirstName + ' ','')  + ISNULL(p.MiddleName + ' ','')  + ISNULL(p.LastName,'') ContactName
      ,eo.[ContactPhone]
      ,eo.[ContactEmail]
	  ,av1.Value + REPLACE(bf.Path,'~/','') AS Photo
      ,av2.Value + 'Registration?RegistrationInstanceId=' + CAST(eiogm.RegistrationInstanceId as varchar) + '&EventOccurrenceID='+ CAST(eo.id as varchar) AS RegistrationLinkURL
	  ,s.Id AS ScheduleId
	  ,s.iCalendarContent EventiCalendarContent
	  ,[dbo].[wcfniCal_GetDateTime](s.iCalendarContent, 'DTSTART') AS EventStartDateTime
      ,[dbo].[wcfniCal_GetDateTime](s.iCalendarContent, 'DTEND') AS EventEndDateTime
	  ,ri.Id AS RegistrationId
	  ,ri.StartDateTime as RegistrationStartDateTime
	  ,ri.EndDateTime as RegistrationEndDateTime	
	  ,eo.CreatedDateTime
	  ,eo.ModifiedDateTime
	  ,eo.CreatedByPersonAliasId
	  ,eo.ModifiedByPersonAliasId
	  ,eo.Guid
	  ,eo.ForeignId
	  ,eo.ForeignGuid
	  ,eo.ForeignKey	
  FROM EventItemOccurrence eo
  LEFT JOIN Campus c
	ON eo.CampusId = c.Id
  Left JOIN PersonAlias pa	
	ON pa.Id = eo.ContactPersonAliasId
  LEFT JOIN Person p
	ON p.id = pa.AliasPersonId
  LEFT JOIN Schedule s
	ON s.Id = eo.ScheduleId
  LEFT JOIN EventItem ei
	ON ei.Id = eo.EventItemId
  LEFT JOIN BinaryFile bf
	ON bf.Id = ei.PhotoId 
  LEFT JOIN AttributeValue av1
	ON av1.AttributeId = 815
  LEFT JOIN AttributeValue av2
	ON av2.AttributeId = 816
  LEFT JOIN EventItemOccurrenceGroupMap eiogm
	ON eiogm.EventItemOccurrenceId = eo.Id
  LEFT JOIN RegistrationInstance ri
	ON ri.Id = eiogm.RegistrationInstanceId
  JOIN EventCalendarItem eci
	ON eci.EventItemId = ei.Id
  WHERE ei.IsActive = 1 and ei.IsApproved = 1 and eci.EventCalendarId = 1  --Get only active and approved events in public calendar
		AND s.iCalendarContent IS NOT NULL


















GO
