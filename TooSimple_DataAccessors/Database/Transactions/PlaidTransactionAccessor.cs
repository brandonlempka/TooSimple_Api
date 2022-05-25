using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using TooSimple_Poco.Models.ApiRequestModels;
using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_DataAccessors.Database.Transactions
{
	public class PlaidTransactionAccessor : IPlaidTransactionAccessor
    {
        private readonly string _connectionString;

        public PlaidTransactionAccessor(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TooSimpleMySql");
        }

        /// <summary>
        /// Returns transactions from database based on search criteria.
        /// </summary>
        /// <param name="requestModel">
        /// <see cref="GetTransactionsRequestModel"/> request model with optional
        /// search criteria. Only user ID is required.
        /// </param>
        /// <returns>
        /// Enumerable of <see cref="PlaidTransaction"/> from database.
        /// </returns>
        public async Task<IEnumerable<PlaidTransaction>> GetPlaidTransactionsByUserIdAsync(GetTransactionsRequestModel requestModel)
        {
            List<string> whereClauseList = new();

            if (!string.IsNullOrWhiteSpace(requestModel.AccountIdFilter))
                whereClauseList.Add("PlaidAccountId = @accountId");

            if (!string.IsNullOrWhiteSpace(requestModel.SearchTerm))
                whereClauseList.Add("(UPPER(MerchantName) LIKE @searchTerm OR UPPER(Name) LIKE @searchTerm)");

            if (requestModel.StartDate.HasValue)
                whereClauseList.Add("TransactionDate >= @startDate");

            if (requestModel.EndDate.HasValue)
                whereClauseList.Add("TransactionDate <= @endDate");

            if (!string.IsNullOrWhiteSpace(requestModel.PrimaryCategoryFilter))
                whereClauseList.Add("PrimaryCategory = @primaryCategory");

            if (!string.IsNullOrWhiteSpace(requestModel.DetailedCategoryFilter))
                whereClauseList.Add("DetailedCategory = @detailedCategory");


            string whereClause = string.Join(" AND ", whereClauseList);

            using MySqlConnection connection = new(_connectionString);
            await connection.OpenAsync();
            string query = @"SELECT
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
                            FROM PlaidTransactions
                            WHERE UserAccountId = @userId";

            if (whereClauseList.Any())
            {
                query += $" and {whereClause}";
            }

            query += " ORDER BY TransactionDate DESC";

            IEnumerable<PlaidTransaction> transactions = await connection.QueryAsync<PlaidTransaction>(
                query
                , new
                {
                    userId = requestModel.UserId,
                    accountId = requestModel.AccountIdFilter,
                    searchTerm = !string.IsNullOrWhiteSpace(requestModel.SearchTerm)
                        ? $"%{requestModel.SearchTerm.ToUpper()}%"
                        : null,
                    startDate = requestModel.StartDate,
                    endDate = requestModel.EndDate
                });

            return transactions;
        }

        /// <summary>
        /// Updates a transaction spent from goal.
        /// </summary>
        /// <param name="requestModel">
        /// <see cref="UpdatePlaidTransactionRequestModel"/> request model with
        /// plaid transaction Id and new goal Id if any.
        /// </param>
        /// <returns>
        /// <see cref="DatabaseResponseModel"/> indicating success or failure.
        /// </returns>
        public async Task<DatabaseResponseModel> UpdatePlaidTransactionAsync(UpdatePlaidTransactionRequestModel requestModel)
        {
            using MySqlConnection connection = new(_connectionString);
            await connection.OpenAsync();
            using IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                string query = @"UPDATE PlaidTransactions 
                                SET SpendingFromGoalId = @SpendingFromGoalId
                                WHERE PlaidTransactionId = @PlaidTransactionId";

                await connection.ExecuteAsync(
                    query,
                    new
                    {
                        requestModel.SpendingFromGoalId,
                        requestModel.PlaidTransactionId
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
