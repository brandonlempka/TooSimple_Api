using TooSimple_Poco.Models.Database;

namespace TooSimple_Managers.Budgeting
{
    public interface IBudgetingManager
	{
		Task<IEnumerable<GoalDataModel>> GetGoalsByUserIdAsync(string userId);
		Task<decimal> GetUserReadyToSpendAsync(string userId);
        Task UpdateBudgetByUserId(string userId, DateTime? today = null);
    }
}

