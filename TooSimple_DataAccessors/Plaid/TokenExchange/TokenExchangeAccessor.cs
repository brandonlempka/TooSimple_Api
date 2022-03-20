using System.Text;
using System.Text.Json;
using TooSimple_Poco.Models.Plaid.TokenExchange.PlaidLinkTokenModels;
using TooSimple_Poco.Settings;

namespace TooSimple_DataAccessors.Plaid.TokenExchange
{
    /// <summary>
    /// Performs account linking for new setup and unlocking accounts.
    /// </summary>
    public class TokenExchangeAccessor : ITokenExchangeAccessor
    {
        private static readonly HttpClient _httpClient = new();

        /// <summary>
        /// Creates initial link token for starting Plaid Link.
        /// First step in linking new bank account.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Response body of plaid request.</returns>
        public async Task<CreateLinkTokenResponseModel> CreateLinkTokenAsync(string userId)
        {
            CreateLinkTokenRequestModel? request = new(userId);

            string? json = JsonSerializer.Serialize(request);
            StringContent? stringContent = new(
                json,
                Encoding.UTF8, 
                "application/json");

            HttpResponseMessage? response = await _httpClient.PostAsync(
                $"{PlaidSettings.BaseUrl}/link/token/create",
                stringContent);

            CreateLinkTokenResponseModel? responseModel = await JsonSerializer
                .DeserializeAsync<CreateLinkTokenResponseModel>(
                    response.Content.ReadAsStream());

            return responseModel ?? new CreateLinkTokenResponseModel();
        }
    }
}
