DROP VIEW [dbo].[wcView_Contributions]
GO
CREATE view [dbo].[wcView_Contributions] as
	select PA.PersonID, T.*, PD.AccountNumberMasked, PD.CurrencyTypeValueId, PD.CreditCardTypeValueId, Conduit = 'No'
	from FinancialTransaction T
	join PersonAlias PA on T.AuthorizedPersonAliasId = PA.ID
	join FinancialPaymentDetail PD on T.FinancialPaymentDetailId = PD.ID
	left join AttributeValue AV on AV.AttributeId = (select ID from Attribute A where A.[Key] = 'ConduitNon-TaxExempt') and AV.EntityId = PD.ID
	where T.TransactionTypeValueId = 53
	and (AV.Value is null or LEN(AV.Value) = 0)
	union all
	select PA.PersonID, T.*, PD.AccountNumberMasked, PD.CurrencyTypeValueId, PD.CreditCardTypeValueId, Conduit = 'Yes'
	from FinancialTransaction T
	join FinancialPaymentDetail PD on T.FinancialPaymentDetailId = PD.ID
	join AttributeValue AV on AV.AttributeId = (select ID from Attribute A where A.[Key] = 'ConduitNon-TaxExempt') and AV.EntityId = PD.ID
	join PersonAlias PA on AV.Value = PA.ID
	where T.TransactionTypeValueId = 53 

GO


