using TooSimple_Poco.Models.DataModels;
using TooSimple_Poco.Models.Dtos.Budgeting;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Managers.Budgeting
{
    public interface IBudgetingManager
	{
        Task<GetDashboardDto> GetUserDashboardAsync(string userId);
        Task<BaseHttpResponse> MoveMoneyAsync(FundingHistoryDataModel fundingHistoryDataModel);
    }
}
