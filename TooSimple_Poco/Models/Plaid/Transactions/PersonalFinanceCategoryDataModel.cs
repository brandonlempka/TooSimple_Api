using System.Text.Json.Serialization;

namespace TooSimple_Poco.Models.Plaid.Transactions
{
    public class PersonalFinanceCategoryDataModel
	{
		[JsonPropertyName("primary")]
		public string Primary { get; set; } = string.Empty;

		[JsonPropertyName("detailed")]
		public string Detailed { get; set; } = string.Empty;
	}
}

