using TooSimple_Poco.Models.Plaid.AccountUpdate;

namespace TooSimple_DataAccessors.Plaid.AccountUpdate
{
    public interface IAccountUpdateAccessor
    {
        Task<AccountUpdateResponseModel> UpdateAccountBalancesAsync(
            AccountUpdateRequestModel requestModel);
    }
}
