using System.Text.Json.Serialization;
using TooSimple_Poco.Models.Plaid.Shared;
using TooSimple_Poco.Settings;

namespace TooSimple_Poco.Models.Plaid.TokenExchange.PlaidLinkTokenModels
{
    public class CreateLinkTokenRequestModel : PlaidRequestModel
    {
        [JsonPropertyName("country_codes")]
        public string[] CountryCodes { get; set; } = Array.Empty<string>();

        [JsonPropertyName("language")]
        public string Language { get; set; } = string.Empty;

        [JsonPropertyName("products")]
        public string[] Products { get; set; } = Array.Empty<string>();

        [JsonPropertyName("user")]
        public CreateLinkTokenUserModel User { get; set; }

        [JsonPropertyName("client_name")]
        public string ClientName { get; set; } = string.Empty;

        [JsonPropertyName("webhook")]
        public string WebHookCallbackUrl { get; set; } = string.Empty;

        public CreateLinkTokenRequestModel(string userId) : base()
        {
            ClientName = PlaidSettings.ClientName;
            CountryCodes = PlaidSettings.Countries;
            Language = PlaidSettings.Language;
            Products = PlaidSettings.Products;
            WebHookCallbackUrl = "https://api.brandonlempka.com/webhookHandler";
            User = new()
            {
                ClientUserId = userId
            };
        }
    }
}
