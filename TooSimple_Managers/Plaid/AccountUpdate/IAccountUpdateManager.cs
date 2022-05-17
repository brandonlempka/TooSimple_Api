using System.Text.Json;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Managers.Plaid.AccountUpdate
{
    public interface IAccountUpdateManager
    {
        Task<BaseHttpResponse> PlaidSyncByUserIdAsync(string userId);
        Task<DatabaseResponseModel> PlaidSyncByItemIdAsync(JsonElement json);
    }
}
