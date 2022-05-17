using TooSimple_Poco.Models.Plaid.AccountUpdate;
using TooSimple_Poco.Models.Plaid.Transactions;

namespace TooSimple_DataAccessors.Plaid.AccountUpdate
{
    public interface IPlaidSyncAccessor
    {
        Task<AccountUpdateResponseModel> UpdateAccountBalancesAsync(AccountUpdateRequestModel requestModel);
        Task<PlaidGetTransactionsResponseModel> GetPlaidTransactionsAsync(TransactionUpdateRequestModel requestModel);
    }
}
