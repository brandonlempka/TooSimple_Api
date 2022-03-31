using TooSimple_Poco.Models.Budgeting;

namespace TooSimple_Managers.Budgeting
{
    public interface IBudgetingManager
	{
		Task<GetGoalsDto> GetGoalsByUserIdAsync(string userId);
        Task<GetGoalDto> GetGoalByGoalIdAsync(string goalId);
        Task<decimal> GetUserReadyToSpendAsync(string userId);
        Task UpdateBudgetByUserId(string userId, DateTime? today = null);
    }
}

