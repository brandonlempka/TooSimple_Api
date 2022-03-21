using System.Text.Json.Serialization;

namespace TooSimple_Poco.Models.Plaid.AccountUpdate
{
    public class BalanceResponseModel
    {
        [JsonPropertyName("available")]
        public decimal? Available { get; set; }
        [JsonPropertyName("current")]
        public decimal? Current { get; set; }
        [JsonPropertyName("iso_currency_code")]
        public string? IsoCurrencyCode { get; set; }
        [JsonPropertyName("limit")]
        public int? Limit { get; set; }
        [JsonPropertyName("unofficial_currency_code")]
        public object? UnofficialCurrencyCode { get; set; }
    }
}
