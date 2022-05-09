using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TooSimple_DataAccessors.Database.FundingSchedules;
using TooSimple_DataAccessors.Database.Goals;
using TooSimple_Managers.Goals;
using TooSimple_Poco.Models.Dtos.Goals;
using TooSimple_Poco.Models.Entities;

namespace TooSimple_UnitTests.Budgeting.Goals
{
    [TestClass]
    public class GetGoalByGoalIdTests
    {
        [TestMethod]
        public async Task GetGoalSuccessTest()
        {
            Goal goal = new()
            {
                GoalAmount = 123.12M,
                GoalName = "Testing123",
                GoalId = "123"
            };

            List<FundingHistory> fundingHistory = new()
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

            List<FundingSchedule> fundingSchedules = new()
            {
                new()
                {
                    FirstContributionDate = DateTime.UtcNow.AddDays(-100),
                    UserAccountId = "123",
                    Frequency = 1,
                    FundingScheduleId = "456",
                    FundingScheduleName = "First one"
                },
                new()
                {
                    FirstContributionDate = DateTime.UtcNow.AddDays(-100),
                    UserAccountId = "456",
                    Frequency = 2,
                    FundingScheduleId = "789",
                    FundingScheduleName = "Second one"
                },
            };

            Mock<IGoalAccessor> goalAccessorMock = new();
            Mock<IFundingScheduleAccessor> fundingScheduleAccessorMock = new();

            goalAccessorMock.Setup(
                x => x.GetGoalByGoalIdAsync(
                    It.IsAny<string>()))
                .ReturnsAsync(goal);

            goalAccessorMock.Setup(
                 x => x.GetFundingHistoryByGoalId(
                     It.IsAny<string>()))
                 .ReturnsAsync(fundingHistory);

            fundingScheduleAccessorMock.Setup(
                x => x.GetFundingSchedulesByUserIdAsync(
                    It.IsAny<string>()))
                .ReturnsAsync(fundingSchedules);

            GoalManager goalManager = new(
                goalAccessorMock.Object,
                fundingScheduleAccessorMock.Object);

            GetGoalDto response = await goalManager
                .GetGoalByGoalIdAsync("123");

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Goal);
            Assert.IsNotNull(response.FundingHistory);
            Assert.IsNotNull(response.FundingSchedules);
            Assert.IsNotNull(string.IsNullOrWhiteSpace(response
                .Goal.GoalName));
            Assert.AreEqual(2, response.FundingHistory.Count());
            Assert.AreEqual(2, response.FundingSchedules.Count());
            Assert.IsTrue(response.Success);
            Assert.IsTrue(string.IsNullOrWhiteSpace(response.ErrorMessage));
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
        }

        [TestMethod]
        public async Task GetGoalSuccessNoHistoryTest()
        {
            Goal goal = new()
            {
                GoalAmount = 123.12M,
                GoalName = "Testing123",
                GoalId = "123"
            };

            List<FundingSchedule> fundingSchedules = new()
            {
                new()
                {
                    FirstContributionDate = DateTime.UtcNow.AddDays(-100),
                    UserAccountId = "123",
                    Frequency = 1,
                    FundingScheduleId = "456",
                    FundingScheduleName = "First one"
                },
                new()
                {
                    FirstContributionDate = DateTime.UtcNow.AddDays(-100),
                    UserAccountId = "456",
                    Frequency = 2,
                    FundingScheduleId = "789",
                    FundingScheduleName = "Second one"
                },
            };

            IEnumerable<FundingHistory> fundingHistory = Enumerable
                .Empty<FundingHistory>();

            Mock<IGoalAccessor> goalAccessorMock = new();
            Mock<IFundingScheduleAccessor> fundingScheduleAccessorMock = new();

            goalAccessorMock.Setup(
                x => x.GetGoalByGoalIdAsync(
                    It.IsAny<string>()))
                .ReturnsAsync(goal);

            goalAccessorMock.Setup(
                 x => x.GetFundingHistoryByGoalId(
                     It.IsAny<string>()))
                 .ReturnsAsync(fundingHistory);

            fundingScheduleAccessorMock.Setup(
                x => x.GetFundingSchedulesByUserIdAsync(
                    It.IsAny<string>()))
                .ReturnsAsync(fundingSchedules);

            GoalManager goalManager = new(
                goalAccessorMock.Object,
                fundingScheduleAccessorMock.Object);

            GetGoalDto response = await goalManager
                .GetGoalByGoalIdAsync("123");

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.Goal);
            Assert.IsNotNull(response.FundingHistory);
            Assert.IsNotNull(response.FundingSchedules);
            Assert.IsNotNull(string.IsNullOrWhiteSpace(response.Goal.GoalName));
            Assert.IsTrue(response.Success);
            Assert.AreEqual(2, response.FundingSchedules.Count());
            Assert.IsTrue(string.IsNullOrWhiteSpace(response.ErrorMessage));
            Assert.AreEqual(HttpStatusCode.OK, response.Status);

        }

        [TestMethod]
        public async Task GetGoalNoContentTest()
        {
            Goal? goal = null;

            Mock<IGoalAccessor> goalAccessorMock = new();
            Mock<IFundingScheduleAccessor> fundingScheduleAccessorMock = new();

            goalAccessorMock.Setup(
                x => x.GetGoalByGoalIdAsync(
                    It.IsAny<string>()))
                .ReturnsAsync(goal);

            GoalManager goalManager = new(
                goalAccessorMock.Object,
                fundingScheduleAccessorMock.Object);

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