using TooSimple_Poco.Models.Shared;

namespace TooSimple_Managers.PlaidAccounts
{
    public interface IPlaidAccountManager
    {
        Task<BaseHttpResponse> DeleteAccountAsync(string accountId);
    }
}
