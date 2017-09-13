--select top 100 * from ExceptionLog order by id desc

--delete from ExceptionLog where id < 1113

select top 100 a.*
from dbo.Location_BAK a
join dbo.Location b on a.id = b.id
where a.County is not null or a.County > ''

update b
set b.county = a.County
from dbo.Location_BAK a
join dbo.Location b on a.id = b.id
where a.County is not null or a.County > ''

--select top 1000 * from location order by id desc