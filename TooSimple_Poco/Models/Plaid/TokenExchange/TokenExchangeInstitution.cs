using System.Text.Json.Serialization;

namespace TooSimple_Poco.Models.Plaid.TokenExchange
{
	public class TokenExchangeInstitution
	{
		[JsonPropertyName("name")]
		public string? Name { get; set; }

		[JsonPropertyName("institution_id")]
		public string? InstitutionId { get; set; }
	}
}
