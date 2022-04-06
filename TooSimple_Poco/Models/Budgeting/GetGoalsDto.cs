using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Poco.Models.Budgeting
{
    public class GetGoalsDto : BaseHttpResponse
	{
		public IEnumerable<GoalDataModel>? Goals { get; set; }
	}
}

