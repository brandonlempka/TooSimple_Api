using System.Net;
using TooSimple_DataAccessors.Database.PlaidAccounts;
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
