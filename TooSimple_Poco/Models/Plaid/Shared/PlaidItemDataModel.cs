using System.Text.Json.Serialization;

namespace TooSimple_Poco.Models.Plaid.Shared
{
    public class PlaidItemDataModel
    {
        [JsonPropertyName("available_products")]
        public List<string>? AvailableProducts { get; set; }

        [JsonPropertyName("billed_products")]
        public List<string>? BilledProducts { get; set; }

        [JsonPropertyName("consent_expiration_time")]
        public string? ConsentExpriationTime { get; set; }

        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonPropertyName("institution_id")]
        public string? InstitutionId { get; set; }

        [JsonPropertyName("item_id")]
        public string? ItemId { get; set; }

        [JsonPropertyName("optional_products")]
        public string? OptionalProducts { get; set; }

        [JsonPropertyName("products")]
        public List<string>? Products { get; set; }

        [JsonPropertyName("update_type")]
        public string? UpdateType { get; set; }

        [JsonPropertyName("webhook")]
        public string? Webhook { get; set; }
    }
}

