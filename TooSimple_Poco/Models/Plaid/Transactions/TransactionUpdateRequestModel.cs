using System.Text.Json.Serialization;
using TooSimple_Poco.Models.Plaid.Shared;

namespace TooSimple_Poco.Models.Plaid.Transactions
{
    public class TransactionUpdateRequestModel  : PlaidRequestModel
	{
        [JsonPropertyName("start_date")]
        public string StartDate { get; set; } = DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-dd");

        [JsonPropertyName("end_date")]
        public string EndDate { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-dd");

        [JsonPropertyName("options")]
        public TransactionsOptionsDataModel Options { get; set; }

        /// <summary>
        /// Creates new request token.
        /// </summary>
        /// <param name="accessToken">
        /// Plaid access token associated with this series of accounts.
        /// </param>
        /// <param name="accountIds">
        /// One plaid access token can be used with multiple account IDs if
        /// they are added at the same time.
        /// </param>        
        public TransactionUpdateRequestModel(
            string accessToken,
            string[] accountIds) : base()
        {
            AccessToken = accessToken;
            Options = new()
            {
                AccountIds = accountIds,
                IncludePersonalFinanceCategory = true
            };
        }

        /// <summary>
        /// Creates new request token with count.
        /// </summary>
        /// <param name="accessToken">
        /// Plaid access token associated with this series of accounts.
        /// </param>
        /// <param name="accountIds">
        /// One plaid access token can be used with multiple account IDs if
        /// they are added at the same time.
        /// </param>        
        public TransactionUpdateRequestModel(
            string accessToken,
            string[] accountIds,
            int? count) : base()
        {
            AccessToken = accessToken;
            Options = new()
            {
                AccountIds = accountIds,
                IncludePersonalFinanceCategory = true,
                Count = count ?? 50
            };
        }
    }
}
