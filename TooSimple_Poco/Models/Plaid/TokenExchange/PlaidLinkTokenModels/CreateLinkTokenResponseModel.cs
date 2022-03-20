using System.Text.Json.Serialization;

namespace TooSimple_Poco.Models.Plaid.TokenExchange.PlaidLinkTokenModels
{
    public class CreateLinkTokenResponseModel
    {
        public string? Expiration { get; set; }
        [JsonPropertyName("link_token")]
        public string? LinkToken { get; set; }
        [JsonPropertyName("request_id")]
        public string? RequestId { get; set; }

    }
}
