USE [Rock]
GO

/****** Object:  StoredProcedure [dbo].[wcUtil_ReassignPushPayLink]    Script Date: 1/30/2017 3:52:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[wcUtil_ReassignPushPayLink] 
	@OldPersonID INT,
	@NewPersonID INT
AS BEGIN
	update PP
	set PersonAliasID = (select ID from PersonAlias where AliasPersonID = @NewPersonID)
	from _com_pushPay_RockRMS_Payer PP
	join PersonAlias PA on PP.PersonAliasID = PA.ID
	join Person P on PA.PersonID = P.ID
	where P.ID = @OldPersonID
END
GO


