/****** Object:  UserDefinedFunction [dbo].[wcfnUtil_StripHTML]    Script Date: 6/13/2016 11:25:28 AM ******/
DROP FUNCTION [dbo].[wcfnUtil_StripHTML]
GO
/****** Object:  UserDefinedFunction [dbo].[wcfnUtil_StripHTML]    Script Date: 6/13/2016 11:25:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[wcfnUtil_StripHTML] (@HTMLText VARCHAR(MAX))
RETURNS VARCHAR(MAX) AS
BEGIN
    DECLARE @Start INT
    DECLARE @End INT
    DECLARE @Length INT
    SET @Start = CHARINDEX('<',@HTMLText)
    SET @End = CHARINDEX('>',@HTMLText,CHARINDEX('<',@HTMLText))
    SET @Length = (@End - @Start) + 1
    WHILE @Start > 0 AND @End > 0 AND @Length > 0
    BEGIN
        SET @HTMLText = STUFF(@HTMLText,@Start,@Length,'')
        SET @Start = CHARINDEX('<',@HTMLText)
        SET @End = CHARINDEX('>',@HTMLText,CHARINDEX('<',@HTMLText))
        SET @Length = (@End - @Start) + 1
    END
    RETURN LTRIM(RTRIM(@HTMLText))
END

GO
