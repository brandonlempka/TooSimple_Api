using TooSimple_Poco.Models.DataModels;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Poco.Models.Dtos.Goals
{
    public class GetGoalDto : BaseHttpResponse
	{
		public GoalDataModel? Goal { get; set; }
		public IEnumerable<FundingHistoryDataModel>? FundingHistory { get; set; }
		public IEnumerable<FundingScheduleDataModel>? FundingSchedules { get; set; }
	}
}
