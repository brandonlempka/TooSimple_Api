using System.Text.Json;

namespace TooSimple_Managers.Plaid.AccountUpdate
{
    public interface IAccountUpdateManager
    {
        Task<bool> UpdateAccountBalancesByUserIdAsync(string userId);
        Task<bool> UpdateAccountBalancesByItemIdAsync(JsonElement json);
    }
}
