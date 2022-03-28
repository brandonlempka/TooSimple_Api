using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.Plaid.AccountUpdate;

namespace TooSimple_DataAccessors.Database.Accounts
{
    public interface IAccountAccessor
    {
        Task<IEnumerable<PlaidAccountDataModel>> GetPlaidAccountsByUserIdAsync(string userId);
        Task<PlaidAccountDataModel> GetPlaidAccountsByItemIdAsync(string itemId);
        Task<bool> UpdateAccountBalancesAsync(AccountUpdateResponseModel responseModel);
        Task<bool> UpdateAccountRelogAsync(bool isLocked, string[] accountIds);
    }
}
