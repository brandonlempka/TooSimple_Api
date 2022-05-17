using System.Text;
using System.Text.Json;
using TooSimple_Poco.Models.Plaid.AccountUpdate;
using TooSimple_Poco.Models.Plaid.Transactions;
using TooSimple_Poco.Settings;

namespace TooSimple_DataAccessors.Plaid.AccountUpdate
{
    public class PlaidSyncAccessor : IPlaidSyncAccessor
    {
        private static readonly HttpClient _httpClient = new();

        /// <summary>
        /// Calls plaid to get updated account balance information.
        /// </summary>
        /// <param name="requestModel">
        /// <see cref="AccountUpdateRequestModel"/> containing access token
        /// & account IDs to update.
        /// </param>
        /// <returns>
        /// <see cref="AccountUpdateResponseModel"/>
        /// Response model with new account balancs.
        /// </returns>
        public async Task<AccountUpdateResponseModel> UpdateAccountBalancesAsync(
            AccountUpdateRequestModel requestModel)
        {
            string json = JsonSerializer.Serialize(requestModel);
            StringContent stringContent = new(
                json,
                Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(
                $"{PlaidSettings.BaseUrl}/accounts/balance/get",
                stringContent);

            AccountUpdateResponseModel? responseModel = await JsonSerializer
                .DeserializeAsync<AccountUpdateResponseModel>(
                    response.Content.ReadAsStream());

            return responseModel ?? new AccountUpdateResponseModel();
        }

        /// <summary>
        /// Calls plaid to get transactions for account Ids.
        /// </summary>
        /// <param name="requestModel">
        /// <see cref="TransactionUpdateRequestModel"/> containing access token
        /// & account Ids to update.
        /// </param>
        /// <returns>
        /// <see cref="AccountUpdateResponseModel"/> with new transactions.
        /// </returns>
        public async Task<PlaidGetTransactionsResponseModel> GetPlaidTransactionsAsync(
            TransactionUpdateRequestModel requestModel)
        {
            string json = JsonSerializer.Serialize(requestModel);
            StringContent stringContent = new(
                json,
                Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(
                $"{PlaidSettings.BaseUrl}/transactions/get",
                stringContent);

            PlaidGetTransactionsResponseModel? responseModel = await JsonSerializer
                .DeserializeAsync<PlaidGetTransactionsResponseModel>(
                    response.Content.ReadAsStream());

            return responseModel ?? new()
            {
                ErrorMessage = "Something went wrong."
            };
        }
    }
}
