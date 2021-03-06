/****** Object:  StoredProcedure [dbo].[wcUtil_saveToOrgChart]    Script Date: 6/13/2016 11:22:16 AM ******/
DROP PROCEDURE [dbo].[wcUtil_saveToOrgChart]
GO
/****** Object:  StoredProcedure [dbo].[wcUtil_saveToOrgChart]    Script Date: 6/13/2016 11:22:16 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE procedure [dbo].[wcUtil_saveToOrgChart]
	@c varchar(100),
	@m varchar(100)
as
begin

declare @pgId INT
declare @cId INT
select @pgId = Id, @cId = CampusId from [group] where [name] = @c and grouptypeid = 33
--select * from [group] where ParentGroupId = @pgId

declare @now datetime = getdate()

insert into [group]
(IsSystem, ParentGroupId, GroupTypeId, CampusId, Name, [Description], IsSecurityRole, IsActive, [Order], [Guid], CreatedDateTime, ModifiedDateTime, CreatedByPersonAliasId, ModifiedByPersonAliasId)
values
(0,@pgId,33,@cId,@m,'',0,1,0,NEWID(),@now,@now,1,1)

end
GO
