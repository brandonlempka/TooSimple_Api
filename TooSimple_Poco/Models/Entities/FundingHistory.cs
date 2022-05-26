using TooSimple_Poco.Models.DataModels;

namespace TooSimple_Poco.Models.Entities
{
	public class FundingHistory
	{
        public string? FundingHistoryId { get; set; }
        public string? SourceGoalId { get; set; }
        public string SourceGoalName { get; set; } = "Ready to Spend";
        public string? DestinationGoalId { get; set; }
        public string DestinationGoalName { get; set; } = "Ready to Spend";
        public decimal Amount { get; set; }
        public DateTime TransferDate { get; set; }
        public string? Note { get; set; }
        public bool IsAutomatedTransfer { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FundingHistory()
        { }

        /// <summary>
        /// Constructor for converting <see cref="FundingHistoryDataModel"/>
        /// from database to <see cref="FundingHistory"/>
        /// </summary>
        /// <param name="fundingHistory">
        /// <see cref="FundingHistoryDataModel" /> data model with additional
        /// properties for client app.
        /// </param>
        public FundingHistory(FundingHistoryDataModel fundingHistory)
        {
            FundingHistoryId = fundingHistory.FundingHistoryId;
            SourceGoalId = fundingHistory.SourceGoalId;
            SourceGoalName = fundingHistory.SourceGoalName;
            DestinationGoalId = fundingHistory.DestinationGoalId;
            DestinationGoalName = fundingHistory.DestinationGoalName;
            Amount = fundingHistory.Amount;
            TransferDate = fundingHistory.TransferDate;
            Note = fundingHistory.Note;
            IsAutomatedTransfer = fundingHistory.IsAutomatedTransfer;
        }
    }
}
