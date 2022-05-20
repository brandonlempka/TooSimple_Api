using TooSimple_Poco.Models.Dtos.Budgeting;

namespace TooSimple_Managers.Budgeting
{
    public interface IBudgetingManager
	{
        Task<GetDashboardDto> GetUserDashboardAsync(string userId);
    }
}
