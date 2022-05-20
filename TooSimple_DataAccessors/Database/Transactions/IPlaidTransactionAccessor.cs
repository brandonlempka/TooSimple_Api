using TooSimple_Poco.Models.ApiRequestModels;
using TooSimple_Poco.Models.Database;

namespace TooSimple_DataAccessors.Database.Transactions
{
	public interface IPlaidTransactionAccessor
	{
		Task<IEnumerable<PlaidTransaction>> GetPlaidTransactionsByUserIdAsync(GetTransactionsRequestModel requestModel);
	}
}
