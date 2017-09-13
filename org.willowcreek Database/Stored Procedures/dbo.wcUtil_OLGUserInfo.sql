USE [Rock]
GO

/****** Object:  StoredProcedure [dbo].[wcUtil_OLGUserInfo]    Script Date: 10/3/2016 3:17:54 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


create procedure [dbo].[wcUtil_OLGUserInfo]
	@BankInfoID integer
as begin

	select U.FirstName
		, U.NickName
		, U.MiddleName
		, U.LastName
		, U.Address1
		, U.Address2
		, U.City
		, U.State
		, U.PostalCode
		, U.CountryCode
		, U.PhoneNumber
		, U.Extension
		, U.EmailAddress
		, U.Birthday
		, U.GenderCode
		, U.MaritalCode
	from anvil5.OLG.dbo.OLG_BankInfo B
	join anvil5.olg_login.dbo.login_profiles U on B.UserID = U.UserID
	where B.BankInfoID = @BankInfoID
end

GO

