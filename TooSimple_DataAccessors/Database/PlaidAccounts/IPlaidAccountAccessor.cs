using TooSimple_Poco.Models.Entities;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_DataAccessors.Database.PlaidAccounts
{
	public interface IPlaidAccountAccessor
	{
		Task<IEnumerable<PlaidAccount>> GetPlaidAccountsByUserIdAsync(string userId);
		Task<PlaidAccount> GetPlaidAccountsByItemIdAsync(string itemId);
		Task<DatabaseResponseModel> InsertNewAccountAsync(PlaidAccount plaidAccount);
	}
}
