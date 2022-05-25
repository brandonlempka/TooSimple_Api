using TooSimple_Poco.Models.ApiRequestModels;
using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_DataAccessors.Database.Transactions
{
	public interface IPlaidTransactionAccessor
	{
		Task<IEnumerable<PlaidTransaction>> GetPlaidTransactionsByUserIdAsync(GetTransactionsRequestModel requestModel);
		Task<DatabaseResponseModel> UpdatePlaidTransactionAsync(UpdatePlaidTransactionRequestModel requestModel);
	}
}
