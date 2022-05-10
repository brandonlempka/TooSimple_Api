using System.Text.Json.Serialization;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Poco.Models.Plaid.TokenExchange
{
	public class TokenExchangeDataModel : PlaidApiResponse
	{
        [JsonPropertyName("public_token")]
        public string? PublicToken { get; set; }

        [JsonPropertyName("accounts")]
        public List<TokenExchangeAccount>? Accounts { get; set; }

        [JsonPropertyName("institution")]
        public TokenExchangeInstitution? Institution { get; set; }

        [JsonPropertyName("link_session_id")]
        public string? LinkSessionId { get; set; }
    }
}
