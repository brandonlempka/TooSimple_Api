using TooSimple_DataAccessors.Database.Accounts;
using TooSimple_DataAccessors.Plaid.AccountUpdate;
using TooSimple_Poco.Enums;
using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.Plaid.AccountUpdate;

namespace TooSimple_Managers.Plaid.AccountUpdate
{
    public class AccountUpdateManager : IAccountUpdateManager
    {
        private readonly IAccountUpdateAccessor _accountUpdateAccessor;
        private readonly IAccountAccessor _accountAccessor;

        public AccountUpdateManager(
            IAccountUpdateAccessor accountUpdateAccessor,
            IAccountAccessor accountAccessor)
        {
            _accountUpdateAccessor = accountUpdateAccessor;
            _accountAccessor = accountAccessor;
        }

        /// <summary>
        /// Calls plaid to update account balances.
        /// </summary>
        /// <param name="userId">Too Simple user Id to update.</param>
        /// <returns>Boolean indicating success.</returns>
        public async Task<bool> UpdateAccountBalancesAsync(string userId)
        {
            IEnumerable<PlaidAccountDataModel> plaidAccounts = await _accountAccessor.GetPlaidAccountsAsync(userId);
            if (plaidAccounts is null || !plaidAccounts.Any())
            {
                return false;
            }

            IEnumerable<
                IGrouping<string, PlaidAccountDataModel>> accountGroups = plaidAccounts.GroupBy(account => account.AccessToken);

            foreach (IGrouping<string, PlaidAccountDataModel> group in accountGroups)
            {
                string accessToken = group.Key;
                string[] accountIds = group
                    .Where(account => !account.ReLoginRequired)
                    .Select(account => account.PlaidAccountId)
                    .ToArray();

                AccountUpdateRequestModel requestModel = new(
                    accessToken,
                    accountIds);

                AccountUpdateResponseModel plaidUpdateResponse = await _accountUpdateAccessor
                    .UpdateAccountBalancesAsync(requestModel);

                if (plaidUpdateResponse is null)
                {
                    return false;
                }

                if (plaidUpdateResponse.ErrorCode == PlaidErrorCodes.ITEM_LOGIN_REQUIRED.ToString())
                {
                    // to do handle relog requested things.
                }
                else
                {

                var response = _accountAccessor.UpdateAccountBalancesAsync(plaidUpdateResponse);
                }

                var test = "123";
            }

            return true;
        }
    }
}
