using System.Text.Json.Serialization;
using TooSimple_Poco.Models.Plaid.Shared;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Poco.Models.Plaid.AccountUpdate
{
    public class AccountUpdateResponseModel : PlaidApiResponse
    {
        [JsonPropertyName("accounts")]
        public IEnumerable<AccountResponseModel>? Accounts { get; set; }

        [JsonPropertyName("item")]
        public PlaidItemDataModel? Item { get; set; }
    }
}
