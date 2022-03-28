using TooSimple_Poco.Models.Database;

namespace TooSimple_DataAccessors.Database.Goals
{
    public interface IGoalAccessor
	{
		Task<IEnumerable<GoalDataModel>> GetGoalsByUserIdAsync(string userId);
        Task<GoalDataModel> GetGoalByGoalIdAsync(string goalId);
        Task<bool> UpdateGoalAsync(GoalDataModel goal);
        Task<IEnumerable<FundingHistoryDataModel>> GetFundingHistoryByGoalId(string goalId);
        Task<IEnumerable<FundingScheduleDataModel>> GetFundingSchedulesByUserId(string userId);
        Task<bool> SaveMoveMoneyAsync(FundingHistoryDataModel dataModel);
    }
}

