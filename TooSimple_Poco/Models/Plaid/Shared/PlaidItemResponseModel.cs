using System.Text.Json.Serialization;

namespace TooSimple_Poco.Models.Plaid.Shared
{
    public class PlaidItemResponseModel
    {
        [JsonPropertyName("available_products")]
        public IEnumerable<string>? AvailableProducts { get; set; }
        [JsonPropertyName("billed_products")]
        public IEnumerable<string>? BilledProducts { get; set; }
        
        [JsonPropertyName("consent_expiration_time")]
        public object? ConsentExpirationTime { get; set; }
        
        [JsonPropertyName("error")]
        public object? Error { get; set; }
        
        [JsonPropertyName("institution_id")]
        public string? InstitutionId { get; set; }
        
        [JsonPropertyName("item_id")]
        public string? ItemId { get; set; }
        
        [JsonPropertyName("webhook")]
        public string? Webhook { get; set; }
    }
}
