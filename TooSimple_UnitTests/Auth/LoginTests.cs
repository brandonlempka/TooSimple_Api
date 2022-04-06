using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TooSimple_DataAccessors.Database.Accounts;
using TooSimple_Managers.Authorization;
using TooSimple_Poco.Models.Auth;
using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_UnitTests.Auth
{
    [TestClass]
    public class LoginTests
    {
        [TestMethod]
        public async Task LoginInvalidPasswordTest()
        {
            UserDto userDto = new()
            {
                UserName = "Tester@test.com",
                Password = "This is a test"
            };

            UserAccountDataModel? userAccountDataModel = new();

            Mock<IUserAccountAccessor> userAccountAccessorMock = new();

            userAccountAccessorMock.Setup(
                x => x.GetUserAccountAsync(It.IsAny<string>()))
                .ReturnsAsync(userAccountDataModel);

            Mock<IConfiguration> configurationMock = new();
            Mock<IConfigurationSection> configurationSectionMock = new();
            configurationSectionMock.Setup(
                a => a.Value).Returns("1234567891123456");

            configurationMock.Setup(
                c => c.GetSection(It.IsAny<string>()))
                .Returns(new Mock<IConfigurationSection>().Object);

            configurationMock.Setup(
                a => a.GetSection("AppSettings:Token"))
                .Returns(configurationSectionMock.Object);

            AuthorizationManager authorizationManager = new(
                configurationMock.Object,
                userAccountAccessorMock.Object);

            JwtDto response = await authorizationManager.LoginUserAsync(userDto);

            Assert.IsNotNull(response);
            Assert.IsFalse(response.Success);
            Assert.IsTrue(string.IsNullOrWhiteSpace(response.BearerToken));
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.ErrorMessage));
            Assert.AreEqual(response.ErrorMessage, "Invalid password.");
            Assert.AreEqual(response.Status, HttpStatusCode.BadRequest);
        }


        [TestMethod]
        public async Task LoginNoUserFoundTest()
        {
            UserDto userDto = new()
            {
                UserName = "Tester@test.com",
                Password = "This is a test"
            };

            UserAccountDataModel? userAccountDataModel = null;

            Mock<IUserAccountAccessor> userAccountAccessorMock = new();

            userAccountAccessorMock.Setup(
                x => x.GetUserAccountAsync(It.IsAny<string>()))
                .ReturnsAsync(userAccountDataModel);

            Mock<IConfiguration> configurationMock = new();
            Mock<IConfigurationSection> configurationSectionMock = new();
            configurationSectionMock.Setup(
                a => a.Value).Returns("1234567891123456");

            configurationMock.Setup(
                c => c.GetSection(It.IsAny<string>()))
                .Returns(new Mock<IConfigurationSection>().Object);

            configurationMock.Setup(
                a => a.GetSection("AppSettings:Token"))
                .Returns(configurationSectionMock.Object);

            AuthorizationManager authorizationManager = new(
                configurationMock.Object,
                userAccountAccessorMock.Object);

            JwtDto response = await authorizationManager.LoginUserAsync(userDto);

            Assert.IsNotNull(response);
            Assert.IsFalse(response.Success);
            Assert.IsTrue(string.IsNullOrWhiteSpace(response.BearerToken));
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.ErrorMessage));
            Assert.AreEqual(response.ErrorMessage, "No user was found with this email address.");
            Assert.AreEqual(response.Status, HttpStatusCode.NotFound);
        }
    }
}
