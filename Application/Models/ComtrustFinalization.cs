namespace TMS_Traning_Management.Models
{
	public class ComtrustFinalizationRequest
	{
		public Finalization Finalization { get; set; }
	}
	public class Finalization
	{
		public string TransactionID { get; set; }
		public string Customer { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
	}
	public class CurrencyConversion
	{
		public string CardCurrency { get; set; }
		public string CardCurrencyName { get; set; }
		public string AmountPaidInCardCurrency { get; set; }
		public string ExchangeRate { get; set; }
		public string Markup { get; set; }
	}
	public class Payer
	{
		public string Information { get; set; }
	}

	public class ComtrustFinalizationResponse
	{
		public ResponseTransaction Transaction { get; set; }
	}

	public class ResponseTransaction
	{
		public string ResponseCode { get; set; }
		public string ResponseClass { get; set; }
		public string ResponseDescription { get; set; }
		public string ResponseClassDescription { get; set; }
		public string Language { get; set; }
		public string ApprovalCode { get; set; }
		public string Account { get; set; }
		public Balance Balance { get; set; }
		public string OrderID { get; set; }
		public Amount Amount { get; set; }
		public Fees Fees { get; set; }
		public string CardNumber { get; set; }
		public Payer Payer { get; set; }
		public string CardToken { get; set; }
		public string CardBrand { get; set; }
		public string CardType { get; set; }
		public string IsWalletUsed { get; set; }
		public string IsCaptured { get; set; }
		public CurrencyConversion CurrencyConversion { get; set; }
		public string UniqueID { get; set; }
	}
}
