using System.Text.Json.Serialization;

namespace TooSimple_Poco.Models.Plaid.Transactions
{
    public class TransactionResponseModel
    {
        [JsonPropertyName("account_id")]
        public string? AccountId { get; set; }

        [JsonPropertyName("account_owner")]
        public string? AccountOwner { get; set; }

        [JsonPropertyName("amount")]
        public double? Amount { get; set; }

        [JsonPropertyName("authorized_date")]
        public DateTime? AuthorizedDate { get; set; }

        [JsonPropertyName("authorized_datetime")]
        public DateTime? AuthorizedDateTime { get; set; }

        [JsonPropertyName("category")]
        public List<string>? Category { get; set; }

        [JsonPropertyName("category_id")]
        public string? CategoryId { get; set; }

        [JsonPropertyName("check_number")]
        public string? CheckNumber { get; set; }

        [JsonPropertyName("date")]
        public DateTime? Date { get; set; }

        [JsonPropertyName("datetime")]
        public DateTime? Datetime { get; set; }

        [JsonPropertyName("iso_currency_code")]
        public string? IsoCurrencyCode { get; set; }

        [JsonPropertyName("location")]
        public LocationDataModel? Location { get; set; }

        [JsonPropertyName("merchant_name")]
        public string? MerchantName { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("payment_channel")]
        public string? PaymentChannel { get; set; }

        [JsonPropertyName("payment_meta")]
        public PaymentMetaDataModel? PaymentMeta { get; set; }

        [JsonPropertyName("pending")]
        public bool IsPending { get; set; }

        [JsonPropertyName("pending_transaction_id")]
        public string? PendingTransactionId { get; set; }

        [JsonPropertyName("personal_finance_category")]
        public PersonalFinanceCategoryDataModel? PersonalFinanceCategory { get; set; }

        [JsonPropertyName("transaction_code")]
        public string? TransactionCode { get; set; }

        [JsonPropertyName("transaction_id")]
        public string? TransactionId { get; set; }

        [JsonPropertyName("transaction_type")]
        public string? TransactionType { get; set; }

        [JsonPropertyName("unofficial_currency_code")]
        public string? UnofficialCurrencyCode { get; set; }
    }
}
