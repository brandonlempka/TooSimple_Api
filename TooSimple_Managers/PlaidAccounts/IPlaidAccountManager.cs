using TooSimple_Poco.Models.Dtos.PlaidAccounts;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Managers.PlaidAccounts
{
    public interface IPlaidAccountManager
    {
        Task<GetPlaidAccountsDto> GetPlaidAccountsByUserIdAsync(string userId);
        Task<BaseHttpResponse> DeleteAccountAsync(string accountId);
    }
}
