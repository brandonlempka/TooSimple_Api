using System.Text.Json.Serialization;

namespace TooSimple_Poco.Models.Plaid.Transactions
{
    public class PaymentMetaDataModel
    {
        [JsonPropertyName("by_order_of")]
        public string? ByOrderOf { get; set; }

        [JsonPropertyName("payee")]
        public string? Payee { get; set; }

        [JsonPropertyName("payer")]
        public string? Payer { get; set; }

        [JsonPropertyName("payment_method")]
        public string? PaymentMethod { get; set; }

        [JsonPropertyName("payment_processor")]
        public string? PaymentProcessor { get; set; }

        [JsonPropertyName("ppd_id")]
        public string? PpdId { get; set; }

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }

        [JsonPropertyName("reference_number")]
        public string? ReferenceNumber { get; set; }
    }
}
