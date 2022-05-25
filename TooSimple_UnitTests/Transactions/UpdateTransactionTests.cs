using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TooSimple_DataAccessors.Database.PlaidAccounts;
using TooSimple_DataAccessors.Database.Transactions;
using TooSimple_Managers.Transactions;
using TooSimple_Poco.Models.ApiRequestModels;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_UnitTests.Transactions
{
    [TestClass]
	public class UpdateTransactionTests
	{
        [DataTestMethod]
        [DataRow(null)]
        [DataRow("123123")]
		public async Task UpdateTransactionSuccess(string? newGoalId)
        {
            UpdatePlaidTransactionRequestModel requestModel = new()
            {
                PlaidTransactionId = "123",
                SpendingFromGoalId = newGoalId
            };

            DatabaseResponseModel databaseResponseModel = DatabaseResponseModel.CreateSuccess();

            Mock<IPlaidTransactionAccessor> plaidTransactionAccessor = new();
            Mock<IPlaidAccountAccessor> plaidAccountAccessor = new();

            plaidTransactionAccessor.Setup(
                x => x.UpdatePlaidTransactionAsync(It.IsAny<UpdatePlaidTransactionRequestModel>()))
                .ReturnsAsync(databaseResponseModel);

            PlaidTransactionManager plaidTransactionManager = new(
                plaidTransactionAccessor.Object,
                plaidAccountAccessor.Object);

            BaseHttpResponse response = await plaidTransactionManager.UpdatePlaidTransactionAsync(requestModel);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.IsTrue(string.IsNullOrWhiteSpace(response.ErrorMessage));
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
        }

        [TestMethod]
        public async Task BadRequestTest()
        {
            UpdatePlaidTransactionRequestModel requestModel = new()
            {
                PlaidTransactionId = null,
                SpendingFromGoalId = "test"
            };

            DatabaseResponseModel databaseResponseModel = DatabaseResponseModel.CreateSuccess();

            Mock<IPlaidTransactionAccessor> plaidTransactionAccessor = new();
            Mock<IPlaidAccountAccessor> plaidAccountAccessor = new();

            plaidTransactionAccessor.Setup(
                x => x.UpdatePlaidTransactionAsync(It.IsAny<UpdatePlaidTransactionRequestModel>()))
                .ReturnsAsync(databaseResponseModel);

            PlaidTransactionManager plaidTransactionManager = new(
                plaidTransactionAccessor.Object,
                plaidAccountAccessor.Object);

            BaseHttpResponse response = await plaidTransactionManager.UpdatePlaidTransactionAsync(requestModel);

            Assert.IsNotNull(response);
            Assert.IsFalse(response.Success);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.ErrorMessage));
            Assert.AreEqual(HttpStatusCode.BadRequest, response.Status);
        }
    }
}
