using System.Text.Json.Serialization;
using TooSimple_Poco.Settings;

namespace TooSimple_Poco.Models.Plaid.Shared
{
    public class PlaidRequestModel
    {
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; } = string.Empty;
        [JsonPropertyName("secret")]
        public string PlaidSecret { get; set; } = string.Empty;

        public PlaidRequestModel()
        {
            ClientId = PlaidSettings.ClientId;
            PlaidSecret = PlaidSettings.Secret;
        }
    }
}
