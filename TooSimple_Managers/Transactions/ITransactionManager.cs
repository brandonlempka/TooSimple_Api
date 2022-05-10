using TooSimple_Poco.Models.ApiRequestModels;
using TooSimple_Poco.Models.Dtos.Transactions;

namespace TooSimple_Managers.Transactions
{
	public interface ITransactionManager
	{
		Task<GetTransactionsDto> GetTransactionsByUserIdAsync(GetTransactionsRequestModel requestModel);
	}
}
