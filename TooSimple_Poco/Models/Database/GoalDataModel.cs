namespace TooSimple_Poco.Models.Database
{
    public class GoalDataModel
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
	}
}

