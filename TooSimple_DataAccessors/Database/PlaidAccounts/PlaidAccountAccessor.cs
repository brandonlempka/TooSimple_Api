using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using TooSimple_Poco.Models.DataModels;
using TooSimple_Poco.Models.Entities;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_DataAccessors.Database.PlaidAccounts
{
	public class PlaidAccountAccessor
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

        public async Task<DatabaseResponseModel> InsertNewAccountAsync(PlaidAccount plaidAccount)
        {

        }
    }
}
