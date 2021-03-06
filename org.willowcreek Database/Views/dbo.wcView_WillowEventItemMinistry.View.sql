/****** Object:  View [dbo].[wcView_WillowEventItemMinistry]    Script Date: 6/13/2016 11:20:36 AM ******/
DROP VIEW [dbo].[wcView_WillowEventItemMinistry]
GO
/****** Object:  View [dbo].[wcView_WillowEventItemMinistry]    Script Date: 6/13/2016 11:20:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[wcView_WillowEventItemMinistry] AS

SELECT  ea.EventItemId
	    ,dv.Value AS Ministry
	    ,ea.Id
	    ,ea.CreatedDateTime
		,ea.ModifiedDateTime
		,ea.CreatedByPersonAliasId
		,ea.ModifiedByPersonAliasId
		,ea.Guid
		,ea.ForeignId
		,ea.ForeignGuid
		,ea.ForeignKey	
FROM DefinedValue dv
JOIN EventItemAudience ea
	ON ea.DefinedValueId = dv.Id


GO
