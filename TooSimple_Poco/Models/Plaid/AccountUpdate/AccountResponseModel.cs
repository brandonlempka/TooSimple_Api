using System.Text.Json.Serialization;

namespace TooSimple_Poco.Models.Plaid.AccountUpdate
{
    public class AccountResponseModel
    {
        [JsonPropertyName("account_id")]
        public string? AccountId { get; set; }
        [JsonPropertyName("balances")]
        public BalanceResponseModel? Balances { get; set; }
        [JsonPropertyName("mask")]
        public string? Mask { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("official_name")]
        public string? OfficialName { get; set; }
        [JsonPropertyName("subtype")]
        public string? Subtype { get; set; }
        [JsonPropertyName("type")]
        public string? Type { get; set; }
    }
}
