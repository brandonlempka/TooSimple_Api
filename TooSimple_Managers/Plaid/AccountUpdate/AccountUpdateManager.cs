using System.Text.Json;
using TooSimple_DataAccessors.Database.Accounts;
using TooSimple_DataAccessors.Database.Logging;
using TooSimple_DataAccessors.Database.PlaidAccounts;
using TooSimple_DataAccessors.Plaid.AccountUpdate;
using TooSimple_Managers.Budgeting;
using TooSimple_Poco.Enums;
using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.DataModels;
using TooSimple_Poco.Models.Entities;
using TooSimple_Poco.Models.Plaid.Transactions;
using TooSimple_Poco.Models.Plaid.Webhooks;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Managers.Plaid.AccountUpdate
{
    public class AccountUpdateManager : IAccountUpdateManager
    {
        private readonly IPlaidSyncAccessor _plaidSyncAccessor;
        private readonly IPlaidAccountAccessor _plaidAccountAccessor;
        private readonly ILoggingAccessor _loggingAccessor;
        private readonly IBudgetingManager _budgetingManager;

        public AccountUpdateManager(
            IPlaidSyncAccessor plaidSyncAccessor,
            IPlaidAccountAccessor plaidAccountAccessor,
            ILoggingAccessor loggingAccessor,
            IBudgetingManager budgetingManager)
        {
            _plaidSyncAccessor = plaidSyncAccessor;
            _plaidAccountAccessor = plaidAccountAccessor;
            _loggingAccessor = loggingAccessor;
            _budgetingManager = budgetingManager;
        }

        /// <summary>
        /// Calls plaid to update account balances.
        /// </summary>
        /// <param name="userId">Too Simple user Id to update.</param>
        /// <see cref="DatabaseResponseModel"/>
        /// Database response indicating success or failure.
        /// </returns>
        public async Task<DatabaseResponseModel> PlaidSyncByUserIdAsync(string userId)
        {
            IEnumerable<IGrouping<string, PlaidAccountDataModel>>? accountGroups = await GetAccountTokenGroups(userId);

            if (accountGroups is null)
                return DatabaseResponseModel.CreateError("Something went wrong while retrieving accounts from database.");

            foreach (IGrouping<string, PlaidAccountDataModel> group in accountGroups)
            {
                string accessToken = group.Key;
                string[] accountIds = group
                    .Where(account => !account.IsPlaidRelogRequired)
                    .Select(account => account.PlaidAccountId)
                    .ToArray();

                DatabaseResponseModel response = await PlaidSync(accessToken, accountIds);
                return response;
            }

            return DatabaseResponseModel.CreateError("Something went wrong.");
        }

        /// <summary>
        /// Calls plaid to update account balances.
        /// </summary>
        /// <param name="userId">Plaid accountId to update.</param>
        /// <returns>
        /// <see cref="DatabaseResponseModel"/>
        /// Database response indicating success or failure.
        /// </returns>
        public async Task<DatabaseResponseModel> PlaidSyncByItemIdAsync(JsonElement json)
        {
            PlaidWebhookResponseDto? webhookResponse = JsonSerializer.Deserialize<PlaidWebhookResponseDto>(json);

            if (webhookResponse is null)
                return DatabaseResponseModel.CreateError("Something went wrong while processing webhook.");


            // todo
            // I'm not sure what else I'm likely to get, but probably need to handle this if
            // a lot of these come through 
            if (webhookResponse.WebhookType != PlaidWebhookType.TRANSACTIONS.ToString()
                || !string.IsNullOrWhiteSpace(webhookResponse.ItemId))
            {
                bool logResponse = await _loggingAccessor.LogMessageAsync(json.ToString());

                return logResponse
                    ? DatabaseResponseModel.CreateSuccess()
                    : DatabaseResponseModel.CreateError("Failure while logging response.");
            }

            IEnumerable<PlaidAccount> plaidAccounts = await _plaidAccountAccessor
                .GetPlaidAccountsByItemIdAsync(webhookResponse.ItemId!);

            if (plaidAccounts is null
                || !plaidAccounts.Any())
                return DatabaseResponseModel.CreateError("Something went wrong.");

            string[] accountIds = plaidAccounts.Select(x => x.PlaidAccountId).ToArray();
            string? accessToken = plaidAccounts.FirstOrDefault()!.AccessToken;
            

            TransactionUpdateRequestModel requestModel = new(
                accessToken,
                accountIds,
                webhookResponse.NewTransactions);

            DatabaseResponseModel response = await PlaidSyncAsync(requestModel, plaidAccounts.FirstOrDefault()!.UserAccountId);

            if (response.Success)
            {
                //await _budgetingManager.UpdateBudgetByUserId(plaidAccount.UserAccountId);
            }

            return response;
        }

        /// <summary>
        /// Calls plaid to get new transactions.
        /// </summary>
        /// <param name="userId">
        /// String user Id to get new transactions for.
        /// </param>
        /// <returns>
        /// <see cref="DatabaseResponseModel"/>
        /// Database response indicating success or failure.
        /// </returns>
        public async Task<DatabaseResponseModel> GetNewTransactionsAsync(string userId)
        {
            IEnumerable<IGrouping<string, PlaidAccountDataModel>>? accountGroups = await GetAccountTokenGroups(userId);

            if (accountGroups is null)
                return DatabaseResponseModel.CreateError("Something went wrong while retrieving accounts from database.");

            foreach (IGrouping<string, PlaidAccountDataModel> group in accountGroups)
            {
                string accessToken = group.Key;
                string[] accountIds = group
                    .Where(account => !account.IsPlaidRelogRequired)
                    .Select(account => account.PlaidAccountId)
                    .ToArray();

                DatabaseResponseModel response = await PlaidSync(accessToken, accountIds);
                if (!response.Success)
                    return response;

                DatabaseResponseModel transactionDatabaseResponse = await SaveNewPlaidTransactions(
                    accessToken,
                    accountIds,
                    userId);
            }

            return DatabaseResponseModel.CreateSuccess();
        }

        /// <summary>
        /// Retrieves balances and transactions from plaid, then saves
        /// to database.
        /// </summary>
        /// <param name="requestModel">
        /// <see cref="TransactionUpdateRequestModel"/> request with account Ids
        /// to refresh and access token.
        /// </param>
        /// <returns>
        /// <see cref="DatabaseResponseModel"/> indicating success or failure.
        /// </returns>
        private async Task<DatabaseResponseModel> PlaidSyncAsync(
            TransactionUpdateRequestModel requestModel,
            string userAccountId)
        {
            TransactionUpdateResponseModel plaidUpdateResponse = await _plaidSyncAccessor
                .GetPlaidTransactionsAsync(requestModel);

            if (!string.IsNullOrWhiteSpace(plaidUpdateResponse.ErrorMessage))
                return new()
                {
                    ErrorMessage = plaidUpdateResponse.ErrorMessage
                };

            if (plaidUpdateResponse.Accounts is null
                || !plaidUpdateResponse.Accounts.Any())
            {
                return new()
                {
                    ErrorMessage = "Error contacting plaid."
                };
            }

            IEnumerable<PlaidAccount> accounts = plaidUpdateResponse.Accounts
                .Select(account => new PlaidAccount()
                {
                    AccessToken = requestModel.AccessToken!,
                    AvailableBalance = account.Balances!.Available,
                    CurrentBalance = account.Balances.Current,
                    CreditLimit = account.Balances.Limit,
                    PlaidAccountId = account.AccountId!
                });

            DatabaseResponseModel accountUpdateResponse = await _plaidAccountAccessor.UpdateAccountBalancesAsync(accounts);

            if (!accountUpdateResponse.Success
                || plaidUpdateResponse.Transactions is null
                || !plaidUpdateResponse.Transactions.Any())
                return accountUpdateResponse;

            IEnumerable<PlaidTransaction> plaidTransactions = plaidUpdateResponse.Transactions
                .Select(transaction => new PlaidTransaction
                {
                    AccountOwner = transaction.AccountOwner,
                    Amount = transaction.Amount.HasValue
                ? Convert.ToDecimal(transaction.Amount)
                : 0M,
                    AuthorizedDate = transaction.AuthorizedDateTime,
                    CategoryId = transaction.CategoryId,
                    Address = transaction.Location?.Address,
                    City = transaction.Location?.City,
                    PostalCode = transaction.Location?.PostalCode,
                    Country = transaction.Location?.Country,
                    Region = transaction.Location?.Region,
                    Latitude = transaction.Location?.Latitude,
                    Longitude = transaction.Location?.Longitude,
                    StoreNumber = transaction.Location?.StoreNumber,
                    CurrencyCode = transaction.IsoCurrencyCode ?? string.Empty,
                    DetailedCategory = transaction.PersonalFinanceCategory?.Detailed,
                    PrimaryCategory = transaction.PersonalFinanceCategory?.Primary,
                    PlaidAccountId = transaction.AccountId,
                    PlaidTransactionId = transaction.TransactionId,
                    UserAccountId = userAccountId,
                    IsPending = transaction.IsPending,
                    PendingTransactionId = transaction.PendingTransactionId,
                    MerchantName = transaction.MerchantName,
                    Name = transaction.Name,
                    Payee = transaction.PaymentMeta?.Payee,
                    Payer = transaction.PaymentMeta?.Payer,
                    PaymentMethod = transaction.PaymentMeta?.PaymentMethod,
                    PaymentProcessor = transaction.PaymentMeta?.PaymentProcessor,
                    ByOrderOf = transaction.PaymentMeta?.ByOrderOf,
                    PaymentChannel = transaction.PaymentChannel,
                    PpdId = transaction.PaymentMeta?.PpdId,
                    Reason = transaction.PaymentMeta?.Reason,
                    ReferenceNumber = transaction.PaymentMeta?.ReferenceNumber,
                    SpendingFromGoalId = null,
                    TransactionCode = transaction.TransactionCode,
                    TransactionDate = transaction.Date ?? DateTime.UtcNow,
                    TransactionType = transaction.TransactionType
                });

            DatabaseResponseModel transactionUpsertResponse = await _plaidAccountAccessor
                .UpsertPlaidTransactionsAsync(plaidTransactions);

            return transactionUpsertResponse;
        }

        private async Task<IEnumerable<IGrouping<string, PlaidAccount>>?> GetAccountTokenGroups(string userId)
        {
            IEnumerable<PlaidAccount> plaidAccounts = await _plaidAccountAccessor.GetPlaidAccountsByUserIdAsync(userId);
            if (plaidAccounts is null || !plaidAccounts.Any())
                return null;

            IEnumerable<
                IGrouping<string, PlaidAccount>> accountGroups = plaidAccounts.GroupBy(account => account.AccessToken);

            return accountGroups;
        }

        private async Task<DatabaseResponseModel> PlaidSync(TransactionUpdateRequestModel requestModel)
        {
            TransactionUpdateResponseModel plaidUpdateResponse = await _plaidSyncAccessor
                .GetPlaidTransactionsAsync(requestModel);

            if (plaidUpdateResponse is null)
                return DatabaseResponseModel.CreateError("Something went wrong while contacting plaid.");

            //if (plaidUpdateResponse.ErrorCode == PlaidErrorCodes.ITEM_LOGIN_REQUIRED.ToString())
            //{
            //    DatabaseResponseModel lockResponse = await _plaidAccountAccessor.UpdateAccountRelogAsync(true, requestModel.);
            //    return lockResponse;
            //}

            //DatabaseResponseModel response = await _accountAccessor.UpdateAccountBalancesAsync(plaidUpdateResponse);
            //return response;
            return new();
        }

        private async Task<DatabaseResponseModel> SaveNewPlaidTransactions(
            string accessToken,
            string[] accountIds,
            string userAccountId)
        {
            TransactionUpdateRequestModel requestModel = new(
                accessToken,
                accountIds);

            TransactionUpdateResponseModel? transactionResponseModel = await _plaidSyncAccessor
                .GetPlaidTransactionsAsync(requestModel);

            if (transactionResponseModel is null
                || transactionResponseModel.Accounts is null
                || transactionResponseModel.Transactions is null)
                return DatabaseResponseModel.CreateError("Something went wrong while contacting plaid.");

            if (transactionResponseModel.ErrorCode == PlaidErrorCodes.ITEM_LOGIN_REQUIRED.ToString())
            {
                DatabaseResponseModel lockResponse = await _accountAccessor.UpdateAccountRelogAsync(true, accountIds);
                return lockResponse;
            }

            IEnumerable<TransactionDataModel> transactions = transactionResponseModel.Accounts
                .Join(transactionResponseModel.Transactions,
                    a => a.AccountId,
                    t => t.AccountId,
                    (account, transaction) => new TransactionDataModel
                    {
                        AccountOwner = transaction.AccountOwner,
                        Amount = transaction.Amount.HasValue
                            ? Convert.ToDecimal(transaction.Amount)
                            : 0M,
                        AuthorizedDate = transaction.AuthorizedDateTime,
                        CategoryId = transaction.CategoryId,
                        Address = transaction.Location?.Address,
                        City = transaction.Location?.City,
                        PostalCode = transaction.Location?.PostalCode,
                        Country = transaction.Location?.Country,
                        Region = transaction.Location?.Region,
                        Latitude = transaction.Location?.Latitude,
                        Longitude = transaction.Location?.Longitude,
                        StoreNumber = transaction.Location?.StoreNumber,
                        CurrencyCode = transaction.IsoCurrencyCode ?? string.Empty,
                        DetailedCategory = transaction.PersonalFinanceCategory?.Detailed,
                        PrimaryCategory = transaction.PersonalFinanceCategory?.Primary,
                        PlaidAccountId = transaction.AccountId,
                        PlaidTransactionId = transaction.TransactionId,
                        UserAccountId = userAccountId,
                        IsPending = transaction.IsPending,
                        PendingTransactionId = transaction.PendingTransactionId,
                        MerchantName = transaction.MerchantName,
                        Name = transaction.Name,
                        Payee = transaction.PaymentMeta?.Payee,
                        Payer = transaction.PaymentMeta?.Payer,
                        PaymentMethod = transaction.PaymentMeta?.PaymentMethod,
                        PaymentProcessor = transaction.PaymentMeta?.PaymentProcessor,
                        ByOrderOf = transaction.PaymentMeta?.ByOrderOf,
                        PaymentChannel = transaction.PaymentChannel,
                        PpdId = transaction.PaymentMeta?.PpdId,
                        Reason = transaction.PaymentMeta?.Reason,
                        ReferenceNumber = transaction.PaymentMeta?.ReferenceNumber,
                        SpendingFromGoalId = null,
                        TransactionCode = transaction.TransactionCode,
                        TransactionDate = transaction.Date ?? DateTime.UtcNow,
                        TransactionType = transaction.TransactionType
                    });

            DatabaseResponseModel databaseResponseModel = await _accountAccessor.UpsertPlaidTransactionsAsync(transactions);
            return databaseResponseModel;
        }
    }
}
