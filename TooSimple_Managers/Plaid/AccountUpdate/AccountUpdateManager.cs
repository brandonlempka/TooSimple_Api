using System.Text.Json;
using TooSimple_DataAccessors.Database.Accounts;
using TooSimple_DataAccessors.Database.Logging;
using TooSimple_DataAccessors.Plaid.AccountUpdate;
using TooSimple_Managers.Budgeting;
using TooSimple_Poco.Enums;
using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.Plaid.AccountUpdate;
using TooSimple_Poco.Models.Plaid.Webhooks;

namespace TooSimple_Managers.Plaid.AccountUpdate
{
    public class AccountUpdateManager : IAccountUpdateManager
    {
        private readonly IAccountUpdateAccessor _accountUpdateAccessor;
        private readonly IAccountAccessor _accountAccessor;
        private readonly ILoggingAccessor _loggingAccessor;
        private readonly IBudgetingManager _budgetingManager;

        public AccountUpdateManager(
            IAccountUpdateAccessor accountUpdateAccessor,
            IAccountAccessor accountAccessor,
            ILoggingAccessor loggingAccessor,
            IBudgetingManager budgetingManager)
        {
            _accountUpdateAccessor = accountUpdateAccessor;
            _accountAccessor = accountAccessor;
            _loggingAccessor = loggingAccessor;
            _budgetingManager = budgetingManager;
        }

        /// <summary>
        /// Calls plaid to update account balances.
        /// </summary>
        /// <param name="userId">Too Simple user Id to update.</param>
        /// <returns>Boolean indicating success.</returns>
        public async Task<bool> UpdateAccountBalancesByUserIdAsync(string userId)
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
                    .Where(account => !account.IsActiveForBudgetingFeatures)
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
                    bool response = await _accountAccessor.UpdateAccountRelogAsync(true, accountIds);
                    if (!response)
                        return false;
                }
                else
                {
                    bool response = await _accountAccessor.UpdateAccountBalancesAsync(plaidUpdateResponse);
                }
            }

            return true;
        }

        /// <summary>
        /// Calls plaid to update account balances.
        /// </summary>
        /// <param name="userId">Plaid accountId to update.</param>
        /// <returns>Boolean indicating success.</returns>
        public async Task<bool> UpdateAccountBalancesByItemIdAsync(JsonElement json)
        {
            var test = await _loggingAccessor.LogMessageAsync(null, json.ToString());

            PlaidWebhookResponseDto? webhookResponse = JsonSerializer.Deserialize<PlaidWebhookResponseDto>(json);
            return true;
        }
    }
}
