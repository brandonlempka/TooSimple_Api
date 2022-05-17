using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.Entities;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_DataAccessors.Database.PlaidAccounts
{
	public interface IPlaidAccountAccessor
	{
		Task<IEnumerable<PlaidAccount>> GetPlaidAccountsByUserIdAsync(string userId);
		Task<IEnumerable<PlaidAccount>> GetPlaidAccountsByItemIdAsync(string itemId);
		Task<DatabaseResponseModel> InsertNewAccountAsync(PlaidAccount plaidAccount);
		Task<DatabaseResponseModel> DeleteAccountAsync(string accountId);
		Task<DatabaseResponseModel> UpdateAccountBalancesAsync(IEnumerable<PlaidAccount> accounts);
		Task<DatabaseResponseModel> UpdateAccountRelogAsync(bool isLocked, string[] accountIds);
		Task<DatabaseResponseModel> UpsertPlaidTransactionsAsync(IEnumerable<PlaidTransaction> transactions);
	}
}
