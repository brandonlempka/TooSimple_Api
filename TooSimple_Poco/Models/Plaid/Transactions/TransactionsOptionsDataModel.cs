using System.Text.Json.Serialization;

namespace TooSimple_Poco.Models.Plaid.Transactions
{
    public class TransactionsOptionsDataModel
	{
		[JsonPropertyName("include_personal_finance_category")]
		public bool IncludePersonalFinanceCategory { get; set; }

		[JsonPropertyName("account_ids")]
		public string[]? AccountIds { get; set; }

		[JsonPropertyName("count")]
		public int Count { get; set; } = 50;

		[JsonPropertyName("offset")]
		public int Offset { get; set; } = 0;
	}
}
