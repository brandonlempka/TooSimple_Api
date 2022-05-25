using TooSimple_Poco.Models.ApiRequestModels;
using TooSimple_Poco.Models.Dtos.Transactions;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Managers.Transactions
{
	public interface IPlaidTransactionManager
	{
		Task<GetTransactionsDto> SearchPlaidTransactionsAsync(GetTransactionsRequestModel requestModel);
		Task<BaseHttpResponse> UpdatePlaidTransactionAsync(UpdatePlaidTransactionRequestModel requestModel);
	}
}
