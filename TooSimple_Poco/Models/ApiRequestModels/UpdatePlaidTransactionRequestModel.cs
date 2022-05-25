using System.Text.Json.Serialization;

namespace TooSimple_Poco.Models.ApiRequestModels
{
	public class UpdatePlaidTransactionRequestModel
	{
        [JsonPropertyName("spendingFromGoalId")]
		public string? SpendingFromGoalId { get; set; }

        [JsonPropertyName("plaidTransactionId")]
		public string? PlaidTransactionId { get; set; }
	}
}
