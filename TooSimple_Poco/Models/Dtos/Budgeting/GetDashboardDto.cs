using TooSimple_Poco.Models.DataModels;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Poco.Models.Dtos.Budgeting
{
	public class GetDashboardDto : BaseHttpResponse
	{
		public decimal ReadyToSpend { get; set; }
		public decimal DepositoryAmount { get; set; }
		public decimal CreditAmount { get; set; }
		public decimal GoalAmount { get; set; }
		public decimal ExpenseAmount { get; set; }
		public DateTime LastUpdated { get; set; }
		public IEnumerable<TransactionDataModel>? Transactions { get; set; }
	}
}
