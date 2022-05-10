using System.Text.Json.Serialization;

namespace TooSimple_Poco.Models.Plaid.TokenExchange
{
	public class TokenExchangeAccount
	{
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("mask")]
        public string? Mask { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("subtype")]
        public string? Subtype { get; set; }

        [JsonPropertyName("verification_status")]
        public string? VerificationStatus { get; set; }

        [JsonPropertyName("class_type")]
        public string? ClassType { get; set; }
    }
}
