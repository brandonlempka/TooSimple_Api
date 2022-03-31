using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_DataAccessors.Database.Goals
{
    public interface IGoalAccessor
	{
		Task<IEnumerable<GoalDataModel>> GetGoalsByUserIdAsync(string userId);
        Task<GoalDataModel?> GetGoalByGoalIdAsync(string goalId);
        Task<DatabaseResponseModel> AddNewGoalAsync(GoalDataModel goalDataModel);
        Task<DatabaseResponseModel> UpdateGoalAsync(GoalDataModel goal);
        Task<DatabaseResponseModel> DeleteGoalAsync(string goalId);
        Task<IEnumerable<FundingHistoryDataModel>> GetFundingHistoryByGoalId(string goalId);
        Task<IEnumerable<FundingScheduleDataModel>> GetFundingSchedulesByUserId(string userId);
        Task<bool> SaveMoveMoneyAsync(FundingHistoryDataModel dataModel);
    }
}

