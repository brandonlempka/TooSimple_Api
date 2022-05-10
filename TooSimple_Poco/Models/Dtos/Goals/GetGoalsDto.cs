using TooSimple_Poco.Models.DataModels;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Poco.Models.Dtos.Goals
{
    public class GetGoalsDto : BaseHttpResponse
	{
		public IEnumerable<GoalDataModel>? Goals { get; set; }
	}
}
