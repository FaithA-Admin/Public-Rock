ALTER FUNCTION [dbo].[UDF_GetVehicleStockNumber] (@Id int,@Status int)
    RETURNS nvarchar(25)
    AS
    BEGIN
    declare @stock nvarchar(25);
    if(@Status=0)
    begin
    set @stock='T'+ Convert(varchar,@Id);
    end
    else
    begin
    set @stock='P'+ Convert(varchar,@Id);
    end
    return @stock;
    END