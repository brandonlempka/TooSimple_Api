using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.Entities;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_DataAccessors.Database.PlaidAccounts
{
	public class PlaidAccountAccessor : IPlaidAccountAccessor
	{
        private readonly string _connectionString;

        public PlaidAccountAccessor(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TooSimpleMySql");
        }

        /// <summary>
        /// Returns all Plaid Accounts associated with user.
        /// </summary>
        /// <param name="userId">Too Simple user Id.</param>
        /// <returns><see cref="PlaidAccount"/>
        /// Enumerable of account data.
        /// </returns>
        public async Task<IEnumerable<PlaidAccount>> GetPlaidAccountsByUserIdAsync(string userId)
        {
            IEnumerable<PlaidAccount> plaidAccounts;
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

                plaidAccounts = await connection.QueryAsync<PlaidAccount>(
                    query, new { UserId = userId });
            }

            return plaidAccounts;
        }

        /// <summary>
        /// Return Plaid Accounts associated with item id.
        /// </summary>
        /// <param name="userId">Too Simple user Id.</param>
        /// <returns>
        /// <see cref="PlaidAccount"/>Enumerable of account data.
        /// </returns>
        public async Task<IEnumerable<PlaidAccount>> GetPlaidAccountsByItemIdAsync(string itemId)
        {
            IEnumerable<PlaidAccount> plaidAccount;
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

                plaidAccount = await connection.QueryAsync<PlaidAccount>(
                    query, new { ItemId = itemId });
            }

            return plaidAccount;
        }

        /// <summary>
        /// Adds new plaid account to database.
        /// </summary>
        /// <param name="plaidAccount">
        /// <see cref="PlaidAccount"/>
        /// Plaid Account from plaid.
        /// </param>
        /// <returns>
        /// <see cref="DatabaseResponseModel"/> with success or any error messages.
        /// </returns>
        public async Task<DatabaseResponseModel> InsertNewAccountAsync(PlaidAccount plaidAccount)
        {
            using MySqlConnection connection = new(_connectionString);
            await connection.OpenAsync();
            using IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                string query = @"INSERT INTO Dev_TooSimple.PlaidAccounts
                                (
                                    PlaidAccountId
                                    , PlaidAccountTypeId
                                    , UserAccountId
                                    , Mask
                                    , Name
                                    , NickName
                                    , CurrentBalance
                                    , AvailableBalance
                                    , CurrencyCode
                                    , AccessToken
                                    , LastUpdated
                                    , IsActiveForBudgetingFeatures
                                    , IsPlaidRelogRequired
                                    , ItemId
                                    , CreditLimit
                                )
                                VALUES
                                (
                                    @PlaidAccountId
                                    , @PlaidAccountTypeId
                                    , @UserAccountId
                                    , @Mask
                                    , @Name
                                    , @NickName
                                    , @CurrentBalance
                                    , @AvailableBalance
                                    , @CurrencyCode
                                    , @AccessToken
                                    , @LastUpdated
                                    , @IsActiveForBudgetingFeatures
                                    , @IsPlaidRelogRequired
                                    , @ItemId
                                    , @CreditLimit
                                )";

                await connection.ExecuteAsync(
                    query,
                    new
                    {
                        plaidAccount.PlaidAccountId,
                        plaidAccount.PlaidAccountTypeId,
                        plaidAccount.UserAccountId,
                        plaidAccount.Mask,
                        plaidAccount.Name,
                        plaidAccount.NickName,
                        plaidAccount.CurrentBalance,
                        plaidAccount.AvailableBalance,
                        plaidAccount.CurrencyCode,
                        plaidAccount.AccessToken,
                        plaidAccount.LastUpdated,
                        plaidAccount.IsActiveForBudgetingFeatures,
                        plaidAccount.IsPlaidRelogRequired,
                        plaidAccount.ItemId,
                        plaidAccount.CreditLimit
                    },
                    transaction);

                transaction.Commit();
                return DatabaseResponseModel.CreateSuccess();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return DatabaseResponseModel.CreateError(ex);
            }
        }

        /// <summary>
        /// Deletes a plaid account from transactions and accounts tables.
        /// </summary>
        /// <param name="accountId">
        /// Account Id to delete.
        /// </param>
        /// <returns>
        /// <see cref="DatabaseResponseModel"/> with success or any error messages.
        /// </returns>
        public async Task<DatabaseResponseModel> DeleteAccountAsync(string accountId)
        {
            using MySqlConnection connection = new(_connectionString);
            await connection.OpenAsync();
            using IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                string deleteTransactionsSql = @"DELETE 
                                            FROM PlaidTransactions 
                                            WHERE PlaidAccountId = @accountId";

                string deleteAccountSql = @"DELETE 
                                            FROM PlaidAccounts 
                                            WHERE PlaidAccountId = @accountId";

                await connection.ExecuteAsync(
                    deleteTransactionsSql,
                    new
                    {
                        accountId
                    },
                    transaction);

                await connection.ExecuteAsync(
                    deleteAccountSql,
                    new
                    {
                        accountId
                    },
                    transaction);

                transaction.Commit();
                return DatabaseResponseModel.CreateSuccess();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return DatabaseResponseModel.CreateError(ex);
            }
        }

        /// <summary>
        /// Updates TooSimple database with latest plaid account balances.
        /// </summary>
        /// <param name="accounts"> IEnumerable of <see cref="PlaidAccount"/>
        /// with new balances from plaid.
        /// </param>
        /// <returns>
        /// <see cref="DatabaseResponseModel"/> indicating success or failure.
        /// </returns>
        public async Task<DatabaseResponseModel> UpdateAccountBalancesAsync(
            IEnumerable<PlaidAccount> accounts)
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
                    foreach (PlaidAccount account in accounts)
                    {
                        await connection.ExecuteAsync(
                            query,
                            new
                            {
                                account.CurrentBalance,
                                account.AvailableBalance,
                                Limit = account.CreditLimit,
                                Now = DateTime.UtcNow,
                                Id = account.PlaidAccountId
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
        public async Task<DatabaseResponseModel> UpdateAccountRelogAsync(
            bool isLocked,
            string[] accountIds)
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
        /// IEnumerable of <see cref="PlaidTransaction"/>.
        /// </param>
        /// <returns>
        /// <see cref="DatabaseResponseModel"/> indicating success or failure.
        /// </returns>
        public async Task<DatabaseResponseModel> UpsertPlaidTransactionsAsync(
            IEnumerable<PlaidTransaction> transactions)
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
