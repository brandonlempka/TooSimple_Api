using TooSimple_Poco.Models.Plaid.AccountUpdate;
using TooSimple_Poco.Models.Plaid.Transactions;

namespace TooSimple_DataAccessors.Plaid.AccountUpdate
{
    public interface IPlaidAccountUpdateAccessor
    {
        Task<AccountUpdateResponseModel> UpdateAccountBalancesAsync(AccountUpdateRequestModel requestModel);
        Task<TransactionUpdateResponseModel> GetPlaidTransactionsAsync(TransactionUpdateRequestModel requestModel);
    }
}
