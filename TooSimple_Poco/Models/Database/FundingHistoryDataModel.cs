namespace TooSimple_Poco.Models.Database
{
    public class FundingHistoryDataModel
	{
        public string FundingHistoryId { get; set; } = string.Empty;
        public string SourceGoalId { get; set; } = string.Empty;
        public string DestinationGoalId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime TransferDate { get; set; }
        public string Note { get; set; } = string.Empty;
        public bool AutomatedTransfer { get; set; }
    }
}
