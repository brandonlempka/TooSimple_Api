using System.Net;
using TooSimple_DataAccessors.Database.PlaidAccounts;
using TooSimple_Poco.Models.DataModels;
using TooSimple_Poco.Models.Dtos.PlaidAccounts;
using TooSimple_Poco.Models.Entities;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Managers.PlaidAccounts
{
    public class PlaidAccountManager : IPlaidAccountManager
    {
        private readonly IPlaidAccountAccessor _plaidAccountAccessor;

        public PlaidAccountManager(IPlaidAccountAccessor plaidAccountAccessor)
        {
            _plaidAccountAccessor = plaidAccountAccessor;
        }

        /// <summary>
        /// Calls data accessor to retrieve plaid accounts for user.
        /// </summary>
        /// <param name="userId">
        /// User Id to run query against.
        /// </param>
        /// <returns>
        /// <see cref="GetPlaidAccountsDto"/> dto with user accounts if successful.
        /// </returns>
        public async Task<GetPlaidAccountsDto> GetPlaidAccountsByUserIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new()
                {
                    Status = HttpStatusCode.BadRequest,
                    ErrorMessage = "User Id is required."
                };
            }

            IEnumerable<PlaidAccount> response = await _plaidAccountAccessor.GetPlaidAccountsByUserIdAsync(userId);
            if (!response.Any())
            {
                return new()
                {
                    Status = HttpStatusCode.NoContent,
                    Success = true
                };
            }

            IEnumerable<PlaidAccountDataModel> plaidAccountDataModels = response
                .Select(account => new PlaidAccountDataModel(account));

            GetPlaidAccountsDto getPlaidAccountsDto = new()
            {
                Status = HttpStatusCode.OK,
                Success = true,
                PlaidAccounts = plaidAccountDataModels
            };

            return getPlaidAccountsDto;
        }

        /// <summary>
        /// Calls data accessor to delete account and associated transactions
        /// from database.
        /// </summary>
        /// <param name="accountId">
        /// Account Id to delete.
        /// </param>
        /// <returns>
        /// <see cref="BaseHttpResponse"/> indicating success or failure.
        /// </returns>
        public async Task<BaseHttpResponse> DeleteAccountAsync(string accountId)
        {
            if (string.IsNullOrWhiteSpace(accountId))
                return new()
                {
                    ErrorMessage = "Account Id is not formatted correctly.",
                    Status = HttpStatusCode.BadRequest
                };

            DatabaseResponseModel databaseResponse = await _plaidAccountAccessor.DeleteAccountAsync(accountId);
            BaseHttpResponse httpResponse = BaseHttpResponse.CreateResponseFromDb(databaseResponse);

            return httpResponse;
        }
    }
}
