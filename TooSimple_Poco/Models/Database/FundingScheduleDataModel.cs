namespace TooSimple_Poco.Models.Database
{
    public class FundingScheduleDataModel
	{
        public string FundingScheduleId { get; set; } = string.Empty;
        public string UserAccountId { get; set; } = string.Empty;
        public string FundingScheduleName { get; set; } = string.Empty;
        public int Frequency { get; set; }
        public DateTime FirstContributionDate { get; set; }
    }
}

