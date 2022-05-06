using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TooSimple_Poco.Enums;
using TooSimple_Poco.Models.DataModels;

namespace TooSimple_UnitTests.Budgeting.Goals
{
	[TestClass]
	public class GoalDataModelTests
	{
		[DataTestMethod]
		[DataRow(true, false, false, false, ContributionSchedule.UNKNOWN, 0.00, "1901-01-01")]
		[DataRow(false, true, false, false, ContributionSchedule.UNKNOWN, 0.00, "1901-01-01")]
		public async Task GoalContributionTests(
			bool isArchived,
			bool isPaused,
			bool isAutoRefillEnabled,
			bool isContributionFixed,
			ContributionSchedule contributionSchedule,
			decimal expectedAmount,
			string expectedDate)
        {
			GoalDataModel goalDataModel = new()
			{
				AmountContributed = 10M,
				AmountSpent = 5M,
				GoalAmount = 100M,
				IsArchived = isArchived,
				IsAutoRefillEnabled = isAutoRefillEnabled,
				IsContributionFixed = isContributionFixed,
				FundingSchedule = new()
				{
					FirstContributionDate = Convert.ToDateTime("2020-01-01"),
					Frequency = (int)contributionSchedule,
					UserAccountId = "123",
					FundingScheduleId = "1",
					FundingScheduleName = "Tester"
				},
				NextContributionAmount = 1M,
				UserAccountId = "123",
				GoalId = "456",
				IsPaused = isPaused
			};

			DateTime currentDate = Convert.ToDateTime("2021-01-01");
			DateTime expectedDateTime = Convert.ToDateTime(expectedDate);
			goalDataModel.GetNextContribution(currentDate);



			Assert.IsNotNull(goalDataModel);
			Assert.AreEqual(goalDataModel.NextContributionAmount, expectedAmount);
			Assert.AreEqual(goalDataModel.NextContributionDate, expectedDateTime);
        }
	}
}
