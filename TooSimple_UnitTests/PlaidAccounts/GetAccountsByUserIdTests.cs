using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TooSimple_DataAccessors.Database.PlaidAccounts;
using TooSimple_Managers.PlaidAccounts;
using TooSimple_Poco.Models.Dtos.PlaidAccounts;
using TooSimple_Poco.Models.Entities;

namespace TooSimple_UnitTests.PlaidAccounts
{
	[TestClass]
	public class GetAccountsByUserIdTests
	{
        [TestMethod]
		public async Task SuccessTest()
        {
            string userId = "123";

            List<PlaidAccount> plaidAccounts = new()
            {
                new()
                {
                    PlaidAccountId = "123"
                },
                new()
                {
                    PlaidAccountId = "456"
                },
            };

            Mock<IPlaidAccountAccessor> accountAccessorMock = new();

            accountAccessorMock.Setup(
                x => x.GetPlaidAccountsByUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(plaidAccounts);

            PlaidAccountManager plaidAccountManager = new(accountAccessorMock.Object);

            GetPlaidAccountsDto response = await plaidAccountManager.GetPlaidAccountsByUserIdAsync(userId);

            Assert.IsNotNull(response);
            Assert.IsNull(response.ErrorMessage);
            Assert.IsTrue(response.Success);
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.IsNotNull(response.PlaidAccounts);
            Assert.AreEqual(2, response.PlaidAccounts.Count());
        }

        [TestMethod]
        public async Task NoContentTest()
        {
            string userId = "123";

            List<PlaidAccount> plaidAccounts = new();

            Mock<IPlaidAccountAccessor> accountAccessorMock = new();

            accountAccessorMock.Setup(
                x => x.GetPlaidAccountsByUserIdAsync(It.IsAny<string>()))
                .ReturnsAsync(plaidAccounts);

            PlaidAccountManager plaidAccountManager = new(accountAccessorMock.Object);

            GetPlaidAccountsDto response = await plaidAccountManager.GetPlaidAccountsByUserIdAsync(userId);

            Assert.IsNotNull(response);
            Assert.IsNull(response.ErrorMessage);
            Assert.IsTrue(response.Success);
            Assert.AreEqual(HttpStatusCode.NoContent, response.Status);
            Assert.IsNull(response.PlaidAccounts);
        }

        [TestMethod]
        public async Task NoUserIdTest()
        {
            string userId = string.Empty;

            Mock<IPlaidAccountAccessor> accountAccessorMock = new();

            PlaidAccountManager plaidAccountManager = new(accountAccessorMock.Object);

            GetPlaidAccountsDto response = await plaidAccountManager.GetPlaidAccountsByUserIdAsync(userId);

            Assert.IsNotNull(response);
            Assert.IsNotNull(response.ErrorMessage);
            Assert.AreEqual("User Id is required.", response.ErrorMessage);
            Assert.IsFalse(response.Success);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.Status);
            Assert.IsNull(response.PlaidAccounts);
        }
    }
}
