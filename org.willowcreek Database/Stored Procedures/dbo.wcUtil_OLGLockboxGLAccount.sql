USE [Rock]
GO

/****** Object:  StoredProcedure [dbo].[wcUtil_OLGLockboxGLAccount]    Script Date: 10/3/2016 3:17:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


create procedure [dbo].[wcUtil_OLGLockboxGLAccount]
	@lockbox integer
as begin
	SELECT GLAccount = CONVERT(VARCHAR, CRGL) + ' ' + LEFT(CRProject, 3) + '-' + SUBSTRING(CRProject, 4, 3) + '-' + SUBSTRING(CRProject, 7, 3) + '-' + RIGHT(CRProject, 4)
	  FROM Anvil5.WCCC.dbo.Lockbox_Src
	  where active = 1
	  and lockbox = @lockbox
end

GO

