using TooSimple_Poco.Models.Budgeting;
using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Managers.Goals
{
    public interface IGoalManager
    {
        Task<GetGoalsDto> GetGoalsByUserIdAsync(string userId);
        Task<GetGoalDto> GetGoalByGoalIdAsync(string goalId);
        Task<BaseHttpResponse> AddNewGoalAsync(GoalDataModel goalDataModel);
        Task<BaseHttpResponse> UpdateGoalAsync(GoalDataModel goalDataModel);
        Task<BaseHttpResponse> DeleteGoalAsync(string goalId);
    }
}
