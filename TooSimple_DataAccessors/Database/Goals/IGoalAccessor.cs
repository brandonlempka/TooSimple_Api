using TooSimple_Poco.Models.Entities;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_DataAccessors.Database.Goals
{
    public interface IGoalAccessor
	{
		Task<IEnumerable<Goal>> GetGoalsByUserIdAsync(string userId);
        Task<Goal?> GetGoalByGoalIdAsync(string goalId);
        Task<DatabaseResponseModel> AddNewGoalAsync(Goal goalDataModel);
        Task<DatabaseResponseModel> UpdateGoalAsync(Goal goal);
        Task<DatabaseResponseModel> DeleteGoalAsync(string goalId);
        Task<IEnumerable<FundingHistory>> GetFundingHistoryByGoalId(string goalId);
    }
}

