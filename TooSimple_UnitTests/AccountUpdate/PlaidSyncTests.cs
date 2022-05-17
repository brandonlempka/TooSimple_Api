using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TooSimple_DataAccessors.Database.Logging;
using TooSimple_DataAccessors.Database.PlaidAccounts;
using TooSimple_DataAccessors.Plaid.AccountUpdate;
using TooSimple_Managers.Budgeting;
using TooSimple_Managers.Plaid.AccountUpdate;
using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.Entities;
using TooSimple_Poco.Models.Plaid.AccountUpdate;
using TooSimple_Poco.Models.Plaid.Transactions;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_UnitTests.AccountUpdate
{
	[TestClass]
	public class PlaidSyncTests
	{
		private IPlaidSyncAccessor? _plaidSyncAccessor;
		private IPlaidAccountAccessor? _plaidAccountAccessor;
		private ILoggingAccessor? _loggingAccessor;
		private IBudgetingManager? _budgetingManager;


		[TestInitialize]
		public void Setup()
        {
			PlaidGetTransactionsResponseModel transactionResponseModel = new()
			{
				Accounts = new List<AccountResponseModel>()
                {
					new()
					{
						AccountId = "123",
						AccountOwner = "test",
						Type = "depository",
						Subtype = "checking",
						Balances = new()
						{
							Available = 123M,
							Current = 124M,
						}
					},
					new()
					{
						AccountId = "456",
						AccountOwner = "test",
						Type = "depository",
						Subtype = "checking",
						Balances = new()
						{
							Available = 123M,
							Current = 124M,
						}
					},
				},
				TotalTransactions = 3,
				Transactions = new List<TransactionResponseModel>()
                {
					new()
					{
						AccountId = "123",
						Amount = 5,
						IsPending = true,
						Name = "Hi",
						MerchantName = "A place",
						Datetime = DateTime.Now,
						PersonalFinanceCategory = new()
						{
							Primary = "TRANSPORTATION",
							Detailed = "TRANSPORTATION_TAXIS_AND_RIDE_SHARES"
						},
						TransactionId = "123"
					},
					new()
					{
						AccountId = "123",
						Amount = -10,
						IsPending = false,
						Name = "Hi",
						MerchantName = "A place",
						Datetime = DateTime.Now,
						PersonalFinanceCategory = new()
						{
							Primary = "TRANSPORTATION",
							Detailed = "TRANSPORTATION_TAXIS_AND_RIDE_SHARES"
						},
						TransactionId = "456"
					},
					new()
					{
						AccountId = "456",
						Amount = 5,
						IsPending = false,
						Name = "Hi",
						MerchantName = "A place",
						Datetime = DateTime.Now,
						PersonalFinanceCategory = new()
						{
							Primary = "TRANSPORTATION",
							Detailed = "TRANSPORTATION_TAXIS_AND_RIDE_SHARES"
						},
						TransactionId = "789"
					},
				}
			};

			List<PlaidAccount> plaidAccounts = new()
			{
				new()
				{
					AccessToken = "123",
					PlaidAccountId = "123",
					UserAccountId = "123",
					IsPlaidRelogRequired = false,
					PlaidAccountTypeId = 1
				},
				new()
				{
					AccessToken = "456",
					PlaidAccountId = "123",
					UserAccountId = "123",
					IsPlaidRelogRequired = false,
					PlaidAccountTypeId = 3
				}
			};

			Mock<IPlaidSyncAccessor> plaidSyncAccessor = new();
			Mock<IPlaidAccountAccessor> plaidAccountAccessor = new();
			Mock<ILoggingAccessor> loggingAccessor = new();
			Mock<IBudgetingManager> budgetingManager = new();

			plaidSyncAccessor.Setup(
				x => x.GetPlaidTransactionsAsync(It.IsAny<TransactionUpdateRequestModel>()))
				.ReturnsAsync(transactionResponseModel);

			loggingAccessor.Setup(
				x => x.LogMessageAsync(It.IsAny<string>(), null))
				.ReturnsAsync(true);

			plaidAccountAccessor.Setup(
				x => x.GetPlaidAccountsByUserIdAsync(It.IsAny<string>()))
				.ReturnsAsync(plaidAccounts);
			plaidAccountAccessor.Setup(
				x => x.GetPlaidAccountsByItemIdAsync(It.IsAny<string>()))
				.ReturnsAsync(plaidAccounts);

			plaidAccountAccessor.Setup(
				x => x.UpdateAccountBalancesAsync(It.IsAny<IEnumerable<PlaidAccount>>()))
				.ReturnsAsync(DatabaseResponseModel.CreateSuccess());

			plaidAccountAccessor.Setup(
				x => x.UpsertPlaidTransactionsAsync(It.IsAny<IEnumerable<PlaidTransaction>>()))
				.ReturnsAsync(DatabaseResponseModel.CreateSuccess());

			_plaidSyncAccessor = plaidSyncAccessor.Object;
			_plaidAccountAccessor = plaidAccountAccessor.Object;
			_loggingAccessor = loggingAccessor.Object;
			_budgetingManager = budgetingManager.Object;
		}

		[TestMethod]
		public async Task ItemIdSuccessTest()
		{
			var jsonObject = new
			{
				error = "",
				item_id = "adsfkaldsfjasd",
				new_transactions = 3,
				webhook_code = "DEFAULT_UPDATE",
				webhook_type = "TRANSACTIONS"
			};

			JsonElement json = JsonSerializer.SerializeToElement(jsonObject);
			AccountUpdateManager accountUpdateManager = new(
				_plaidSyncAccessor!,
				_plaidAccountAccessor!,
				_loggingAccessor!,
				_budgetingManager!);

			DatabaseResponseModel response = await accountUpdateManager.PlaidSyncByItemIdAsync(json);

			Assert.IsNotNull(response);
			Assert.IsNull(response.ErrorMessage);
			Assert.IsTrue(response.Success);
		}

		[DataTestMethod]
		[DataRow("DEFAULT_SUPDATE", "TRANSACTIONS")]
		[DataRow("DEFAULT_UPDATE", "Hi")]
		public async Task ItemIdBadRequest(
			string code,
			string type)
        {
			var jsonObject = new
			{
				error = "",
				item_id = "adsfkaldsfjasd",
				new_transactions = 3,
				webhook_code = code,
				webhook_type = type
			};

			JsonElement json = JsonSerializer.SerializeToElement(jsonObject);
			AccountUpdateManager accountUpdateManager = new(
				_plaidSyncAccessor!,
				_plaidAccountAccessor!,
				_loggingAccessor!,
				_budgetingManager!);

			DatabaseResponseModel response = await accountUpdateManager.PlaidSyncByItemIdAsync(json);

			Assert.IsNotNull(response);
			Assert.IsNull(response.ErrorMessage);
			Assert.IsTrue(response.Success);
		}

		[TestMethod]
		public async Task UserIdSuccessTest()
		{
			AccountUpdateManager accountUpdateManager = new(
				_plaidSyncAccessor!,
				_plaidAccountAccessor!,
				_loggingAccessor!,
				_budgetingManager!);

			BaseHttpResponse response = await accountUpdateManager.PlaidSyncByUserIdAsync("123");

			Assert.IsNotNull(response);
			Assert.IsNull(response.ErrorMessage);
			Assert.IsTrue(response.Success);
		}
	}
}
