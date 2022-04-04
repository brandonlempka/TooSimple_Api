using System.Text.Json.Serialization;

namespace TooSimple_Poco.Models.Plaid.Webhooks
{
    public class PlaidWebhookResponseDto
    {
        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("item_id")]
        public string? ItemId { get; set; }

        [JsonPropertyName("new_transactions")]
        public int? NewTransactions{ get; set; }

        [JsonPropertyName("webhook_code")]
        public string? WebhookCode { get; set; }

        [JsonPropertyName("webhook_type")]
        public string? WebhookType { get; set; }
    }
}
