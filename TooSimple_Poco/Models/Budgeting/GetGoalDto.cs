using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Poco.Models.Budgeting
{
    public class GetGoalDto : BaseHttpResponse
	{
		public GoalDataModel? Goal { get; set; }
		public IEnumerable<FundingHistoryDataModel>? FundingHistory { get; set; }
	}
}

