using System.Text;
using System.Text.Json;
using TooSimple_Poco.Models.Plaid.TokenExchange;
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
        /// <param name="userId">
        /// Plaid requires a user ID to generate a link token.
        /// </param>
        /// <returns>
        /// Response body of plaid request.
        /// </returns>
        public async Task<CreateLinkTokenResponseModel> CreateLinkTokenAsync(string userId)
        {
            CreateLinkTokenRequestModel request = new(userId);

            string json = JsonSerializer.Serialize(request);
            StringContent stringContent = new(
                json,
                Encoding.UTF8,
                "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(
                    $"{PlaidSettings.BaseUrl}/link/token/create",
                    stringContent);

                CreateLinkTokenResponseModel? responseModel = await JsonSerializer
                    .DeserializeAsync<CreateLinkTokenResponseModel>(
                        response.Content.ReadAsStream());

                return responseModel ?? new CreateLinkTokenResponseModel();
            }
            catch (Exception ex)
            {
                CreateLinkTokenResponseModel errorModel = new()
                {
                    ErrorMessage = ex.ToString(),
                };

                return errorModel;
            }
        }

        /// <summary>
        /// Calls plaid to exchange public token for access token.
        /// </summary>
        /// <param name="requestModel">
        /// <see cref="TokenExchangeRequestModel"/> with public token.
        /// </param>
        /// <returns>
        /// <see cref="TokenExchangeResponseModel"/> with plaid response.
        /// If success, includes access token.
        /// </returns>
        public async Task<TokenExchangeResponseModel> PublicTokenExchangeAsync(TokenExchangeRequestModel requestModel)
        {
            string json = JsonSerializer.Serialize(requestModel);
            StringContent stringContent = new(
                json,
                Encoding.UTF8,
                "application/json");

            try
            {
                HttpResponseMessage response = await _httpClient.PostAsync(
                    $"{PlaidSettings.BaseUrl}/item/public_token/exchange",
                    stringContent);

                TokenExchangeResponseModel? responseModel = await JsonSerializer
                    .DeserializeAsync<TokenExchangeResponseModel>(
                        response.Content.ReadAsStream());

                return responseModel ?? new TokenExchangeResponseModel();
            }
            catch (Exception ex)
            {
                TokenExchangeResponseModel errorModel = new()
                {
                    ErrorMessage = ex.ToString(),
                };

                return errorModel;
            }
        }
    }
}
