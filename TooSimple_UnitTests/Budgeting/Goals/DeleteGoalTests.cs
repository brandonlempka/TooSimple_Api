using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Net;
using System.Threading.Tasks;
using TooSimple_DataAccessors.Database.Goals;
using TooSimple_Managers.Goals;
using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_UnitTests.Budgeting.Goals
{
    [TestClass]
    public class DeleteGoalTests
    {
        [TestMethod]
        public async Task DeleteGoalSuccess()
        {
            Goal goal = new()
            {
                GoalId = "123",
                GoalName = "Testing",
                GoalAmount = 123M,
                UserAccountId = "123",
                FundingScheduleId = "123",
            };

            DatabaseResponseModel databaseResponseModel =
                DatabaseResponseModel.CreateSuccess();

            Mock<IGoalAccessor> goalAccessorMock = new();

            goalAccessorMock.Setup(
                x => x.GetGoalByGoalIdAsync(It.IsAny<string>()))
                .ReturnsAsync(goal);

            goalAccessorMock.Setup(
                x => x.DeleteGoalAsync(It.IsAny<string>()))
                .ReturnsAsync(databaseResponseModel);

            GoalManager goalManager = new(goalAccessorMock.Object);

            BaseHttpResponse response = await goalManager.DeleteGoalAsync("123");

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.AreEqual(response.Status, HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task DeleteGoalValidationErrors()
        {
            Goal? goal = null;

            Mock<IGoalAccessor> goalAccessorMock = new();

            goalAccessorMock.Setup(
                x => x.GetGoalByGoalIdAsync(It.IsAny<string>()))
                .ReturnsAsync(goal);

            GoalManager goalManager = new(goalAccessorMock.Object);

            BaseHttpResponse response = await goalManager.DeleteGoalAsync("123");

            Assert.IsNotNull(response);
            Assert.IsFalse(response.Success);
            Assert.AreEqual(response.Status, HttpStatusCode.NotFound);
        }

        [TestMethod]
        public async Task DeleteGoalDatabaseFailure()
        {
            Goal goal = new()
            {
                GoalId = "123",
                GoalName = "Testing",
                GoalAmount = 123M,
                UserAccountId = "123",
                FundingScheduleId = "123",
            };


            DatabaseResponseModel databaseResponseModel = DatabaseResponseModel
                .CreateError("Test");

            Mock<IGoalAccessor> goalAccessorMock = new();

            goalAccessorMock.Setup(
                x => x.GetGoalByGoalIdAsync(It.IsAny<string>()))
                .ReturnsAsync(goal);

            goalAccessorMock.Setup(
                x => x.DeleteGoalAsync(It.IsAny<string>()))
                .ReturnsAsync(databaseResponseModel);

            GoalManager goalManager = new(
                goalAccessorMock.Object);

            BaseHttpResponse response = await goalManager.DeleteGoalAsync("123");

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.ErrorMessage);
            Assert.IsFalse(response.Success);
            Assert.AreEqual(response.ErrorMessage, "Test");
            Assert.AreEqual(response.Status, HttpStatusCode.InternalServerError);
        }
    }
}
