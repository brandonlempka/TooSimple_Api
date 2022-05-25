using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TooSimple_DataAccessors.Database.Goals;
using TooSimple_DataAccessors.Database.PlaidAccounts;
using TooSimple_DataAccessors.Database.Transactions;
using TooSimple_Managers.Budgeting;
using TooSimple_Poco.Enums;
using TooSimple_Poco.Models.ApiRequestModels;
using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.Dtos.Budgeting;
using TooSimple_Poco.Models.Entities;

namespace TooSimple_UnitTests.Budgeting.Dashboard
{
	[TestClass]
	public class GetDashboardTests
	{
		private List<PlaidAccount>? _plaidAccounts;
		private List<PlaidTransaction>? _plaidTransactions;
		private List<Goal>? _goals;

		[TestInitialize]
		public void Setup()
        {
			List<PlaidAccount> plaidAccounts = new()
			{
				new()
				{
					IsActiveForBudgetingFeatures = false,
					PlaidAccountTypeId = (int)PlaidAccountType.Checking,
					AvailableBalance = 100,
					LastUpdated = DateTime.Now.AddDays(-30)
				},
				new()
				{
					IsActiveForBudgetingFeatures = true,
					PlaidAccountTypeId = (int)PlaidAccountType.Savings,
					AvailableBalance = 100,
					LastUpdated = DateTime.Now.AddDays(-30)
				},
				new()
				{
					IsActiveForBudgetingFeatures = true,
					PlaidAccountTypeId = (int)PlaidAccountType.Checking,
					AvailableBalance = 100,
					Name = "Hi",
					PlaidAccountId = "123",
					LastUpdated = DateTime.Now.AddDays(-30)
				},
				new()
				{
					IsActiveForBudgetingFeatures = true,
					PlaidAccountTypeId = (int)PlaidAccountType.CreditCard,
					AvailableBalance = 50,
					CreditLimit = 100,
					LastUpdated = DateTime.Now.AddDays(-30)
				},
				new()
				{
					IsActiveForBudgetingFeatures = true,
					PlaidAccountTypeId = (int)PlaidAccountType.CreditCard,
					AvailableBalance = 50,
					CreditLimit = 100,
					LastUpdated = DateTime.Now.AddDays(-10)
				},
			};

			List<Goal> goals = new()
			{
				new()
				{
					AmountContributed = 100,
					AmountSpent = 50,
					IsArchived = true,
					IsExpense = false
				},
				new()
				{
					AmountContributed = 100,
					IsArchived = true,
					IsExpense = true
				},
				new()
				{
					AmountContributed = 100,
					AmountSpent = 50,
					IsArchived = false,
					IsExpense = false
				},
				new()
				{
					AmountContributed = 100,
					IsArchived = false,
					IsExpense = true
				},
			};

			List<PlaidTransaction> plaidTransactions = new()
			{
				new()
				{
					Amount = 123,
					Name = "hi",
					IsPending = false,
					TransactionDate = DateTime.Now,
					PlaidAccountId = "123"
				},
				new()
				{
					Amount = 123,
					Name = "hi",
					IsPending = false,
					TransactionDate = DateTime.Now,
					PlaidAccountId = "123"
				},
			};

			_plaidAccounts = plaidAccounts;
			_plaidTransactions = plaidTransactions;
			_goals = goals;
		}

		[TestMethod]
		public async Task SuccessTest()
        {
			Mock<IPlaidAccountAccessor> plaidAccountAccessorMock = new();
			Mock<IPlaidTransactionAccessor> plaidTransactionsAccessorMock = new();
			Mock<IGoalAccessor> goalAccessorMock = new();

			plaidAccountAccessorMock.Setup(x =>
				x.GetPlaidAccountsByUserIdAsync(It.IsAny<string>()))
				.ReturnsAsync(_plaidAccounts!);

			plaidTransactionsAccessorMock.Setup(x =>
				x.GetPlaidTransactionsByUserIdAsync(It.IsAny<GetTransactionsRequestModel>()))
				.ReturnsAsync(_plaidTransactions!);

			goalAccessorMock.Setup(x =>
				x.GetGoalsByUserIdAsync(It.IsAny<string>()))
				.ReturnsAsync(_goals!);

			BudgetingManager budgetingManager = new(
				goalAccessorMock.Object,
				plaidAccountAccessorMock.Object,
				plaidTransactionsAccessorMock.Object);

			GetDashboardDto response = await budgetingManager.GetUserDashboardAsync("123");

			Assert.IsNotNull(response);
			Assert.IsTrue(response.Success);
			Assert.AreEqual(HttpStatusCode.OK, response.Status);
			Assert.AreEqual(-50, response.ReadyToSpend);
			Assert.AreEqual(200, response.DepositoryAmount);
			Assert.AreEqual(100, response.CreditAmount);
			Assert.AreEqual(50, response.GoalAmount);
			Assert.AreEqual(100, response.ExpenseAmount);
			Assert.AreEqual(DateTime.Now.AddDays(-10).Day, response.LastUpdated.Day);

			Assert.IsNotNull(response.Transactions);
			Assert.AreEqual(2, response.Transactions.Count());

			Assert.IsNotNull(response.Goals);
			Assert.AreEqual(2, response.Goals.Count());
		}

