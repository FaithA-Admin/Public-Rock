USE [Rock]
GO

/****** Object:  StoredProcedure [dbo].[wcUtil_OLGDecryptionKey]    Script Date: 10/3/2016 3:17:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[wcUtil_OLGDecryptionKey]
	@id integer
as begin
	select TheKey1 from Anvil5.WCCC.dbo.Lockbox_OLG where ID = @id 
end

GO

