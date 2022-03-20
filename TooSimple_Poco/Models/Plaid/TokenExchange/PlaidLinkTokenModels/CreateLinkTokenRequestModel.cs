using System.Text.Json.Serialization;
using TooSimple_Poco.Settings;

namespace TooSimple_Poco.Models.Plaid.TokenExchange.PlaidLinkTokenModels
{
    public class CreateLinkTokenRequestModel
    {
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; } = string.Empty;
        [JsonPropertyName("secret")]
        public string PlaidSecret { get; set; } = string.Empty;
        [JsonPropertyName("client_name")]
        public string ClientName { get; set; } = string.Empty;
        [JsonPropertyName("country_codes")]
        public string[] CountryCodes { get; set; } = Array.Empty<string>();
        [JsonPropertyName("language")]
        public string Language { get; set; } = string.Empty;
        [JsonPropertyName("products")]
        public string[] Products { get; set; } = Array.Empty<string>();
        [JsonPropertyName("user")]
        public CreateLinkTokenUserModel User { get; set; }

        public CreateLinkTokenRequestModel(string userId)
        {
            ClientId = PlaidSettings.ClientId;
            PlaidSecret = PlaidSettings.Secret;
            ClientName = PlaidSettings.ClientName;
            CountryCodes = PlaidSettings.Countries;
            Language = PlaidSettings.Language;
            Products = PlaidSettings.Products;
            User = new()
            {
                ClientUserId = userId
            };
        }
    }
}
