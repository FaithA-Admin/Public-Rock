SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
-- [wcRpt_PledgeStatusLetter] 20, '3/31/2017', 1, 'P25863'
alter procedure [dbo].[wcRpt_PledgeStatusLetter] 
	@AccountID int
	, @EndDate datetime
	, @IncludeContributorsWithNoPledge bit
	, @GivingID VARCHAR(MAX) = null
	, @IncludePreviousAddress BIT = 0
as begin

	declare @Title varchar(max)
	declare @PledgeLinkRoleId int

	select @Title = AV.Value
	from AttributeValue AV
	join Attribute A on AV.AttributeId = A.ID
	where A.[Key] = 'CampaignName'
	and A.EntityTypeId = 76 -- Financial Account
	and AV.EntityId = @AccountID

	select @PledgeLinkRoleId = GTR.ID
	from AttributeValue AV
	join Attribute A on AV.AttributeId = A.ID
	join GroupTypeRole GTR on CONVERT(VARCHAR(36), GTR.Guid) = AV.Value
	where A.[Key] = 'PledgeLinkRole'
	and A.EntityTypeId = 76 -- Financial Account
	and AV.EntityId = @AccountID

	-- Get the Pledge Link relationships where the gifts of one person or business should be counted toward someone else's pledge
	select PersonID = PR.ID, PledgePersonID = PO.ID, PledgeGivingID = PO.GivingId
	into #Linked
	from Person PR
	join GroupMember GM on PR.ID = GM.PersonId
	join GroupMember GM2 on GM.GroupID = GM2.GroupId and GM.PersonID != GM2.PersonID
	join Person PO on GM2.PersonID = PO.ID
	where GM.GroupRoleId = @PledgeLinkRoleId
	and GM2.GroupRoleId = 5 -- Group Owner

	select P.GivingID, Pledge = SUM(FP.TotalAmount)
	into #Pledges
	from FinancialPledge FP
	join PersonAlias PA on FP.PersonAliasID = PA.ID
	join Person P on PA.PersonID = P.Id
	where FP.AccountID = @AccountID
	and (@GivingID is null or P.GivingID = @GivingID) -- Don't include linked relationships when looking for pledges
	group by P.GivingID
	having SUM(FP.TotalAmount) > 0

	declare @MinYear int
	select @MinYear = YEAR(MIN(FP.StartDate)) from FinancialPledge FP where FP.AccountID = @AccountID

	-- Get all contributors that have a mailing address
	select PersonID = ISNULL(PledgePersonID, PA.PersonID)
			, GivingID = ISNULL(PledgeGivingID, PR.GivingID)
			, TransactionID = T.ID, TransactionDateTime, TransactionCode, CurrencyType = DV.Value, SourceType = DV2.Value, Amount = SUM(D.Amount), A.Description, CurrencyTypeValueID = ISNULL(P.CurrencyTypeValueId, 0)
			, Desc2 = case when P.CurrencyTypeValueId = 1136 then NonCashDesc.Value
						when P.CurrencyTypeValueId = 1135 then NumShares.Value + ' shares of ' + SecurityName.Value + ' on ' + format(Received.ValueAsDateTime, 'M/d/yyyy')
						else null end
			, DetailID = D.ID
	into #Contributions
	from FinancialTransaction T -- Do not use wcView_Contributions because pledges do not take conduits into account
	join PersonAlias PA on T.AuthorizedPersonAliasId = PA.ID
	left join #Linked L on PA.PersonID = L.PersonID
	join Person PR on PR.ID = PA.PersonID
	join FinancialPaymentDetail P on T.FinancialPaymentDetailId = P.ID
	left join DefinedValue DV on DV.ID = P.CurrencyTypeValueId
	left join DefinedValue DV2 on DV2.ID = SourceTypeValueId
	join FinancialTransactionDetail D on D.TransactionId = T.ID
	join FinancialAccount A on A.ID = AccountId
	left join AttributeValue NumShares on NumShares.AttributeID = (select ID from Attribute where entitytypeid = 338 and [Key] = 'NumberofShares') and NumShares.EntityID = T.FinancialPaymentDetailId
	left join AttributeValue SecurityName on SecurityName.AttributeID = (select ID from Attribute where entitytypeid = 338 and [Key] = 'SecurityName') and SecurityName.EntityID = T.FinancialPaymentDetailId
	left join AttributeValue Received on Received.AttributeID = (select ID from Attribute where entitytypeid = 338 and [Key] = 'DateReceived') and Received.EntityID = T.FinancialPaymentDetailId
	left join AttributeValue NonCashDesc on NonCashDesc.AttributeID = (select ID from Attribute where entitytypeid = 338 and [Key] = 'NonCashDescription') and NonCashDesc.EntityID = T.FinancialPaymentDetailId
	where T.TransactionTypeValueId = 53
	and A.ParentAccountId = @AccountID
	and T.TransactionDateTime >= '1/1/' + CONVERT(VARCHAR(4), @MinYear)
	and T.TransactionDateTime < DATEADD(day, 1, convert(date, @EndDate))
	and (@GivingID is null or ISNULL(PledgeGivingID, PR.GivingID) = @GivingID)
	group by ISNULL(PledgePersonID, PA.PersonID), ISNULL(PledgeGivingID, PR.GivingID), T.ID, TransactionDateTime, TransactionCode, DV.Value, DV2.Value, A.Description, P.CurrencyTypeValueId, NumShares.Value, SecurityName.Value, Received.ValueAsDateTime, NonCashDesc.Value, D.ID;

	-- Calculate the correct number of transactions expected and confirm that they match what we received above
	declare @SelectedCount INT, @SelectedTotal DECIMAL(20,2), @CorrectCount INT, @CorrectTotal DECIMAL(20, 2)

	select T.ID, Amount
	into #CorrectAmounts
	from FinancialTransaction T 
	join PersonAlias PA on T.AuthorizedPersonAliasId = PA.ID
	left join FinancialPaymentDetail P on T.FinancialPaymentDetailId = P.ID
	left join FinancialTransactionDetail D on D.TransactionId = T.ID
	left join Person PR on PA.PersonID = PR.ID
	left join FinancialAccount A on A.ID = AccountId
	left join #Linked L on PA.PersonID = L.PersonID
	where TransactionTypeValueId = 53 
	and A.ParentAccountId = @AccountID
	and T.TransactionDateTime >= '1/1/' + CONVERT(VARCHAR(4), @MinYear)
	and T.TransactionDateTime < DATEADD(day, 1, convert(date, @EndDate))
	and (@GivingID is null or ISNULL(PledgeGivingID, PR.GivingID) = @GivingID)

	select @SelectedCount = count(*), @SelectedTotal = sum(Amount) from #Contributions
	select @CorrectCount = count(*), @CorrectTotal = sum(Amount) from #CorrectAmounts

	if @SelectedCount != @CorrectCount or @SelectedTotal != @CorrectTotal BEGIN
		raiserror (N'The transaction count or total appears to be incorrect.', 10, 1);

		select SelectedCount = @SelectedCount, CorrectCount = @CorrectCount, SelectedTotal = @SelectedTotal, CorrectTotal = @CorrectTotal

		select * 
		from #Contributions C
		full outer join #CorrectAmounts CA on C.TransactionID = CA.ID
		where C.TransactionID is null or CA.ID is null

	END
	else begin
		if @IncludeContributorsWithNoPledge = 0 and @GivingID = null begin
			delete C
			from #Contributions C
			where not exists (select * from #Pledges P where P.GivingId = C.GivingID)
		end

		declare @firstPageLines int = 38 - ((year(getdate()) - @MinYear - 1) * 2)
		declare @subsequentPageLines int = 66;

		with GivingIDs as (
			select GivingID
				, Transactions = COUNT(distinct DetailID)
				, TotalGifts = SUM(Amount)
				, CashGifts = SUM(case when CurrencyTypeValueID not in (1135, 1136) then Amount else 0 end)
				, EstimatedLines = COUNT(distinct DetailID) + SUM(case when CurrencyTypeValueID = 1135 then 2 else 0 end) + SUM(case when CurrencyTypeValueID = 1136 then 1 else 0 end) + 2 -- add 2 for footer
				, FirstContribution = MIN(TransactionDateTime)
			from #Contributions
			group by GivingID)
		select *, Pages = case when EstimatedLines <= @firstPageLines then 1
							else ((EstimatedLines - @firstPageLines - 1) / @subsequentPageLines) + 2
							end
		into #GivingIDs 
		from GivingIDs

		-- Insert records for people who had pledges but no contributions
		insert into #GivingIDs
		select GivingID, 0, 0, 0, 0, 1, 1
		from #Pledges P
		where not exists (select * from #GivingIDs G where G.GivingID = P.GivingID)

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
			, Pledge = format(isnull(P.Pledge, 0), 'C', 'en-us')
			, Remaining = format(ABS(isnull(P.Pledge, 0) - TotalGifts), 'C', 'en-us')
			, Received = format(TotalGifts, 'C', 'en-us')
			, RemainingMessage = case when isnull(P.Pledge, 0) >= TotalGifts
									then 'Your remaining ' + @Title + ' commitment'
									else 'You exceeded ' + @Title + ' commitment by'
									end
			, AdditionalMessage = case when isnull(P.Pledge, 0) < TotalGifts
									then 'Thank you for these additional gifts; they are greatly appreciated.'
									when isnull(P.Pledge, 0) = TotalGifts
									then 'Thank you for fulfilling your commitment.'
									end
			, G.FirstContribution
		into #Results
		from #Names N
		join #GivingIDs G on G.GivingID = N.GivingID
		left join #Addresses A on N.GivingID = A.GivingID
		left join #Pledges P on N.givingID = P.GivingId
		where (street1 is not null or @GivingID is not null)
		and not exists (select * from #DoNotMail D where G.GivingID = D.GivingID)

		select *, MinYear = @MinYear
		from #Results 
		order by case when Country != '' then 0 else 1 end
			, Pages desc
			, Transactions desc
			, GivingID

		-- Create a table of years to display in the header

		declare @Years int
		set @Years = YEAR(GETDATE()) - @MinYear + 1

		create table #Years (Year INT)

		while (select count(*) from #Years) < @Years
		begin
			declare @Y int
			select @Y = COUNT(*) + @MinYear from #Years
			insert into #Years values (@Y)
		end

		-- Header lines for each letter
		; with ContributionsByYear as (
			select GivingID, Year = YEAR(TransactionDateTime), Amount = SUM(Amount)
			from #Contributions
			group by GivingID, YEAR(TransactionDateTime)
			)
		select R.GivingID, Y.Year,  Amount = case when ISNULL(C.Amount, 0) = 0 then '' else format(C.Amount, 'C', 'en-us') end
		from #Results R
		cross join #Years Y
		left join ContributionsByYear C on C.Year = Y.Year and C.GivingId = R.GivingID
		order by R.GivingID, Y.Year
		
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
		order by C.GivingID, TransactionDateTime, TransactionId desc
	end
end

GO


