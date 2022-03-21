using System.Text.Json.Serialization;

namespace TooSimple_Poco.Models.Plaid.AccountUpdate
{
    public class AccountIdsRequestModel
    {
        [JsonPropertyName("account_ids")]
        public string[]? AccountIds { get; set; }
    }
}
