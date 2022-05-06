namespace TooSimple_Managers.Budgeting
{
    public interface IBudgetingManager
	{
        Task<decimal> GetUserReadyToSpendAsync(string userId);
    }
}
