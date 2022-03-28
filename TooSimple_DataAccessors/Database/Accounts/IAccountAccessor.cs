using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.Plaid.AccountUpdate;

namespace TooSimple_DataAccessors.Database.Accounts
{
    public interface IAccountAccessor
    {
        Task<IEnumerable<PlaidAccountDataModel>> GetPlaidAccountsAsync(string userId);
        Task<PlaidAccountDataModel> GetPlaidAccountByAccountIdAsync(string accountId);
        Task<bool> UpdateAccountBalancesAsync(AccountUpdateResponseModel responseModel);
        Task<bool> UpdateAccountRelogAsync(bool isLocked, string[] accountIds);
    }
}
