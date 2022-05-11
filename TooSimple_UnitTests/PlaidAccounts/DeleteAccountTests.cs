using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Net;
using System.Threading.Tasks;
using TooSimple_DataAccessors.Database.PlaidAccounts;
using TooSimple_Managers.PlaidAccounts;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_UnitTests.PlaidAccounts
{
    [TestClass]
    public class DeleteAccountTests
    {
        [TestMethod]
        public async Task SuccessTest()
        {
            string accountId = "123";
            DatabaseResponseModel databaseResponseModel = DatabaseResponseModel.CreateSuccess();

            Mock<IPlaidAccountAccessor> accountAccessorMock = new();

            accountAccessorMock.Setup(
                x => x.DeleteAccountAsync(It.IsAny<string>()))
                .ReturnsAsync(databaseResponseModel);

            PlaidAccountManager plaidAccountManager = new(accountAccessorMock.Object);

            BaseHttpResponse response = await plaidAccountManager.DeleteAccountAsync(accountId);

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
        }

        [TestMethod]
        public async Task NoAccountIdTest()
        {
            string accountId = string.Empty;
            Mock<IPlaidAccountAccessor> accountAccessorMock = new();

            PlaidAccountManager plaidAccountManager = new(accountAccessorMock.Object);

            BaseHttpResponse response = await plaidAccountManager.DeleteAccountAsync(accountId);

            Assert.IsNotNull(response);
            Assert.IsFalse(response.Success);
            Assert.AreEqual(response.Status, HttpStatusCode.BadRequest);
            Assert.AreEqual(response.ErrorMessage, "Account Id is not formatted correctly.");
        }

        [TestMethod]
        public async Task DbFailureTest()
        {
            string accountId = "123";
            DatabaseResponseModel databaseResponseModel = DatabaseResponseModel.CreateError("Test");

            Mock<IPlaidAccountAccessor> accountAccessorMock = new();

            accountAccessorMock.Setup(
                x => x.DeleteAccountAsync(It.IsAny<string>()))
                .ReturnsAsync(databaseResponseModel);

            PlaidAccountManager plaidAccountManager = new(accountAccessorMock.Object);

            BaseHttpResponse response = await plaidAccountManager.DeleteAccountAsync(accountId);

            Assert.IsNotNull(response);
            Assert.IsFalse(response.Success);
            Assert.AreEqual(response.Status, HttpStatusCode.InternalServerError);
            Assert.AreEqual(response.ErrorMessage, "Test");

        }
    }
}
