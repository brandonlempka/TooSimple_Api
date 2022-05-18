using System.Net;
using System.Text.Json;
using TooSimple_DataAccessors.Database.Logging;
using TooSimple_DataAccessors.Database.PlaidAccounts;
using TooSimple_DataAccessors.Plaid.AccountUpdate;
using TooSimple_Managers.Budgeting;
using TooSimple_Poco.Enums;
using TooSimple_Poco.Models.Database;
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
        public async Task<BaseHttpResponse> PlaidSyncByUserIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new()
                {
                    ErrorMessage = "User Id was not found.",
                    Status = HttpStatusCode.BadRequest
                };
            }

            IEnumerable<IGrouping<string, PlaidAccount>>? accountGroups = await GetAccountTokenGroups(userId);

            if (accountGroups is null)
            {
                return new()
                {
                    ErrorMessage = "Something went wrong while retrieving accounts from database.",
                    Status = HttpStatusCode.InternalServerError
                };
            }

            foreach (IGrouping<string, PlaidAccount> group in accountGroups)
            {
                var test1 = group.Where(x => x.IsPlaidRelogRequired);
                var test2 = group.Select(x => x.PlaidAccountId);
                var test3 = test2.ToArray();

                string[] accountIds = group
                    .Where(account => !account.IsPlaidRelogRequired)
                    .Select(account => account.PlaidAccountId)
                    .ToArray();

                if (accountIds.Length == 0)
                {
                    break;
                }

                TransactionUpdateRequestModel requestModel = new(group.Key, accountIds);

                DatabaseResponseModel response = await PlaidSyncAsync(requestModel, userId);

                if (!response.Success)
                {
                    return BaseHttpResponse.CreateResponseFromDb(response);
                }
            }

            return BaseHttpResponse.CreateOkResponse();
        }

        /// <summary>
        /// Calls plaid to update account balances.
        /// </summary>
        /// <param name="json">
        /// Json webhook from plaid.
        /// </param>
        /// <returns>
        /// <see cref="DatabaseResponseModel"/>
        /// Database response indicating success or failure.
        /// </returns>
        public async Task<DatabaseResponseModel> PlaidSyncByItemIdAsync(JsonElement json)
        {
            PlaidWebhookResponseDto? webhookResponse = JsonSerializer.Deserialize<PlaidWebhookResponseDto>(json);

            if (webhookResponse is null)
            {
                string errorMessage = "Something went wrong while parsing: ";
                bool logResponse = await _loggingAccessor.LogMessageAsync(errorMessage + json.ToString());
                return DatabaseResponseModel.CreateError("Something went wrong while processing webhook.");
            }

            // Log anything that isn't a Transactions webhook or if the item ID doesn't come across.
            if (webhookResponse.WebhookType != PlaidWebhookType.TRANSACTIONS.ToString()
                || string.IsNullOrWhiteSpace(webhookResponse.ItemId))
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
                //todo future task to update budget when new transactions come in.
                //await _budgetingManager.UpdateBudgetByUserId(plaidAccount.UserAccountId);
            }

            return response;
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
            PlaidGetTransactionsResponseModel plaidUpdateResponse = await _plaidSyncAccessor
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
                    Amount = Convert.ToDecimal(transaction.Amount ?? 0),
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

        /// <summary>
        /// A plaid access token can be shared if multiple accounts were added at the
        /// same time at the same institution. This groups account Ids & access tokens
        /// to reduce api calls.
        /// </summary>
        /// <param name="userId">
        /// User Id to get accounts for.
        /// </param>
        /// <returns>
        /// Grouping of <see cref="PlaidAccount"/>s with access token as the key.
        /// </returns>
        private async Task<IEnumerable<IGrouping<string, PlaidAccount>>?> GetAccountTokenGroups(string userId)
        {
            IEnumerable<PlaidAccount> plaidAccounts = await _plaidAccountAccessor.GetPlaidAccountsByUserIdAsync(userId);
            if (plaidAccounts is null || !plaidAccounts.Any())
                return null;

            IEnumerable<
                IGrouping<string, PlaidAccount>> accountGroups = plaidAccounts.GroupBy(account => account.AccessToken);

            return accountGroups;
        }
    }
}
