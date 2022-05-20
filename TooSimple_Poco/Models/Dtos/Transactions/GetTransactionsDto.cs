using TooSimple_Poco.Models.DataModels;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Poco.Models.Dtos.Transactions
{
	public class GetTransactionsDto : BaseHttpResponse
	{
		public IEnumerable<TransactionDataModel> Transactions { get; set; } = Enumerable.Empty<TransactionDataModel>();
	}
}
