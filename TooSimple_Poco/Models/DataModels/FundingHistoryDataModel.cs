using TooSimple_Poco.Models.Entities;

namespace TooSimple_Poco.Models.DataModels
{
    public class FundingHistoryDataModel
	{
        public string FundingHistoryId { get; set; } = string.Empty;
        public string SourceGoalId { get; set; } = string.Empty;
        public string? SourceGoalName { get; set; }
        public string DestinationGoalId { get; set; } = string.Empty;
        public string? DestinationGoalName { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransferDate { get; set; }
        public string TransferDateDisplay { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
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
                SourceGoalName = fundingHistory.SourceGoalName;
                DestinationGoalId = fundingHistory.DestinationGoalId;
                DestinationGoalName = fundingHistory.DestinationGoalName;
                Amount = fundingHistory.Amount;
                TransferDate = fundingHistory.TransferDate;
                TransferDateDisplay = fundingHistory.TransferDate.ToString("MM/dd/yyyy");
                Note = fundingHistory.Note;
                IsAutomatedTransfer = fundingHistory.IsAutomatedTransfer;
        }
    }
}
