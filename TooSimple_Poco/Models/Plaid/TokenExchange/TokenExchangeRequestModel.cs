using System.Text.Json.Serialization;
using TooSimple_Poco.Models.Plaid.Shared;
using TooSimple_Poco.Settings;

namespace TooSimple_Poco.Models.Plaid.TokenExchange
{
	public class TokenExchangeRequestModel : PlaidRequestModel
    {
		[JsonPropertyName("public_token")]
		public string PublicToken { get; set; } = string.Empty;

        public TokenExchangeRequestModel(string publicToken) : base()
        {
            PublicToken = publicToken;
        }
    }
}
