namespace TooSimple_Managers.Plaid.AccountUpdate
{
    public interface IAccountUpdateManager
    {
        Task<bool> UpdateAccountBalancesAsync(string userId);
    }
}
