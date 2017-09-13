USE [Rock]
--GO

/****** Object:  StoredProcedure [dbo].[wcRpt_ContributorsByYear]    Script Date: 2/1/2017 3:06:54 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

create procedure [dbo].[wcRpt_ContributorsByYear]
	@Year INT
	, @GivingID VARCHAR(MAX) = null
	, @IncludePreviousAddress BIT = 0
as begin
	-- Get all contributors that have a mailing address
	select PersonID, GivingID, TransactionID = C.ID, TransactionDateTime, TransactionCode, CurrencyType = DV.Value, SourceType = DV2.Value, Amount = SUM(D.Amount), A.Description, CurrencyTypeValueID = ISNULL(C.CurrencyTypeValueId, 0)
			, Desc2 = case when C.CurrencyTypeValueId = 1136 then NonCashDesc.Value
						when C.CurrencyTypeValueId = 1135 then NumShares.Value + ' shares of ' + SecurityName.Value + ' on ' + format(Received.ValueAsDateTime, 'M/d/yyyy')
						else null end
			, DetailID = D.ID
	into #Contributions
	from wcView_Contributions C
	join FinancialPaymentDetail P on C.FinancialPaymentDetailId = P.ID
	left join DefinedValue DV on DV.ID = P.CurrencyTypeValueId
	left join DefinedValue DV2 on DV2.ID = SourceTypeValueId
	join FinancialTransactionDetail D on D.TransactionId = C.ID
	join Person PR on PR.ID = C.PersonID
	left join FinancialAccount A on A.ID = AccountId
	left join AttributeValue NumShares on NumShares.AttributeID = (select ID from Attribute where entitytypeid = 338 and [Key] = 'NumberofShares') and NumShares.EntityID = C.FinancialPaymentDetailId
	left join AttributeValue SecurityName on SecurityName.AttributeID = (select ID from Attribute where entitytypeid = 338 and [Key] = 'SecurityName') and SecurityName.EntityID = C.FinancialPaymentDetailId
	left join AttributeValue Received on Received.AttributeID = (select ID from Attribute where entitytypeid = 338 and [Key] = 'DateReceived') and Received.EntityID = C.FinancialPaymentDetailId
	left join AttributeValue NonCashDesc on NonCashDesc.AttributeID = (select ID from Attribute where entitytypeid = 338 and [Key] = 'NonCashDescription') and NonCashDesc.EntityID = C.FinancialPaymentDetailId
	where C.TransactionDateTime >= convert(varchar, @Year) + '-01-01' 
	and C.TransactionDateTime < convert(varchar, @Year+1) + '-01-01'
	and (@GivingID is null or PR.GivingID = @GivingID)
	group by PersonID, GivingID, C.ID, TransactionDateTime, TransactionCode, DV.Value, DV2.Value, A.Description, C.CurrencyTypeValueId, NumShares.Value, SecurityName.Value, Received.ValueAsDateTime, NonCashDesc.Value, D.ID;

	-- Calculate the correct number of transactions expected and confirm that they match what we received above
	declare @CorrectCount INT
	declare @CorrectTotal FLOAT
	select @CorrectCount = count(*), @CorrectTotal = sum(amount)
	from wcView_Contributions C 
	left join FinancialPaymentDetail P on C.FinancialPaymentDetailId = P.ID
	left join FinancialTransactionDetail D on D.TransactionId = C.ID
	left join Person PR on C.PersonID = PR.ID
	where TransactionTypeValueId = 53 and TransactionDateTime >= convert(varchar, @Year) + '-01-01' and TransactionDateTime < convert(varchar, @Year+1) + '-01-01'
	and (@GivingID is null or PR.GivingID = @GivingID)

	if (select count(*) from #Contributions) != @CorrectCount or (select sum(amount) from #Contributions) != @CorrectTotal BEGIN
		raiserror (N'The transaction count or total appears to be incorrect.', 10, 1);
	END
	else begin
		declare @firstPageLines int = 45
		declare @subsequentPageLines int = 66;

		with GivingIDs as (
			select GivingID
				, Transactions = COUNT(distinct DetailID)
				, TotalGifts = SUM(Amount)
				, CashGifts = SUM(case when CurrencyTypeValueID not in (1135, 1136) then Amount else 0 end)
				, EstimatedLines = COUNT(distinct DetailID) + SUM(case when CurrencyTypeValueID = 1135 then 2 else 0 end) + SUM(case when CurrencyTypeValueID = 1136 then 1 else 0 end) + 2 -- add 2 for footer
			from #Contributions
			group by GivingID)
		select *, Pages = case when EstimatedLines <= @firstPageLines then 1
							else ((EstimatedLines - @firstPageLines - 1) / @subsequentPageLines) + 2
							end
		into #GivingIDs 
		from GivingIDs
	
		select distinct G.GivingID, P.GivingGroupId, P.ID, P.FirstName, P.LastName, FM.GroupRoleId
			, Gender = case when Gender = 0 then 999 else Gender end -- Sort them as Male, Female, Unknown
			, BirthDate = ISNULL(BirthDate, GETDATE())
			, Deceased = CASE WHEN P.RecordStatusReasonValueId = 167 then 1 else 0 end
		into #Donors
		from #GivingIDs G
		join Person P on P.GivingID = G.GivingID
		join GroupMember FM on FM.PersonID = P.ID
		join [Group] F on FM.GroupID = F.ID and F.GroupTypeId = 10;

		-- Remove the names of any deceased people in groups where at least one person is not deceased
		delete D 
		from #Donors D
		where exists (select * from #Donors D0 where Deceased = 0 and D0.GivingID = D.GivingID)
		and exists (select * from #Donors D1 where Deceased = 1 and D1.GivingID = D.GivingID)
		and Deceased = 1

		select distinct G.GivingID
			, Name = case when (select count(distinct ltrim(rtrim(LastName))) from #Donors where GivingID = G.GivingID) = 1 -- All share a last name
								then ltrim(rtrim(dbo.ReplaceLastOccurrence(SUBSTRING((select ', ' + ltrim(rtrim(FirstName)) from #Donors where GivingID = G.GivingID order by GroupRoleId, Gender, BirthDate, ID for xml path ('')), 3, 99999), ', ', ' & ') -- First Names
									 + ' ' 
									 + (select top 1 ltrim(rtrim(LastName)) from #Donors where GivingID = G.GivingID))) -- Last Name
								else ltrim(rtrim(dbo.ReplaceLastOccurrence(substring((select ', ' + ltrim(rtrim(FirstName)) + ' ' + ltrim(rtrim(LastName)) from #Donors where GivingID = G.GivingID order by GroupRoleId, Gender, BirthDate, ID for xml path ('')), 3, 99999), ', ', ' & ')))
								end
		into #Names
		from #Donors G;
	
		-- Create a table that contains the address types that are allowed. Usually this is only Home and Work but we allow requesting Previous as well
		select GroupLocationTypeValueID = 19
		into #AllowedMailingAddressTypes

		insert into #AllowedMailingAddressTypes values (20)

		if @IncludePreviousAddress = 1 begin
			insert into #AllowedMailingAddressTypes values (137)
		end;

		with GroupLocations as (
				-- Get all the mailing locations for these givers
				-- If the person is giving alone, return all related families' mailing addresses
				select PersonID = C.ID, FL.LocationId, IsMappedLocation, FL.GroupLocationTypeValueId
				from #Donors C
				join GroupMember GM on C.ID = GM.PersonID
				join [Group] F on F.ID = GM.GroupID and F.GroupTypeId = 10
				join GroupLocation FL on FL.GroupID = F.ID -- and FL.IsMailingLocation = 1  -- Ignore IsMailingLocation because it is not reliable
				join #AllowedMailingAddressTypes MA on FL.GroupLocationTypeValueId = MA.GroupLocationTypeValueID
				where C.GivingGroupID is null
				union all
				-- If the person is giving with a family, return all that family's mailing addresses
				select C.ID, GL.LocationID, IsMappedLocation, GL.GroupLocationTypeValueId
				from #Donors C
				join GroupMember GM on C.GivingGroupId = GM.GroupID
				join GroupLocation GL on GL.GroupID = GM.GroupId --and GL.IsMailingLocation = 1 -- Ignore IsMailingLocation because it is not reliable
				join #AllowedMailingAddressTypes MA on GL.GroupLocationTypeValueId = MA.GroupLocationTypeValueID
				join Location L on L.ID = GL.LocationId
				),
			MailingLocations as (
				-- Rank the mailing locations for each giver
				select *, row = row_number() over (
					partition by PersonID 
					order by case when GroupLocationTypeValueID = 19 then 0 else 1 end -- Home first
						, case when GroupLocationTypeValueID = 20 then 0 else 1 end -- Work next (in case previous addresses are allowed)
						, IsMappedLocation desc -- Use Mapped if there is more than one of the same type
						, LocationID desc -- Most recent of a given type takes precedence
						) 
				from GroupLocations 
				),
			MailingAddresses as (
				-- Get the best mailing location for each giver
				select PersonID, Street1, Street2, City, State, Country, PostalCode, StandardizeAttemptedDateTime, StandardizedDateTime, LocationID = L.ID
				from MailingLocations ML
				join Location L on L.ID = ML.LocationId
				where row = 1
				),
			FormattedMailingAddresses as (
				-- Format the mailing address
				select ID, GivingGroupID, GivingID
					, Street1, Street2, City, State, Country, PostalCode
					, StandardizeAttemptedDateTime, StandardizedDateTime
					, LocationID
				from #Donors C
				join MailingAddresses MA on MA.PersonID = C.ID
				)
		select distinct GivingID, Street1, Street2 = CASE WHEN LEN(LTRIM(RTRIM(Street2))) > 0 THEN Street2 ELSE NULL END, City, State, Country = CASE WHEN Country IS NULL or Country = 'US' then '' else ISNULL(DV.Description, Country) end, PostalCode
			, StandardizeAttemptedDateTime, StandardizedDateTime, LocationID
		into #Addresses
		from FormattedMailingAddresses MAP
		left join DefinedValue DV on MAP.Country = DV.Value and DV.DefinedTypeID = 45

		select GivingID
		into #DoNotMail
		from AttributeValue AV
		join Person P on AV.EntityID = P.ID
		where AV.AttributeID = 12889 -- Do Not Mail
		and Value = 'Contribution Statement'

		select PersonID = (select top 1 id from person where givingid = N.GivingID), N.GivingID, N.Name, Street1, Street2, City, State, Country, PostalCode, Transactions, TotalGifts, CashGifts, Pages, EstimatedLines
			, TotalGiftsLine = Case when CashGifts > 0 then 'TOTAL CASH CONTRIBUTIONS: ' + format(CashGifts, 'C', 'en-us') else 'NO CASH CONTRIBUTIONS RECEIVED' end
			, LocationID
		into #Results
		from #Names N
		join #GivingIDs G on G.GivingID = N.GivingID
		left join #Addresses A on N.GivingID = A.GivingID
		where ((TotalGifts > 0 and street1 is not null) or @GivingID is not null)
		and not exists (select * from #DoNotMail D where G.GivingID = D.GivingID)
		-- Test members
		--and (N.GivingID in (select distinct givingid from person where id in (41119, 33635, 20853, 7900, 73662, 50857, 29418, 31949, 28500, 29136, 34822, 12435, 18706, 39545, 47081, 36686, 15144, 97533, 7604))
		--or N.GivingID = @GivingID)

		select * from #Results 
		order by case when Country != '' then 0 else 1 end
			, Pages desc
			, PostalCode
			, GivingID

			select GivingID
				, TransactionId
				, TransactionDateTime
				, CheckDesc = case CurrencyType 
								when 'Check' then TransactionCode 
								when 'ACH' then 'Online'
								when 'Credit Card' then 
									case when TransactionCode like '%brick%' then TransactionCode
									when SourceType = 'Kiosk' then SourceType
									else 'Online'
									end
								else CurrencyType 
								end
				, Amount = case when CurrencyTypeValueId in (1135, 1136) then '' else format(Amount, 'C', 'en-us') end
				, Description, Desc2
				, Row = row_number() over (partition by C.GivingID order by TransactionDateTime, TransactionID)
				, Even = case when row_number() over (partition by C.GivingID order by TransactionDateTime, TransactionID) % 2 = 1 then 'Even' else null end
			from #Contributions C
			where exists (select * from #Results R where R.GivingID = C.GivingID)
			order by C.GivingID, TransactionDateTime, TransactionId
	end
end

GO


