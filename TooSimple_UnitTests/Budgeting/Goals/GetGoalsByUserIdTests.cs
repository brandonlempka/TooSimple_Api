using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TooSimple_DataAccessors.Database.Accounts;
using TooSimple_DataAccessors.Database.Goals;
using TooSimple_Managers.Goals;
using TooSimple_Poco.Models.Budgeting;
using TooSimple_Poco.Models.Database;

namespace TooSimple_UnitTests.Budgeting.Goals
{
    [TestClass]
    public class GetGoalsByUserIdTests
	{
		[TestMethod]
		public async Task GetGoalsSuccessTest()
		{
			List<GoalDataModel> goals = new()
			{
				new()
				{
					GoalAmount = 123.12M,
					GoalName = "Testing123"
				},
				new()
				{
					GoalAmount = 456.45M,
					GoalName = "Testing456"
				}
			};

			Mock<IGoalAccessor> goalAccessorMock = new();
			Mock<IAccountAccessor> accountAccessorMock = new();

			goalAccessorMock.Setup(
				x => x.GetGoalsByUserIdAsync(
					It.IsAny<string>()))
				.ReturnsAsync(goals);


			GoalManager goalManager = new(
				goalAccessorMock.Object);

			GetGoalsDto response = await goalManager.GetGoalsByUserIdAsync("123");
			Assert.IsNotNull(response.Goals);
			Assert.AreEqual(2, response.Goals.Count());
			Assert.IsTrue(response.Success);
			Assert.IsTrue(string.IsNullOrWhiteSpace(response.ErrorMessage));
			Assert.AreEqual(HttpStatusCode.OK, response.Status);
		}

		[TestMethod]
		public async Task GetGoalsNoResultsTest()
        {
			List<GoalDataModel> goals = new();

			Mock<IGoalAccessor> goalAccessorMock = new();
			Mock<IAccountAccessor> accountAccessorMock = new();

			goalAccessorMock.Setup(
				x => x.GetGoalsByUserIdAsync(
					It.IsAny<string>()))
				.ReturnsAsync(goals);


			GoalManager goalManager = new(
				goalAccessorMock.Object);

			GetGoalsDto response = await goalManager
				.GetGoalsByUserIdAsync("123");

			Assert.IsNull(response.Goals);
			Assert.IsFalse(response.Success);
			Assert.IsFalse(string.IsNullOrWhiteSpace(response.ErrorMessage));
			Assert.AreEqual(HttpStatusCode.NotFound, response.Status);
		}
	}
}

