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
            IEnumerable<PlaidAccountDataModel> plaidAccounts = await _accountAccessor.GetPlaidAccountsByUserIdAsync(userId);
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

                bool response = await PlaidBalanceUpdate(accessToken, accountIds);
                return response;
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
            bool response = false;
            PlaidWebhookResponseDto? webhookResponse = JsonSerializer.Deserialize<PlaidWebhookResponseDto>(json);

            if (webhookResponse is null)
            {
                return response;
            }

            string? errorCode = null;
            
            // todo
            // I'm not sure what else I'm likely to get, but probably need to handle this if
            // a lot of these come through 
            if (webhookResponse.WebhookType != PlaidWebhookType.TRANSACTIONS.ToString()
                || webhookResponse.WebhookCode != PlaidWebhookCode.DEFAULT_UPDATE.ToString())
            {
                response = await _loggingAccessor.LogMessageAsync(errorCode, json.ToString());
                return response;
            }

            if (webhookResponse.WebhookCode == PlaidWebhookCode.DEFAULT_UPDATE.ToString()
                && !string.IsNullOrWhiteSpace(webhookResponse.ItemId))
            {
                PlaidAccountDataModel plaidAccount = await _accountAccessor
                    .GetPlaidAccountsByItemIdAsync(webhookResponse.ItemId);

                if (plaidAccount is null || plaidAccount.IsPlaidRelogRequired)
                {
                    return response;
                }

                string[] accountId =
                {
                    plaidAccount.PlaidAccountId
                };

                response = await PlaidBalanceUpdate(plaidAccount.AccessToken, accountId);

                if (response)
                {
                    await _budgetingManager.UpdateBudgetByUserId(plaidAccount.UserAccountId);
                }

                return response;
            }

            return response;
        }

        private async Task<bool> PlaidBalanceUpdate(string accessToken, string[] accountIds)
        {
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
                bool lockResponse = await _accountAccessor.UpdateAccountRelogAsync(true, accountIds);
                return lockResponse;
            }

            bool response = await _accountAccessor.UpdateAccountBalancesAsync(plaidUpdateResponse);
            return response;
        }
    }
}
