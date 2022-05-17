using System.Text.Json.Serialization;
using TooSimple_Poco.Models.Plaid.AccountUpdate;
using TooSimple_Poco.Models.Plaid.Shared;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Poco.Models.Plaid.Transactions
{
	public class PlaidGetTransactionsResponseModel : PlaidApiResponse
	{
		[JsonPropertyName("accounts")]
		public IEnumerable<AccountResponseModel>? Accounts { get; set; }

		[JsonPropertyName("item")]
		public PlaidItemDataModel? Item { get; set; }

		[JsonPropertyName("total_transactions")]
		public int TotalTransactions { get; set; }

		[JsonPropertyName("transactions")]
		public IEnumerable<TransactionResponseModel>? Transactions { get; set; }
	}
}

