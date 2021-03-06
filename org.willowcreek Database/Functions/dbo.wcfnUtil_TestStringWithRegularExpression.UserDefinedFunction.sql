/****** Object:  UserDefinedFunction [dbo].[wcfnUtil_TestStringWithRegularExpression]    Script Date: 6/13/2016 11:25:28 AM ******/
DROP FUNCTION [dbo].[wcfnUtil_TestStringWithRegularExpression]
GO
/****** Object:  UserDefinedFunction [dbo].[wcfnUtil_TestStringWithRegularExpression]    Script Date: 6/13/2016 11:25:28 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--select dbo.wcfnUtil_TestStringWithRegularExpression('pippo123will.org', 1, 'T', 'F')

/*

sp_configure 'show advanced options', 1;
go


reconfigure
go

sp_configure 'ole automation procedures', 1
go

reconfigure
go


*/


CREATE         function [dbo].[wcfnUtil_TestStringWithRegularExpression]
 (
  @stringaDaControllare varchar(4096),      -- string to check

  @tipoStringa smallint,                 	-- 1 = email
                                    		-- 2 = phone number Italian
                                    		-- 3 = phone number US
                                    		-- 4 = string of letters and numbers at least 8 characters

  @ricercaGlobale char(1),                	-- T = seeks all occurrences
                                    		-- F = seeks only the first occurrence

  @ignoreCaseSensitive char(1)            	-- T = search is case sensitive
                                    		-- F = Search without ignoring case sensitive									)
)  
returns char(1)                         	-- T = string to control ok
                                    		-- F = string to check incorrect
as
 begin
  -- control expressions depending on the type of string to test
  declare @espressioneDiControllo varchar(1048)
  set @espressioneDiControllo = ''
  if (@tipoStringa = 1)
   begin
    -- email
    -- non accetta abc_@abc.it
    -- set @espressioneDiControllo = '^([a-zA-Z0-9_\-])+(\.([a-zA-Z0-9_\-])+)*@((\[(((([0-1])?([0-9])?[0-9])|(2[0-4][0-9])|(2[0-5][0-5])))\.(((([0-1])?([0-9])?[0-9])|(2[0-4][0-9])|(2[0-5][0-5])))\.(((([0-1])?([0-9])?[0-9])|(2[0-4][0-9])|(2[0-5][0-5])))\.(((([0-1])?([0-9])?[0-9])|(2[0-4][0-9])|(2[0-5][0-5]))\]))|((([a-zA-Z0-9])+(([\-])+([a-zA-Z0-9])+)*\.)+([a-zA-Z])+(([\-])+([a-zA-Z0-9])+)*))$'    --'^(([A-Za-z0-9]+_+)|([A-Za-z0-9]+\-+)|([A-Za-z0-9]+\.+)|([A-Za-z0-9]+\++))*[A-Za-z0-9]+@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$'
    -- ==============================================================================================================================
    set @espressioneDiControllo = '^([a-zA-Z0-9_\-])+(\.([a-zA-Z0-9_\-])+)*@((\w+\-+)|(\w+\.))*\w{1,63}\.[a-zA-Z]{2,6}$'
   end
  if (@tipoStringa = 2)
   begin
    -- phone number Italian
    set @espressioneDiControllo = '^([0-9]*\-?\ ?\/?[0-9]*)$'
   end
  if (@tipoStringa = 3)
   begin
    -- phone number US
    set @espressioneDiControllo = '^(? ?<1>[(])?(?<AreaCode>[2-9]\d{2})(?(1)[)])(?(1)(?<2>[ ])|(? ?<3>[-])|(?<4>[ ])))?)?(?<Prefix>[1-9]\d{2})(?(AreaCode)(? ?(1)(?(2)[- ]|[-]?))|(?(3)[-])|(?(4)[- ]))|[- ]?)(?<Suffix>\d{4})$'
   end
  if (@tipoStringa = 4)
   begin
    -- string of letters and numbers at least 8 characters
    set @espressioneDiControllo = '(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{8,20})$'
   end
  if (@espressioneDiControllo <> '')
   begin
    -- performs control
    declare @hr integer
    declare @objRegExp integer
    declare @results bit
    declare @output char(1)
    
    -- com object creation for regular expressions
    exec @hr = sp_OACreate 'VBScript.RegExp', @objRegExp output
  
    if (@hr <> 0)
     begin
      set @results = 0
  
      goto USCITA
     end
     
    -- set the properties of the com
    -- Pattern over which to test
    exec @hr = sp_OAsetProperty @objRegExp, 'Pattern', @espressioneDiControllo
  
    if (@hr <> 0)
     begin
      set @results = 0
  
      goto USCITA
     end
  
    -- comprehensive monitoring or stops at the first occurrence
    if (@ricercaGlobale = 'T')
     begin
      exec @hr = sp_OAsetProperty @objRegExp, 'Global', True
     end
    else
     begin
      exec @hr = sp_OAsetProperty @objRegExp, 'Global', False
     end
  
    if (@hr <> 0)
     begin
      set @results = 0
  
      goto USCITA
     end
  
    -- ignores capital letters
    if (@ignoreCaseSensitive = 'T')
     begin
      exec @hr = sp_OAsetProperty @objRegExp, 'IgnoreCase', True
     end
    else
     begin
      exec @hr = sp_OAsetProperty @objRegExp, 'IgnoreCase', False
     end
  
    if (@hr <> 0)
     begin
      set @results = 0
  
      goto USCITA
     end
  
    -- It performs the control of the string by calling the Test
    exec @hr = sp_OAMethod @objRegExp, 'Test', @results output, @stringaDaControllare
  
    if (@hr <> 0)
     begin
      set @results = 0
  
      goto USCITA
     end
  
    -- destroys the object com
    exec @hr = sp_OADestroy @objRegExp
  
    if (@hr <> 0)
     begin
      set @results = 0
  
      goto USCITA
     end
   end
  else
   begin
    -- type not provided
    set @results = 0
    goto USCITA
   end
USCITA:
  if (@results = 1)
   begin
    set @output = 'T'
   end
  else
   begin
    set @output = 'F'
   end
  return @output
 end


GO
