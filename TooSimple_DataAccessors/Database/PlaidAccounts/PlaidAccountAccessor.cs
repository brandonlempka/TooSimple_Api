using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
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
        public async Task<PlaidAccount> GetPlaidAccountsByItemIdAsync(string itemId)
        {
            PlaidAccount plaidAccount;
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

                plaidAccount = await connection.QueryFirstOrDefaultAsync<PlaidAccount>(
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
    }
}
