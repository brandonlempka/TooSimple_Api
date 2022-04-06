using System.Text.Json.Serialization;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Poco.Models.Plaid.TokenExchange.PlaidLinkTokenModels
{
    public class CreateLinkTokenResponseModel : PlaidApiResponse
    {
        [JsonPropertyName("expiration")]
        public string? Expiration { get; set; }

        [JsonPropertyName("link_token")]
        public string? LinkToken { get; set; }
    }
}
