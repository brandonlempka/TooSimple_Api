
using TooSimple_Poco.Models.Entities;

namespace TooSimple_DataAccessors.Database.FundingSchedules
{
    public interface IFundingScheduleAccessor
	{
		Task<IEnumerable<FundingSchedule>> GetFundingSchedulesByUserIdAsync(string userId);

	}
}

