using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.Plaid.AccountUpdate;

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
        /// <returns><see cref="PlaidAccountDataModel"/>Enumerable of account data.</returns>
        public async Task<IEnumerable<PlaidAccountDataModel>> GetPlaidAccountsByUserIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                userId = "1d4c76c2-148b-47b5-9a53-c29f3a233c80";

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
        /// <returns><see cref="PlaidAccountDataModel"/>Enumerable of account data.</returns>
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

        public async Task<bool> UpdateAccountBalancesAsync(AccountUpdateResponseModel responseModel)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using (IDbTransaction transaction = connection.BeginTransaction())
            {
                string query = @"UPDATE PlaidAccounts
                        SET CurrentBalance = @CurrentBalance
                        , AvailableBalance = @AvailableBalance
                        , LastUpdated = @Now
                        WHERE PlaidAccountId = @Id";

                try
                {
                    foreach (AccountResponseModel? account in responseModel.Accounts)
                    {

                        await connection.ExecuteAsync(
                            query,
                            new
                            {
                                CurrentBalance = account.Balances.Current,
                                AvailableBalance = account.Balances.Available,
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
                    return false;
                }
            }

            return true;
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
        /// Boolean indicating success.
        /// </returns>
        public async Task<bool> UpdateAccountRelogAsync(bool isLocked, string[] accountIds)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
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
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
