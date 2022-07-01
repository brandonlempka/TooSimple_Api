using TooSimple_Poco.Models.ApiRequestModels;
using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_DataAccessors.Database.Transactions
{
	public interface IPlaidTransactionAccessor
	{
		Task<IEnumerable<PlaidTransaction>> GetPlaidTransactionsByUserIdAsync(GetTransactionsRequestModel requestModel);
		Task<PlaidTransaction?> GetPlaidTransactionByIdAsync(string plaidTransactionId);
        Task<DatabaseResponseModel> UpdatePlaidTransactionAsync(UpdatePlaidTransactionRequestModel requestModel);
	}
}