		[TestMethod]
		public async Task NoTransactionsTest()
        {
			List<PlaidTransaction>? noTransactions = new();

			Mock<IPlaidAccountAccessor> plaidAccountAccessorMock = new();
			Mock<IPlaidTransactionAccessor> plaidTransactionsAccessorMock = new();
			Mock<IGoalAccessor> goalAccessorMock = new();

			plaidAccountAccessorMock.Setup(x =>
				x.GetPlaidAccountsByUserIdAsync(It.IsAny<string>()))
				.ReturnsAsync(_plaidAccounts!);

			plaidTransactionsAccessorMock.Setup(x =>
				x.GetPlaidTransactionsByUserIdAsync(It.IsAny<GetTransactionsRequestModel>()))
				.ReturnsAsync(noTransactions!);

			goalAccessorMock.Setup(x =>
				x.GetGoalsByUserIdAsync(It.IsAny<string>()))
				.ReturnsAsync(_goals!);

			BudgetingManager budgetingManager = new(
				goalAccessorMock.Object,
				plaidAccountAccessorMock.Object,
				plaidTransactionsAccessorMock.Object);

			GetDashboardDto response = await budgetingManager.GetUserDashboardAsync("123");

			Assert.IsNotNull(response);
			Assert.IsTrue(response.Success);
			Assert.AreEqual(HttpStatusCode.OK, response.Status);
			Assert.AreEqual(-50, response.ReadyToSpend);
			Assert.AreEqual(200, response.DepositoryAmount);
			Assert.AreEqual(100, response.CreditAmount);
			Assert.AreEqual(50, response.GoalAmount);
			Assert.AreEqual(100, response.ExpenseAmount);
			Assert.AreEqual(DateTime.Now.AddDays(-10).Day, response.LastUpdated.Day);

			Assert.IsNotNull(response.Transactions);
			Assert.AreEqual(0, response.Transactions.Count());
		}

		[TestMethod]
		public async Task NoGoalsTest()
		{
			List<Goal>? noGoals = new();

			Mock<IPlaidAccountAccessor> plaidAccountAccessorMock = new();
			Mock<IPlaidTransactionAccessor> plaidTransactionsAccessorMock = new();
			Mock<IGoalAccessor> goalAccessorMock = new();

			plaidAccountAccessorMock.Setup(x =>
				x.GetPlaidAccountsByUserIdAsync(It.IsAny<string>()))
				.ReturnsAsync(_plaidAccounts!);

			plaidTransactionsAccessorMock.Setup(x =>
				x.GetPlaidTransactionsByUserIdAsync(It.IsAny<GetTransactionsRequestModel>()))
				.ReturnsAsync(_plaidTransactions!);

			goalAccessorMock.Setup(x =>
				x.GetGoalsByUserIdAsync(It.IsAny<string>()))
				.ReturnsAsync(noGoals!);

			BudgetingManager budgetingManager = new(
				goalAccessorMock.Object,
				plaidAccountAccessorMock.Object,
				plaidTransactionsAccessorMock.Object);

			GetDashboardDto response = await budgetingManager.GetUserDashboardAsync("123");

			Assert.IsNotNull(response);
			Assert.IsTrue(response.Success);
			Assert.AreEqual(HttpStatusCode.OK, response.Status);
			Assert.AreEqual(100, response.ReadyToSpend);
			Assert.AreEqual(200, response.DepositoryAmount);
			Assert.AreEqual(100, response.CreditAmount);
			Assert.AreEqual(0, response.GoalAmount);
			Assert.AreEqual(0, response.ExpenseAmount);
			Assert.AreEqual(DateTime.Now.AddDays(-10).Day, response.LastUpdated.Day);

			Assert.IsNotNull(response.Transactions);
			Assert.AreEqual(2, response.Transactions.Count());

			Assert.IsNotNull(response.Goals);
			Assert.AreEqual(0, response.Goals.Count());
		}
	}
}
