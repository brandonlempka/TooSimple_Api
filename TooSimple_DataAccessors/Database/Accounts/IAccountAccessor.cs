using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.Plaid.AccountUpdate;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_DataAccessors.Database.Accounts
{
    public interface IAccountAccessor
    {
        Task<IEnumerable<PlaidAccountDataModel>> GetPlaidAccountsByUserIdAsync(string userId);
        Task<PlaidAccountDataModel> GetPlaidAccountsByItemIdAsync(string itemId);
        Task<DatabaseResponseModel> UpdateAccountBalancesAsync(AccountUpdateResponseModel responseModel);
        Task<DatabaseResponseModel> UpdateAccountRelogAsync(bool isLocked, string[] accountIds);
        Task<DatabaseResponseModel> UpsertPlaidTransactionsAsync(IEnumerable<TransactionDataModel> responseModels);
    }
}
