using TooSimple_Poco.Models.DataModels;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Poco.Models.Dtos.PlaidAccounts
{
	public class GetPlaidAccountsDto : BaseHttpResponse
	{
		public IEnumerable<PlaidAccountDataModel>? PlaidAccounts { get; set; }
	}
}

