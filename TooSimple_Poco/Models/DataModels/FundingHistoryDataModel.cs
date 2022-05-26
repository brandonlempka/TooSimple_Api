using TooSimple_Poco.Models.Entities;

namespace TooSimple_Poco.Models.DataModels
{
    public class FundingHistoryDataModel
    {
        public string? FundingHistoryId { get; set; }
        public string? SourceGoalId { get; set; }
        public string SourceGoalName { get; set; } = "Ready to Spend";
        public string? DestinationGoalId { get; set; }
        public string DestinationGoalName { get; set; } = "Ready to Spend";
        public decimal Amount { get; set; }
        public DateTime TransferDate { get; set; } = DateTime.UtcNow;
        public string? Note { get; set; }
        public bool IsAutomatedTransfer { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FundingHistoryDataModel()
        { }

        /// <summary>
        /// Constructor for converting <see cref="FundingHistory"/>
        /// from database to <see cref="FundingHistoryDataModel"/>
        /// </summary>
        /// <param name="fundingHistory">
        /// <see cref="FundingHistory"/> from database.
        /// </param>
        public FundingHistoryDataModel(FundingHistory fundingHistory)
        {
            FundingHistoryId = fundingHistory.FundingHistoryId;
            SourceGoalId = fundingHistory.SourceGoalId;
            DestinationGoalId = fundingHistory.DestinationGoalId;
            Amount = fundingHistory.Amount;
            TransferDate = fundingHistory.TransferDate;
            Note = fundingHistory.Note;
            IsAutomatedTransfer = fundingHistory.IsAutomatedTransfer;
        }
    }
}
