using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TooSimple_DataAccessors.Database.PlaidAccounts;
using TooSimple_DataAccessors.Plaid.TokenExchange;
using TooSimple_Managers.Plaid.TokenExchange;
using TooSimple_Poco.Models.Entities;
using TooSimple_Poco.Models.Plaid.TokenExchange;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_UnitTests.TokenExchange
{
    [TestClass]
    public class TokenExchangeTests
    {
        [TestMethod]
        public async Task SuccessTest()
        {
            string userId = "123";
            TokenExchangeDataModel dataModel = new()
            {
                Accounts = new List<TokenExchangeAccount>
                {
                    new()
                    {
                        Id = "1",
                        Name = "Test",
                        Mask = "Hi!",
                        Type = "liability",
                        Subtype = "credit card",
                    },
                    new()
                    {
                        Id = "2",
                        Name = "Test 2",
                        Mask = "bye!",
                        Type = "depository",
                        Subtype = "checking",
                    }
                },
                PublicToken = "123",
            };

            TokenExchangeResponseModel responseModel = new()
            {
                ItemId = "1",
                AccessToken = "asdfklsadjfklasjdkl",
                RequestId = "kdkdkd"
            };

            DatabaseResponseModel databaseResponseModel = new()
            {
                Success = true
            };

            Mock<ITokenExchangeAccessor> tokenExchangeAccessor = new();
            Mock<IPlaidAccountAccessor> accountAccessorMock = new();

            tokenExchangeAccessor.Setup(
                x => x.PublicTokenExchangeAsync(It.IsAny<TokenExchangeRequestModel>()))
                .ReturnsAsync(responseModel);

            accountAccessorMock.Setup(
                x => x.InsertNewAccountAsync(It.IsAny<PlaidAccount>()))
                .ReturnsAsync(databaseResponseModel);

            TokenExchangeManager tokenExchangeManager = new(
                tokenExchangeAccessor.Object,
                accountAccessorMock.Object);

            BaseHttpResponse httpResponse = await tokenExchangeManager.PublicTokenExchangeAsync(userId, dataModel);

            Assert.IsNotNull(httpResponse);
            Assert.IsTrue(httpResponse.Success);
            Assert.AreEqual(httpResponse.Status, HttpStatusCode.Created);
            Assert.IsNull(httpResponse.ErrorMessage);
        }

        [TestMethod]
        public async Task ValidationFailureOneTest()
        {
            string userId = "123";
            TokenExchangeDataModel dataModel = new()
            {
                Accounts = new List<TokenExchangeAccount>
                {
                    new()
                    {
                        Id = "1",
                        Name = "Test",
                        Mask = "Hi!",
                        Type = "liability",
                        Subtype = "credit card",
                    },
                    new()
                    {
                        Id = "2",
                        Name = "Test 2",
                        Mask = "bye!",
                        Type = "depository",
                        Subtype = "checking",
                    }
                },
                PublicToken = "",
            };

            Mock<ITokenExchangeAccessor> tokenExchangeAccessor = new();
            Mock<IPlaidAccountAccessor> accountAccessorMock = new();

            TokenExchangeManager tokenExchangeManager = new(
                tokenExchangeAccessor.Object,
                accountAccessorMock.Object);

            BaseHttpResponse httpResponse = await tokenExchangeManager.PublicTokenExchangeAsync(userId, dataModel);

            Assert.IsNotNull(httpResponse);
            Assert.IsFalse(httpResponse.Success);
            Assert.AreEqual(httpResponse.Status, HttpStatusCode.BadRequest);
            Assert.AreEqual(httpResponse.ErrorMessage, "Public token was not formed correctly.");
        }

        [TestMethod]
        public async Task ValidationFailureTwoTest()
        {
            string userId = "123";
            TokenExchangeDataModel dataModel = new()
            {
                Accounts = new List<TokenExchangeAccount>(),
                PublicToken = "123",
            };

            Mock<ITokenExchangeAccessor> tokenExchangeAccessor = new();
            Mock<IPlaidAccountAccessor> accountAccessorMock = new();

            TokenExchangeManager tokenExchangeManager = new(
                tokenExchangeAccessor.Object,
                accountAccessorMock.Object);

            BaseHttpResponse httpResponse = await tokenExchangeManager.PublicTokenExchangeAsync(userId, dataModel);

            Assert.IsNotNull(httpResponse);
            Assert.IsFalse(httpResponse.Success);
            Assert.AreEqual(httpResponse.Status, HttpStatusCode.BadRequest);
            Assert.AreEqual(httpResponse.ErrorMessage, "Public token was not formed correctly.");
        }

        [DataTestMethod]
        [DataRow("", "123", "")]
        [DataRow("123", "", "")]
        [DataRow("", "", "123")]
        public async Task PlaidErrorTests(
            string itemId, 
            string accessToken, 
            string errorMessage)
        {
            string userId = "123";
            TokenExchangeDataModel dataModel = new()
            {
                Accounts = new List<TokenExchangeAccount>
                {
                    new()
                    {
                        Id = "1",
                        Name = "Test",
                        Mask = "Hi!",
                        Type = "liability",
                        Subtype = "credit card",
                    },
                    new()
                    {
                        Id = "2",
                        Name = "Test 2",
                        Mask = "bye!",
                        Type = "depository",
                        Subtype = "checking",
                    }
                },
                PublicToken = "123",
            };

            TokenExchangeResponseModel responseModel = new()
            {
                ItemId = itemId,
                AccessToken = accessToken,
                RequestId = "123",
                ErrorMessage = errorMessage
            };

            Mock<ITokenExchangeAccessor> tokenExchangeAccessor = new();
            Mock<IPlaidAccountAccessor> accountAccessorMock = new();

            tokenExchangeAccessor.Setup(
                x => x.PublicTokenExchangeAsync(It.IsAny<TokenExchangeRequestModel>()))
                .ReturnsAsync(responseModel);

            TokenExchangeManager tokenExchangeManager = new(
                tokenExchangeAccessor.Object,
                accountAccessorMock.Object);

            BaseHttpResponse httpResponse = await tokenExchangeManager.PublicTokenExchangeAsync(userId, dataModel);

            Assert.IsNotNull(httpResponse);
            Assert.IsFalse(httpResponse.Success);
            Assert.AreEqual(httpResponse.Status, HttpStatusCode.InternalServerError);
            Assert.AreEqual(httpResponse.ErrorMessage, errorMessage);
        }

        [DataTestMethod]
        [DataRow("TEST", "TEST")]
        [DataRow("", "Something went while saving new account.")]
        public async Task DatabaseErrorTests(
            string errorMessage,
            string expectedError)
        {
            string userId = "123";
            TokenExchangeDataModel dataModel = new()
            {
                Accounts = new List<TokenExchangeAccount>
                {
                    new()
                    {
                        Id = "1",
                        Name = "Test",
                        Mask = "Hi!",
                        Type = "liability",
                        Subtype = "credit card",
                    },
                    new()
                    {
                        Id = "2",
                        Name = "Test 2",
                        Mask = "bye!",
                        Type = "depository",
                        Subtype = "checking",
                    }
                },
                PublicToken = "123",
            };

            TokenExchangeResponseModel responseModel = new()
            {
                ItemId = "1",
                AccessToken = "asdfklsadjfklasjdkl",
                RequestId = "kdkdkd"
            };

            DatabaseResponseModel databaseResponseModel = new()
            {
                Success = false,
                ErrorMessage = errorMessage
            };

            Mock<ITokenExchangeAccessor> tokenExchangeAccessor = new();
            Mock<IPlaidAccountAccessor> accountAccessorMock = new();

            tokenExchangeAccessor.Setup(
                x => x.PublicTokenExchangeAsync(It.IsAny<TokenExchangeRequestModel>()))
                .ReturnsAsync(responseModel);

            accountAccessorMock.Setup(
                x => x.InsertNewAccountAsync(It.IsAny<PlaidAccount>()))
                .ReturnsAsync(databaseResponseModel);

            TokenExchangeManager tokenExchangeManager = new(
                tokenExchangeAccessor.Object,
                accountAccessorMock.Object);

            BaseHttpResponse httpResponse = await tokenExchangeManager.PublicTokenExchangeAsync(userId, dataModel);
            
            Assert.IsNotNull(httpResponse);
            Assert.IsFalse(httpResponse.Success);
            Assert.AreEqual(httpResponse.Status, HttpStatusCode.InternalServerError);
            Assert.AreEqual(httpResponse.ErrorMessage, errorMessage);
        }
    }
}
