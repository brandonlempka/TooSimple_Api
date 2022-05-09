using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TooSimple_DataAccessors.Database.FundingSchedules;
using TooSimple_DataAccessors.Database.Goals;
using TooSimple_Managers.Goals;
using TooSimple_Poco.Models.DataModels;
using TooSimple_Poco.Models.Entities;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_UnitTests.Budgeting.Goals
{
    [TestClass]
    public class AddNewGoalTests
    {
        [TestMethod]
        public async Task AddNewGoalSuccess()
        {
            GoalDataModel goalDataModel = new()
            {
                GoalName = "Testing",
                GoalAmount = 123M,
                DesiredCompletionDate = DateTime.UtcNow.AddDays(10),
                UserAccountId = "123",
                FundingScheduleId = "123",
            };

            DatabaseResponseModel databaseResponseModel = DatabaseResponseModel
                .CreateSuccess();

            Mock<IGoalAccessor> goalAccessorMock = new();
            Mock<IFundingScheduleAccessor> fundingScheduleAccessor = new();

            goalAccessorMock.Setup(
                x => x.AddNewGoalAsync(It.IsAny<Goal>()))
                .ReturnsAsync(databaseResponseModel);

            GoalManager goalManager = new(
                goalAccessorMock.Object,
                fundingScheduleAccessor.Object);

            BaseHttpResponse response = await goalManager.AddNewGoalAsync(goalDataModel);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        [DataTestMethod]
        [DataRow("", "123", "2100-01-01", "123", 1)]
        [DataRow("123", "123", "2000-01-01", "123", 1)]
        [DataRow("123", "123", "2000-01-01", "123", 1)]
        [DataRow("123", "", "2100-01-01", "123", 1)]
        [DataRow("123", "123", "2100-01-01", "", 1)]
        [DataRow("", "", "2000-01-01", "", 4)]
        public async Task AddNewGoalValidationFailure(
            string goalName,
            string fundingScheduleId,
            string desiredCompletionDate,
            string userAccountId,
            int numberOfErrors)
        {
            GoalDataModel goalDataModel = new()
            {
                GoalName = goalName,
                GoalAmount = 123M,
                DesiredCompletionDate = Convert.ToDateTime(desiredCompletionDate),
                UserAccountId = userAccountId,
                FundingScheduleId = fundingScheduleId,
            };

            DatabaseResponseModel databaseResponseModel = DatabaseResponseModel
                .CreateSuccess();

            Mock<IGoalAccessor> goalAccessorMock = new();
            Mock<IFundingScheduleAccessor> fundingScheduleAccessor = new();

            goalAccessorMock.Setup(
                x => x.AddNewGoalAsync(It.IsAny<Goal>()))
                .ReturnsAsync(databaseResponseModel);

            GoalManager goalManager = new(
                goalAccessorMock.Object,
                fundingScheduleAccessor.Object);

            BaseHttpResponse response = await goalManager.AddNewGoalAsync(goalDataModel);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.ErrorMessage);
            Assert.IsFalse(response.Success);
            Assert.AreEqual(response.Status, HttpStatusCode.BadRequest);
            // Have to subtract 1 from number of expected errors since I'm counting
            // the commas.
            Assert.AreEqual(
                response.ErrorMessage.Count(c => c == ','),
                numberOfErrors - 1);
        }

        [TestMethod]
        public async Task AddNewGoalDatabaseFailure()
        {
            GoalDataModel goalDataModel = new()
            {
                GoalName = "Testing",
                GoalAmount = 123M,
                DesiredCompletionDate = DateTime.UtcNow.AddDays(10),
                UserAccountId = "123",
                FundingScheduleId = "123",
            };

            DatabaseResponseModel databaseResponseModel = DatabaseResponseModel
                .CreateError("Test");

            Mock<IGoalAccessor> goalAccessorMock = new();
            Mock<IFundingScheduleAccessor> fundingScheduleAccessor = new();

            goalAccessorMock.Setup(
                x => x.AddNewGoalAsync(It.IsAny<Goal>()))
                .ReturnsAsync(databaseResponseModel);

            GoalManager goalManager = new(
                goalAccessorMock.Object,
                fundingScheduleAccessor.Object);

            BaseHttpResponse response = await goalManager.AddNewGoalAsync(goalDataModel);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.ErrorMessage);
            Assert.IsFalse(response.Success);
            Assert.AreEqual(response.ErrorMessage, "Test");
            Assert.AreEqual(response.Status, HttpStatusCode.InternalServerError);
        }
    }
}
