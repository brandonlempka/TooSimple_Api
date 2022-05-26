using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TooSimple_Poco.Enums;
using TooSimple_Poco.Models.DataModels;

namespace TooSimple_UnitTests.Budgeting.Goals
{
	[TestClass]
	public class GoalDataModelTests
	{
		[DataTestMethod]
		[DataRow(true, false, false, false, "2022-01-01", ContributionSchedule.UNKNOWN, 0.00, "0001-01-01")]
		[DataRow(false, true, false, false, "2022-01-01", ContributionSchedule.UNKNOWN, 0.00, "0001-01-01")]
		[DataRow(false, false, false, false, "2000-01-01", ContributionSchedule.Weekly, 1, "0001-01-01")]
		[DataRow(false, false, false, false, "2022-01-01", ContributionSchedule.Weekly, 1.73, "2021-01-05")]
		[DataRow(false, false, true, false, "2022-01-01", ContributionSchedule.Weekly, 1.83, "2021-01-05")]
		[DataRow(false, false, false, true, "2022-01-01", ContributionSchedule.Weekly, 1.00, "2021-01-05")]
		[DataRow(false, false, false, false, "2022-01-01", ContributionSchedule.BiWeekly, 3.46, "2021-01-12")]
		[DataRow(false, false, true, false, "2022-01-01", ContributionSchedule.BiWeekly, 3.65, "2021-01-12")]
		[DataRow(false, false, false, true, "2022-01-01", ContributionSchedule.BiWeekly, 1.00, "2021-01-12")]
		[DataRow(false, false, false, false, "2022-01-01", ContributionSchedule.Monthly, 7.5, "2021-02-01")]
		[DataRow(false, false, true, false, "2022-01-01", ContributionSchedule.Monthly, 7.92, "2021-02-01")]
		[DataRow(false, false, false, true, "2022-01-01", ContributionSchedule.Monthly, 1.00, "2021-02-01")]
		[DataRow(false, false, false, false, "2022-01-01", ContributionSchedule.BiMonthly, 15, "2021-03-01")]
		[DataRow(false, false, true, false, "2022-01-01", ContributionSchedule.BiMonthly, 15.83, "2021-03-01")]
		[DataRow(false, false, false, true, "2022-01-01", ContributionSchedule.BiMonthly, 1.00, "2021-03-01")]
		[DataRow(false, false, false, false, "2022-01-01", ContributionSchedule.LastDayOfMonth, 7.5, "2021-01-31")]
		[DataRow(false, false, true, false, "2022-01-01", ContributionSchedule.LastDayOfMonth, 7.92, "2021-01-31")]
		[DataRow(false, false, false, true, "2022-01-01", ContributionSchedule.LastDayOfMonth, 1.00, "2021-01-31")]
		public void GoalContributionTests(
			bool isArchived,
			bool isPaused,
			bool isAutoRefillEnabled,
			bool isContributionFixed,
			string desiredCompletion,
			ContributionSchedule contributionSchedule,
			double expectedAmount,
			string expectedDate)
        {
			DateTime desiredCompletionDate = Convert.ToDateTime(desiredCompletion);
			DateTime firstContributionDate = contributionSchedule == ContributionSchedule.LastDayOfMonth
				? Convert.ToDateTime("2019-01-31")
				: Convert.ToDateTime("2019-01-01");

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
					FirstContributionDate = firstContributionDate,
					Frequency = (int)contributionSchedule,
					UserAccountId = "123",
					FundingScheduleId = "1",
					FundingScheduleName = "Tester"
				},
				NextContributionAmount = 1M,
				UserAccountId = "123",
				GoalId = "456",
				IsPaused = isPaused,
				DesiredCompletionDate = desiredCompletionDate
			};

			DateTime currentDate = Convert.ToDateTime("2021-01-01");
			DateTime expectedDateTime = Convert.ToDateTime(expectedDate);
			goalDataModel.GetNextContribution(currentDate);

