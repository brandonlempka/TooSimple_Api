using System.Text.Json.Serialization;
using TooSimple_Poco.Models.Plaid.Shared;
using TooSimple_Poco.Settings;

namespace TooSimple_Poco.Models.Plaid.TokenExchange
{
	public class TokenExchangeRequestModel
    {
		[JsonPropertyName("public_token")]
		public string PublicToken { get; set; } = string.Empty;

        [JsonPropertyName("client_id")]
        public string ClientId { get; set; } = string.Empty;
        [JsonPropertyName("secret")]
        public string PlaidSecret { get; set; } = string.Empty;
        public TokenExchangeRequestModel(string publicToken) : base()
        {
            PublicToken = publicToken;
            ClientId = PlaidSettings.ClientId;
            PlaidSecret = PlaidSettings.Secret;
        }
    }
}
