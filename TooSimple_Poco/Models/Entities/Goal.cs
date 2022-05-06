using TooSimple_Poco.Models.DataModels;

namespace TooSimple_Poco.Models.Entities
{
    public class Goal
    {
        public string GoalId { get; set; } = string.Empty;
        public string GoalName { get; set; } = string.Empty;
        public decimal GoalAmount { get; set; }
        public DateTime DesiredCompletionDate { get; set; }
        public string UserAccountId { get; set; } = string.Empty;
        public string FundingScheduleId { get; set; } = string.Empty;
        public bool IsExpense { get; set; }
        public int RecurrenceTimeFrame { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsPaused { get; set; }
        public string AutoSpendMerchantName { get; set; } = string.Empty;
        public decimal AmountContributed { get; set; }
        public decimal AmountSpent { get; set; }
        public bool IsAutoRefillEnabled { get; set; }
        public decimal NextContributionAmount { get; set; }
        public DateTime NextContributionDate { get; set; }
        public bool IsContributionFixed { get; set; }
        public bool IsArchived { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Goal() { }

        /// <summary>
        /// Constructor for converting from <see cref="Goal"/> entity to data model.
        /// </summary>
        /// <param name="goal">
        /// <see cref="Goal"/> entity from database.
        /// </param>
        public Goal(GoalDataModel goalDataModel)
        {
            GoalId = goalDataModel.GoalId;
            GoalName = goalDataModel.GoalName;
            GoalAmount = goalDataModel.GoalAmount;
            DesiredCompletionDate = goalDataModel.DesiredCompletionDate;
            UserAccountId = goalDataModel.UserAccountId;
            FundingScheduleId = goalDataModel.FundingScheduleId;
            IsExpense = goalDataModel.IsExpense;
            RecurrenceTimeFrame = goalDataModel.RecurrenceTimeFrame;
            CreationDate = goalDataModel.CreationDate;
            IsPaused = goalDataModel.IsPaused;
            AutoSpendMerchantName = goalDataModel.AutoSpendMerchantName;
            AmountContributed = goalDataModel.AmountContributed;
            AmountSpent = goalDataModel.AmountSpent;
            IsAutoRefillEnabled = goalDataModel.IsAutoRefillEnabled;
            NextContributionAmount = goalDataModel.NextContributionAmount;
            NextContributionDate = goalDataModel.NextContributionDate;
            IsContributionFixed = goalDataModel.IsContributionFixed;
            IsArchived = goalDataModel.IsArchived;
        }
    }
}
