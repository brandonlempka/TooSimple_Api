using System.Text.Json.Serialization;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Poco.Models.Plaid.TokenExchange
{
	public class TokenExchangeResponseModel : PlaidApiResponse
	{
		[JsonPropertyName("access_token")]
		public string? AccessToken { get; set; }

		[JsonPropertyName("item_id")]
		public string? ItemId { get; set; }
	}
}
