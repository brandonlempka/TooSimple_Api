using System.Text.Json.Serialization;
using TooSimple_Poco.Models.Plaid.Shared;

namespace TooSimple_Poco.Models.Plaid.AccountUpdate
{
    public class AccountUpdateRequestModel : PlaidRequestModel
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("options")]
        public AccountIdsRequestModel? Options { get; set; }

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
        public AccountUpdateRequestModel(
            string accessToken,
            string[] accountIds) : base()
        {
            AccessToken = accessToken;
            Options = new()
            {
                AccountIds = accountIds
            };
        }
    }
}
