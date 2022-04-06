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
    public class GetGoalByGoalIdTests
    {
        [TestMethod]
        public async Task GetGoalSuccessTest()
        {
            GoalDataModel goal = new()
            {
                GoalAmount = 123.12M,
                GoalName = "Testing123",
                GoalId = "123"
            };

            List<FundingHistoryDataModel> fundingHistory = new()
            {
                new()
                {
                    SourceGoalId = "123",
                    DestinationGoalId = "456",
                    SourceGoalName = "Hello",
                    DestinationGoalName = "Testing",
                    Amount = 54.12M
                },
                new()
                {
                    SourceGoalId = "123",
                    DestinationGoalId = "0",
                    SourceGoalName = "Hello",
                    DestinationGoalName = "Ready to Spend",
                    Amount = 54.12M
                }
            };

            Mock<IGoalAccessor> goalAccessorMock = new();

            goalAccessorMock.Setup(
                x => x.GetGoalByGoalIdAsync(
                    It.IsAny<string>()))
                .ReturnsAsync(goal);

            goalAccessorMock.Setup(
                 x => x.GetFundingHistoryByGoalId(
                     It.IsAny<string>()))
                 .ReturnsAsync(fundingHistory);

            GoalManager goalManager = new(
                goalAccessorMock.Object);

            GetGoalDto response = await goalManager
                .GetGoalByGoalIdAsync("123");

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Goal);
            Assert.IsNotNull(response.FundingHistory);
            Assert.IsNotNull(string.IsNullOrWhiteSpace(response
                .Goal.GoalName));
            Assert.AreEqual(2, response.FundingHistory.Count());
            Assert.IsTrue(response.Success);
            Assert.IsTrue(string.IsNullOrWhiteSpace(response.ErrorMessage));
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
        }

        [TestMethod]
        public async Task GetGoalSuccessNoHistoryTest()
        {
            GoalDataModel goal = new()
            {
                GoalAmount = 123.12M,
                GoalName = "Testing123",
                GoalId = "123"
            };

            IEnumerable<FundingHistoryDataModel> fundingHistory = Enumerable
                .Empty<FundingHistoryDataModel>();

            Mock<IGoalAccessor> goalAccessorMock = new();
            Mock<IAccountAccessor> accountAccessorMock = new();

            goalAccessorMock.Setup(
                x => x.GetGoalByGoalIdAsync(
                    It.IsAny<string>()))
                .ReturnsAsync(goal);

            goalAccessorMock.Setup(
                 x => x.GetFundingHistoryByGoalId(
                     It.IsAny<string>()))
                 .ReturnsAsync(fundingHistory);

            GoalManager goalManager = new(
                goalAccessorMock.Object);

            GetGoalDto response = await goalManager
                .GetGoalByGoalIdAsync("123");

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Goal);
            Assert.IsNotNull(response.FundingHistory);
            Assert.IsNotNull(string.IsNullOrWhiteSpace(response.Goal.GoalName));
            Assert.IsTrue(response.Success);
            Assert.IsTrue(string.IsNullOrWhiteSpace(response.ErrorMessage));
            Assert.AreEqual(HttpStatusCode.OK, response.Status);

        }

        [TestMethod]
        public async Task GetGoalNoContentTest()
        {
            GoalDataModel? goal = null;

            Mock<IGoalAccessor> goalAccessorMock = new();
            Mock<IAccountAccessor> accountAccessorMock = new();

            goalAccessorMock.Setup(
                x => x.GetGoalByGoalIdAsync(
                    It.IsAny<string>()))
                .ReturnsAsync(goal);

            GoalManager goalManager = new(
                goalAccessorMock.Object);

            GetGoalDto response = await goalManager
                .GetGoalByGoalIdAsync("123");

            Assert.IsNotNull(response);
            Assert.IsNull(response.Goal);
            Assert.IsNull(response.FundingHistory);
            Assert.IsFalse(response.Success);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.ErrorMessage));
            Assert.AreEqual(HttpStatusCode.NotFound, response.Status);
        }
    }
}