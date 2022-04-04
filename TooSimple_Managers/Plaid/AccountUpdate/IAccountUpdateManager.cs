using System.Text.Json;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Managers.Plaid.AccountUpdate
{
    public interface IAccountUpdateManager
    {
        Task<DatabaseResponseModel> UpdateAccountBalancesByUserIdAsync(string userId);
        Task<DatabaseResponseModel> UpdateAccountBalancesByItemIdAsync(JsonElement json);
        Task<DatabaseResponseModel> GetNewTransactionsAsync(string userId);
    }
}
