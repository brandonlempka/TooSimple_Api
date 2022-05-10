using System.Text.Json.Serialization;

namespace TooSimple_Poco.Models.ApiRequestModels
{
	public class GetTransactionsRequestModel
	{
		[JsonPropertyName("userId")]
		public string UserId { get; set; } = string.Empty;

		[JsonPropertyName("startDate")]
		public DateTime? StartDate { get; set; }

		[JsonPropertyName("endDate")]
		public DateTime? EndDate { get; set; }

		[JsonPropertyName("searchTerm")]
		public string? SearchTerm { get; set; }

		[JsonPropertyName("accountIdFilter")]
		public string? AccountIdFilter { get; set; }
	}
}

