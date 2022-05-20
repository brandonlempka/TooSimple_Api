using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using TooSimple_Poco.Models.ApiRequestModels;
using TooSimple_Poco.Models.Database;

namespace TooSimple_DataAccessors.Database.Transactions
{
	public class PlaidTransactionAccessor : IPlaidTransactionAccessor
    {
        private readonly string _connectionString;

        public PlaidTransactionAccessor(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TooSimpleMySql");
        }

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
	}
}
