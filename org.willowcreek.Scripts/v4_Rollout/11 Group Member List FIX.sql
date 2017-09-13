select * from [page] where id = 113
select * from PageContext where PageId = 113
select * from PageRoute where PageId = 113
select * from Block where PageId = 113

select * from Attribute where EntityTypeId = 9
/*select top 100 * from AttributeValue where modifiedbypersonaliasid = 8658 order by ModifiedDateTime desc

select * from PersonAlias where id = 8658
select * from person where id = 26159

select * from Attribute where id in (3329,
3328,
3326)*/

select * 
--delete
from AttributeValue
where attributeid = 3326
and modifiedbypersonaliasid = 8658 --change to logged in user alias id