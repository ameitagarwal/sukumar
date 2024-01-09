namespace TMS_Traning_Management.Models
{
	public class ComtrustRegistrationRequest
	{
		public ComtrustRegistration Registration { get; set; }
	}
	public class ComtrustRegistrationResponse
	{
		public RegistrationTransaction Transaction { get; set; }
	}
	public class ComtrustRegistration
	{
		public string Currency { get; set; }
		public string ReturnPath { get; set; }
		public string TransactionHint { get; set; }
		public string OrderID { get; set; }
		public string Store { get; set; }
		public string Terminal { get; set; }
		public string Channel { get; set; }
		public string Amount { get; set; }
		public string Customer { get; set; }
		public string OrderName { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
		public ExtraData ExtraData { get; set; }

	}
	public class ExtraData
	{
		public string MerchantDescriptor { get; set; }
		public string TransactionDescriptor { get; set; }
	}
	public class Amount
	{
		public string Value { get; set; }
	}

	public class Balance
	{
		public string Value { get; set; }
	}

	public class Fees
	{
		public string Value { get; set; }
	}

	public class RegistrationTransaction
	{
		public string PaymentPortal { get; set; }
		public string PaymentPage { get; set; }
		public string ResponseCode { get; set; }
		public string ResponseClass { get; set; }
		public string ResponseDescription { get; set; }
		public string ResponseClassDescription { get; set; }
		public string TransactionID { get; set; }
		public Balance Balance { get; set; }
		public Amount Amount { get; set; }
		public Fees Fees { get; set; }
		public object Payer { get; set; }
		public string UniqueID { get; set; }
	}
}