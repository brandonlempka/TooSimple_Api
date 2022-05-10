using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using TooSimple_Poco.Models.ApiRequestModels;
using TooSimple_Poco.Models.Database;

namespace TooSimple_DataAccessors.Database.Transactions
{
	public class TransactionsAccessor : ITransactionsAccessor
    {
        private readonly string _connectionString;

        public TransactionsAccessor(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TooSimpleMySql");
        }

        public async Task<IEnumerable<PlaidTransaction>> GetPlaidTransactionsByUserIdAsync(GetTransactionsRequestModel requestModel)
        {
            List<string> whereClauseList = new();
            IEnumerable<PlaidTransaction> transactions = Enumerable.Empty<PlaidTransaction>();

            if (!string.IsNullOrWhiteSpace(requestModel.AccountIdFilter))
                whereClauseList.Add("PlaidAccountId = @accountId");

            if (!string.IsNullOrWhiteSpace(requestModel.SearchTerm))
                whereClauseList.Add("(MerchantName LIKE @searchTerm OR Name LIKE @searchTerm)");

            if (requestModel.StartDate.HasValue)
                whereClauseList.Add("TransactionDate >= @startDate");

            if (requestModel.EndDate.HasValue)
                whereClauseList.Add("TransactionDate <= @endDate");

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
                            WHERE UserId = @userId";

            if (whereClauseList.Any())
            {
                query += $" and {whereClause}";
            }

            transactions = await connection.QueryAsync<PlaidTransaction>(
                query
                , new
                {
                    userId = requestModel.UserId,
                    accountId = requestModel.AccountIdFilter,
                    searchTerm = requestModel.SearchTerm,
                    startDate = requestModel.StartDate,
                    endDate = requestModel.EndDate
                });

            return transactions;
        }
	}
}
