using System.Text.Json.Serialization;

namespace TooSimple_Poco.Models.Plaid.TokenExchange.PlaidLinkTokenModels
{
    public class CreateLinkTokenUserModel
    {
        [JsonPropertyName("client_user_id")]
        public string? ClientUserId { get; set; }
    }
}
