using TooSimple_Poco.Models.ApiRequestModels;
using TooSimple_Poco.Models.Dtos.Transactions;

namespace TooSimple_Managers.Transactions
{
	public interface IPlaidTransactionManager
	{
		Task<GetTransactionsDto> SearchPlaidTransactionsAsync(GetTransactionsRequestModel requestModel);
	}
}
