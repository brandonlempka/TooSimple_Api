namespace TooSimple_Poco.Models.Entities
{
	public class FundingSchedule
	{
        public string FundingScheduleId { get; set; } = string.Empty;
        public string UserAccountId { get; set; } = string.Empty;
        public string FundingScheduleName { get; set; } = string.Empty;
        public int Frequency { get; set; }
        public DateTime FirstContributionDate { get; set; }
    }
}

