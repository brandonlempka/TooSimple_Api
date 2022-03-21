using System.Text.Json.Serialization;

namespace TooSimple_Poco.Models.Shared
{
    public class PlaidApiResponse
    {
        [JsonPropertyName("display_message")]
        public string? DisplayMessage { get; set; }
        [JsonPropertyName("documentation_url")]
        public string? DocumentationUrl { get; set; }
        [JsonPropertyName("error_code")]
        public string? ErrorCode { get; set; }
        [JsonPropertyName("error_message")]
        public string? ErrorMessage { get; set; }
        [JsonPropertyName("error_type")]
        public string? ErrorType { get; set; }
        [JsonPropertyName("request_id")]
        public string? RequestId { get; set; }
        [JsonPropertyName("suggested_action")]
        public object? SuggestedAction { get; set; }
    }
}
