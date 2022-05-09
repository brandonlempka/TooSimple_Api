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
    public class GetGoalsByUserIdTests
    {
        [TestMethod]
        public async Task GetGoalsSuccessTest()
        {
            List<Goal> goals = new()
            {
                new()
                {
                    GoalAmount = 123.12M,
                    GoalName = "Testing123",
                    FundingScheduleId = "456"
                },
                new()
                {
                    GoalAmount = 456.45M,
                    GoalName = "Testing456",
                    FundingScheduleId = "456"
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
                x => x.GetGoalsByUserIdAsync(
                    It.IsAny<string>()))
                .ReturnsAsync(goals);

            fundingScheduleAccessorMock.Setup(
                x => x.GetFundingSchedulesByUserIdAsync(
                    It.IsAny<string>()))
                .ReturnsAsync(fundingSchedules);

            GoalManager goalManager = new(
                goalAccessorMock.Object,
                fundingScheduleAccessorMock.Object);

            GetGoalsDto response = await goalManager.GetGoalsByUserIdAsync("123");

            Assert.IsNotNull(response.Goals);
            Assert.AreEqual(2, response.Goals.Count());
            Assert.IsTrue(response.Success);
            Assert.IsTrue(string.IsNullOrWhiteSpace(response.ErrorMessage));
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
        }

        [TestMethod]
        public async Task GetGoalsNoSchedulesTest()
        {
            List<Goal> goals = new()
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
                x => x.GetGoalsByUserIdAsync(
                    It.IsAny<string>()))
                .ReturnsAsync(goals);

            fundingScheduleAccessorMock.Setup(
                x => x.GetFundingSchedulesByUserIdAsync(
                    It.IsAny<string>()))
                .ReturnsAsync(fundingSchedules);

            GoalManager goalManager = new(
                goalAccessorMock.Object,
                fundingScheduleAccessorMock.Object);

            GetGoalsDto response = await goalManager.GetGoalsByUserIdAsync("123");

            Assert.IsNull(response.Goals);
            Assert.IsFalse(response.Success);
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.Status);
        }

        [TestMethod]
        public async Task GetGoalsNoResultsTest()
        {
            List<Goal> goals = new();

            List<FundingSchedule> fundingSchedules = new();

            Mock<IGoalAccessor> goalAccessorMock = new();
            Mock<IFundingScheduleAccessor> fundingScheduleAccessorMock = new();

            goalAccessorMock.Setup(
                x => x.GetGoalsByUserIdAsync(
                    It.IsAny<string>()))
                .ReturnsAsync(goals);


            fundingScheduleAccessorMock.Setup(
                x => x.GetFundingSchedulesByUserIdAsync(
                    It.IsAny<string>()))
                .ReturnsAsync(fundingSchedules);

            GoalManager goalManager = new(
                goalAccessorMock.Object,
                fundingScheduleAccessorMock.Object);

            GetGoalsDto response = await goalManager
                .GetGoalsByUserIdAsync("123");

            Assert.IsNull(response.Goals);
            Assert.IsFalse(response.Success);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.ErrorMessage));
            Assert.AreEqual(HttpStatusCode.NotFound, response.Status);
        }
    }
}