			Assert.IsNotNull(goalDataModel);
			Assert.AreEqual(goalDataModel.NextContributionAmount, (decimal)expectedAmount);
			Assert.AreEqual(goalDataModel.NextContributionDate, expectedDateTime);
        }

		[DataTestMethod]
		[DataRow(true, false, "2022-01-01", ContributionSchedule.UNKNOWN, ExpenseFrequency.Weekly, 0.00, "0001-01-01")]
		[DataRow(false, true, "2022-01-01", ContributionSchedule.UNKNOWN, ExpenseFrequency.Monthly, 0.00, "0001-01-01")] 
		[DataRow(false, false, "2020-01-01", ContributionSchedule.Weekly, ExpenseFrequency.Weekly, 0, "2021-01-05")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.Weekly, ExpenseFrequency.BiWeekly, 90, "2021-01-05")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.Weekly, ExpenseFrequency.Monthly, 22.5, "2021-01-05")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.Weekly, ExpenseFrequency.BiMonthly, 11.25, "2021-01-05")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.Weekly, ExpenseFrequency.ThreeMonths, 7.5, "2021-01-05")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.Weekly, ExpenseFrequency.SixMonths, 3.6, "2021-01-05")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.Weekly, ExpenseFrequency.Yearly, 1.73, "2021-01-05")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.BiWeekly, ExpenseFrequency.Weekly, 0, "2021-01-12")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.BiWeekly, ExpenseFrequency.BiWeekly, 0, "2021-01-12")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.BiWeekly, ExpenseFrequency.Monthly, 45, "2021-01-12")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.BiWeekly, ExpenseFrequency.BiMonthly, 22.5, "2021-01-12")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.BiWeekly, ExpenseFrequency.ThreeMonths, 15, "2021-01-12")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.BiWeekly, ExpenseFrequency.SixMonths, 7.5, "2021-01-12")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.BiWeekly, ExpenseFrequency.Yearly, 3.46, "2021-01-12")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.Monthly, ExpenseFrequency.Weekly, 90, "2021-02-01")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.Monthly, ExpenseFrequency.BiWeekly, 90, "2021-02-01")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.Monthly, ExpenseFrequency.Monthly, 90, "2021-02-01")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.Monthly, ExpenseFrequency.BiMonthly, 45, "2021-02-01")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.Monthly, ExpenseFrequency.ThreeMonths, 30, "2021-02-01")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.Monthly, ExpenseFrequency.SixMonths, 15, "2021-02-01")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.Monthly, ExpenseFrequency.Yearly, 7.5, "2021-02-01")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.BiMonthly, ExpenseFrequency.Weekly, 90, "2021-03-01")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.BiMonthly, ExpenseFrequency.BiWeekly, 90, "2021-03-01")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.BiMonthly, ExpenseFrequency.Monthly, 90, "2021-03-01")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.BiMonthly, ExpenseFrequency.BiMonthly, 90, "2021-03-01")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.BiMonthly, ExpenseFrequency.ThreeMonths, 45, "2021-03-01")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.BiMonthly, ExpenseFrequency.SixMonths, 30, "2021-03-01")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.BiMonthly, ExpenseFrequency.Yearly, 15, "2021-03-01")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.LastDayOfMonth, ExpenseFrequency.Weekly, 90, "2021-01-31")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.LastDayOfMonth, ExpenseFrequency.BiWeekly, 90, "2021-01-31")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.LastDayOfMonth, ExpenseFrequency.Monthly, 90, "2021-01-31")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.LastDayOfMonth, ExpenseFrequency.BiMonthly, 45, "2021-01-31")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.LastDayOfMonth, ExpenseFrequency.ThreeMonths, 30, "2021-01-31")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.LastDayOfMonth, ExpenseFrequency.SixMonths, 15, "2021-01-31")]
		[DataRow(false, false, "2020-01-01", ContributionSchedule.LastDayOfMonth, ExpenseFrequency.Yearly, 7.5, "2021-01-31")]
		public void ExpenseContributionTests(
			bool isArchived,
			bool isPaused,
			string desiredCompletion,
			ContributionSchedule contributionSchedule,
			ExpenseFrequency expenseFrequency,
			double expectedAmount,
			string expectedDate)
		{
			DateTime desiredCompletionDate = Convert.ToDateTime(desiredCompletion);
			DateTime firstContributionDate = contributionSchedule == ContributionSchedule.LastDayOfMonth
				? Convert.ToDateTime("2019-01-31")
				: Convert.ToDateTime("2019-01-01");

			GoalDataModel goalDataModel = new()
			{
				AmountContributed = 10M,
				AmountSpent = 5M,
				GoalAmount = 100M,
				IsArchived = isArchived,
				IsAutoRefillEnabled = false,
				IsContributionFixed = false,
				FundingSchedule = new()
				{
					FirstContributionDate = firstContributionDate,
					Frequency = (int)contributionSchedule,
					UserAccountId = "123",
					FundingScheduleId = "1",
					FundingScheduleName = "Tester"
				},
				NextContributionAmount = 1M,
				UserAccountId = "123",
				GoalId = "456",
				IsPaused = isPaused,
				DesiredCompletionDate = desiredCompletionDate,
				IsExpense = true,
				RecurrenceTimeFrame = (int)expenseFrequency
			};

			DateTime currentDate = Convert.ToDateTime("2021-01-01");
			DateTime expectedDateTime = Convert.ToDateTime(expectedDate);
			goalDataModel.GetNextContribution(currentDate);

			Assert.IsNotNull(goalDataModel);
			Assert.AreEqual(goalDataModel.NextContributionAmount, (decimal)expectedAmount);
			Assert.AreEqual(goalDataModel.NextContributionDate, expectedDateTime);
		}
	}
}
