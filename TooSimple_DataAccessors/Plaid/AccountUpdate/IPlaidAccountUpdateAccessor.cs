using TooSimple_Poco.Models.Plaid.AccountUpdate;

namespace TooSimple_DataAccessors.Plaid.AccountUpdate
{
    public interface IPlaidAccountUpdateAccessor
    {
        Task<AccountUpdateResponseModel> UpdateAccountBalancesAsync(
            AccountUpdateRequestModel requestModel);
    }
}
