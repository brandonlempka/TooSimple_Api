namespace TooSimple_Poco.Models.Database
{
    public class TransactionDataModel
	{
		public string? PlaidTransactionId { get; set; } = string.Empty;
		public string? AccountOwner { get; set; }
		public decimal Amount { get; set; }
		public DateTime? AuthorizedDate { get; set; }
		public DateTime TransactionDate { get; set; }
		public string? CategoryId { get; set; }
		public string? PrimaryCategory { get; set; } = string.Empty;
		public string? DetailedCategory { get; set; } = string.Empty;
		public string? CurrencyCode { get; set; } = string.Empty;
		public string? Address { get; set; }
		public string? City { get; set; }
		public string? Country { get; set; }
		public string? Latitude { get; set; }
		public string? Longitude { get; set; }
		public string? PostalCode { get; set; }
		public string? Region { get; set; }
		public string? StoreNumber { get; set; }
		public string? MerchantName { get; set; }
		public string? Name { get; set; }
		public string? PaymentChannel { get; set; }
		public string? ByOrderOf { get; set; }
		public string? Payee { get; set; }
		public string? Payer { get; set; }
		public string? PaymentMethod { get; set; }
		public string? PaymentProcessor { get; set; }
		public string? PpdId { get; set; }
		public string? Reason { get; set; }
		public string? ReferenceNumber { get; set; }
		public bool IsPending { get; set; }
		public string? PendingTransactionId { get; set; }
		public string? TransactionCode { get; set; }
		public string? TransactionType { get; set; }
		public string? UnofficialCurrencyCode { get; set; }
		public string? SpendingFromGoalId { get; set; }
		public string? PlaidAccountId { get; set; }
		public string? UserAccountId { get; set; }
	}
}
