using TooSimple_Poco.Models.ApiRequestModels;
using TooSimple_Poco.Models.Database;

namespace TooSimple_DataAccessors.Database.Transactions
{
	public interface ITransactionsAccessor
	{
		Task<IEnumerable<PlaidTransaction>> GetPlaidTransactionsByUserIdAsync(GetTransactionsRequestModel requestModel);
	}
}
