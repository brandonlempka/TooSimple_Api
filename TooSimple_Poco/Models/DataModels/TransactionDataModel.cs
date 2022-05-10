using TooSimple_Poco.Models.Database;

namespace TooSimple_Poco.Models.DataModels
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

		public TransactionDataModel() { }

        public TransactionDataModel(PlaidTransaction plaidTransaction)
        {
            PlaidTransactionId = plaidTransaction.PlaidTransactionId;
            AccountOwner = plaidTransaction.AccountOwner;
            Amount = plaidTransaction.Amount;
            AuthorizedDate = plaidTransaction.AuthorizedDate;
            TransactionDate = plaidTransaction.TransactionDate;
            CategoryId = plaidTransaction.CategoryId;
            PrimaryCategory = plaidTransaction.PrimaryCategory;
            DetailedCategory = plaidTransaction.DetailedCategory;
            CurrencyCode = plaidTransaction.CurrencyCode;
            Address = plaidTransaction.Address;
            City = plaidTransaction.City;
            Country = plaidTransaction.Country;
            Latitude = plaidTransaction.Latitude;
            Longitude = plaidTransaction.Longitude;
            PostalCode = plaidTransaction.PostalCode;
            Region = plaidTransaction.Region;
            StoreNumber = plaidTransaction.StoreNumber;
            MerchantName = plaidTransaction.MerchantName;
            Name = plaidTransaction.Name;
            PaymentChannel = plaidTransaction.PaymentChannel;
            ByOrderOf = plaidTransaction.ByOrderOf;
            Payee = plaidTransaction.Payee;
            Payer = plaidTransaction.Payer;
            PaymentMethod = plaidTransaction.PaymentMethod;
            PaymentProcessor = plaidTransaction.PaymentProcessor;
            PpdId = plaidTransaction.PpdId;
            Reason = plaidTransaction.Reason;
            ReferenceNumber = plaidTransaction.ReferenceNumber;
            IsPending = plaidTransaction.IsPending;
            PendingTransactionId = plaidTransaction.PendingTransactionId;
            TransactionCode = plaidTransaction.TransactionCode;
            TransactionType = plaidTransaction.TransactionType;
            UnofficialCurrencyCode = plaidTransaction.UnofficialCurrencyCode;
            SpendingFromGoalId = plaidTransaction.SpendingFromGoalId;
            PlaidAccountId = plaidTransaction.PlaidAccountId;
            UserAccountId = plaidTransaction.UserAccountId;
        }
    }
}
