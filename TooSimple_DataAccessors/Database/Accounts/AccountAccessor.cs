using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.DataModels;
using TooSimple_Poco.Models.Plaid.AccountUpdate;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_DataAccessors.Database.Accounts
{
    public class AccountAccessor : IAccountAccessor
    {
        private readonly string _connectionString;

        public AccountAccessor(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TooSimpleMySql");
        }

        /// <summary>
        /// Returns all Plaid Accounts associated with user.
        /// </summary>
        /// <param name="userId">Too Simple user Id.</param>
        /// <returns><see cref="PlaidAccountDataModel"/>
        /// Enumerable of account data.
        /// </returns>
        public async Task<IEnumerable<PlaidAccountDataModel>> GetPlaidAccountsByUserIdAsync(string userId)
        {
            IEnumerable<PlaidAccountDataModel> plaidAccounts;
            using (MySqlConnection connection = new(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"SELECT 
                    PlaidAccountId
                    , PlaidAccountTypeId
                    , UserAccountId
                    , Mask
                    , Name
                    , NickName
                    , CurrentBalance
                    , AvailableBalance
                    , CreditLimit
                    , CurrencyCode
                    , AccessToken
                    , LastUpdated
                    , IsActiveForBudgetingFeatures
                    , IsPlaidRelogRequired
                    FROM PlaidAccounts
                    WHERE UserAccountId = @UserId";

                plaidAccounts = await connection.QueryAsync<PlaidAccountDataModel>(
                    query, new { UserId = userId });
            }

            return plaidAccounts;
        }

        /// <summary>
        /// Return Plaid Accounts associated with item id.
        /// </summary>
        /// <param name="userId">Too Simple user Id.</param>
        /// <returns>
        /// <see cref="PlaidAccountDataModel"/>Enumerable of account data.
        /// </returns>
        public async Task<PlaidAccountDataModel> GetPlaidAccountsByItemIdAsync(string itemId)
        {
            PlaidAccountDataModel plaidAccount;
            using (MySqlConnection connection = new(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"SELECT 

                    PlaidAccountId
                    , PlaidAccountTypeId
                    , UserAccountId
                    , Mask
                    , Name
                    , NickName
                    , CurrentBalance
                    , AvailableBalance
                    , CreditLimit
                    , CurrencyCode
                    , AccessToken
                    , LastUpdated
                    , IsActiveForBudgetingFeatures
                    , IsPlaidRelogRequired
                    FROM PlaidAccounts
                    WHERE ItemId = @ItemId";

                plaidAccount = await connection.QueryFirstOrDefaultAsync<PlaidAccountDataModel>(
                    query, new { ItemId = itemId });
            }

            return plaidAccount;
        }

        public async Task<DatabaseResponseModel> InsertNewAccountAsync()
        /// <summary>
        /// Updates TooSimple database with latest plaid account balances.
        /// </summary>
        /// <param name="responseModel">
        /// The return from plaid.
        /// </param>
        /// <returns>
        /// <see cref="PlaidAccountDataModel"/>Enumerable of account data.
        /// </returns>
        public async Task<DatabaseResponseModel> UpdateAccountBalancesAsync(AccountUpdateResponseModel responseModel)
        {
            using MySqlConnection connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using (IDbTransaction transaction = connection.BeginTransaction())
            {
                string query = @"UPDATE PlaidAccounts
                        SET CurrentBalance = @CurrentBalance
                        , AvailableBalance = @AvailableBalance
                        , CreditLimit = @Limit
                        , LastUpdated = @Now
                        WHERE PlaidAccountId = @Id";

                try
                {
                    if (responseModel.Accounts is null)
                    {
                        return DatabaseResponseModel.CreateError("Data from plaid was invalid.");
                    }

                    foreach (AccountResponseModel? account in responseModel.Accounts)
                    {
                        await connection.ExecuteAsync(
                            query,
                            new
                            {
                                CurrentBalance = account.Balances!.Current,
                                AvailableBalance = account.Balances!.Available,
                                Limit = account.Balances!.Limit,
                                Now = DateTime.UtcNow,
                                Id = account.AccountId
                            },
                            transaction);
                    }

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return DatabaseResponseModel.CreateError(ex);
                }
            }

            return DatabaseResponseModel.CreateSuccess();
        }

        /// <summary>
        /// Locks or unlocks Plaid Account if credentials have expired.
        /// </summary>
        /// <param name="isLocked">
        /// Boolean indicating whether to lock or unlock the account.
        /// True = lock, false = unlock.
        /// </param>
        /// <param name="accountIds">
        /// Account Ids to lock or unlock.
        /// </param>
        /// <returns>
        /// <see cref="DatabaseResponseModel"/>
        /// Database response indicating success or failure.
        /// </returns>
        public async Task<DatabaseResponseModel> UpdateAccountRelogAsync(bool isLocked, string[] accountIds)
        {
            using MySqlConnection connection = new(_connectionString);
            await connection.OpenAsync();
            using IDbTransaction transaction = connection.BeginTransaction();

            string query = @"UPDATE PlaidAccounts 
                                    SET IsPlaidRelogRequired = @IsLocked
                                    WHERE PlaidAccountId = @Id";

            try
            {
                await connection.ExecuteAsync(
                    query,
                    new
                    {
                        IsLocked = isLocked,
                        Id = accountIds
                    },
                    transaction);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return DatabaseResponseModel.CreateError(ex);
            }

            return DatabaseResponseModel.CreateSuccess();
        }

        /// <summary>
        /// Upserts new plaid transactions into local database.
        /// </summary>
        /// <param name="transactions">
        /// IEnumerable of transactions.
        /// </param>
        /// <returns>
        /// <see cref="DatabaseResponseModel"/> indicating success or failure.
        /// </returns>
        public async Task<DatabaseResponseModel> UpsertPlaidTransactionsAsync(
            IEnumerable<TransactionDataModel> transactions)
        {
            using MySqlConnection connection = new(_connectionString);
            await connection.OpenAsync();
            using IDbTransaction transaction = connection.BeginTransaction();

            string query = @"REPLACE INTO PlaidTransactions
                            (
                                PlaidTransactionId
                                , AccountOwner
                                , Amount
                                , AuthorizedDate
                                , TransactionDate
                                , CategoryId
                                , PrimaryCategory
                                , DetailedCategory
                                , CurrencyCode
                                , Address
                                , City
                                , Country
                                , Latitude
                                , Longitude
                                , PostalCode
                                , Region
                                , StoreNumber
                                , MerchantName
                                , Name
                                , PaymentChannel
                                , ByOrderOf
                                , Payee
                                , Payer
                                , PaymentMethod
                                , PaymentProcessor
                                , PpdId
                                , Reason
                                , ReferenceNumber
                                , IsPending
                                , PendingTransactionId
                                , TransactionCode
                                , TransactionType
                                , UnofficialCurrencyCode
                                , SpendingFromGoalId
                                , PlaidAccountId
                                , UserAccountId
                            )
                            VALUES
                            (
                                @PlaidTransactionId
                                , @AccountOwner
                                , @Amount
                                , @AuthorizedDate
                                , @TransactionDate
                                , @CategoryId
                                , @PrimaryCategory
                                , @DetailedCategory
                                , @CurrencyCode
                                , @Address
                                , @City
                                , @Country
                                , @Latitude
                                , @Longitude
                                , @PostalCode
                                , @Region
                                , @StoreNumber
                                , @MerchantName
                                , @Name
                                , @PaymentChannel
                                , @ByOrderOf
                                , @Payee
                                , @Payer
                                , @PaymentMethod
                                , @PaymentProcessor
                                , @PpdId
                                , @Reason
                                , @ReferenceNumber
                                , @IsPending
                                , @PendingTransactionId
                                , @TransactionCode
                                , @TransactionType
                                , @UnofficialCurrencyCode
                                , @SpendingFromGoalId
                                , @PlaidAccountId
                                , @UserAccountId
                                );";

            try
            {
                await connection.ExecuteAsync(
                    query,
                    transactions,
                    transaction);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return DatabaseResponseModel.CreateError(ex);
            }

            return DatabaseResponseModel.CreateSuccess();
        }
    }
}
