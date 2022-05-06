using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TooSimple_DataAccessors.Database.Accounts;
using TooSimple_Managers.Authorization;
using TooSimple_Poco.Models.DataModels;
using TooSimple_Poco.Models.Dtos.Auth;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_UnitTests.Auth
{
    [TestClass]
	public class RegistrationTests
	{
		[TestMethod]
		public async Task RegistrationSuccessTest()
        {
			UserRequestDto userDto = new()
			{
				UserName = "Tester@test.com",
				Password = "This is a test"
			};

			DatabaseResponseModel databaseResponseModel = DatabaseResponseModel
				.CreateSuccess();

			Mock<IUserAccountAccessor> userAccountAccessorMock = new();

			userAccountAccessorMock.Setup(
				x => x.RegisterUserAsync(It.IsAny<UserAccountDataModel>()))
				.ReturnsAsync(databaseResponseModel);

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

			JwtDto response = await authorizationManager.RegisterUserAsync(userDto);

			Assert.IsNotNull(response);
			Assert.IsTrue(response.Success);
			Assert.IsFalse(string.IsNullOrWhiteSpace(response.BearerToken));
			Assert.IsTrue(string.IsNullOrWhiteSpace(response.ErrorMessage));
			Assert.AreEqual(response.Status, HttpStatusCode.Created);
		}

		[DataTestMethod]
		[DataRow("", "", 2)]
		[DataRow("username@test.com", "", 1)]
		[DataRow("", "password", 1)]
		public async Task RegistrationValidationErrorsTest(
			string userName,
			string password,
			int numberOfErrors)
        {
			UserRequestDto userDto = new()
			{
				UserName = userName,
				Password = password
			};

			Mock<IUserAccountAccessor> userAccountAccessorMock = new();

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

			JwtDto response = await authorizationManager.RegisterUserAsync(userDto);

			Assert.IsNotNull(response);
			Assert.IsNotNull(response.ErrorMessage);
			Assert.IsFalse(response.Success);
			// Have to subtract 1 from number of expected errors since I'm counting
			// the commas.
			Assert.AreEqual(
				response.ErrorMessage.Count(c => c == ','),
				numberOfErrors - 1);
			Assert.AreEqual(response.Status, HttpStatusCode.BadRequest);
		}

		[TestMethod]
		public async Task RegistrationDuplicateUserTest()
		{
			UserRequestDto userDto = new()
			{
				UserName = "tester@test.com",
				Password = "This is a test"
			};

			DatabaseResponseModel databaseResponseModel = DatabaseResponseModel
				.CreateError("Error: Duplicate entry 'TESTER@TEST.COM' for key 'NormalizedUserName'");

			Mock<IUserAccountAccessor> userAccountAccessorMock = new();

			userAccountAccessorMock.Setup(
				x => x.RegisterUserAsync(It.IsAny<UserAccountDataModel>()))
				.ReturnsAsync(databaseResponseModel);

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

			JwtDto response = await authorizationManager.RegisterUserAsync(userDto);

			Assert.IsNotNull(response);
			Assert.IsFalse(response.Success);
			Assert.IsTrue(string.IsNullOrWhiteSpace(response.BearerToken));
			Assert.AreEqual(
				response.ErrorMessage,
				"Error: Duplicate entry 'TESTER@TEST.COM' for key 'NormalizedUserName'");
			Assert.AreEqual(response.Status, HttpStatusCode.InternalServerError);
		}

		[TestMethod]
		public async Task BadEmailAddressTest()
		{
			UserRequestDto userDto = new()
			{
				UserName = "tester",
				Password = "This is a test"
			};

			DatabaseResponseModel databaseResponseModel = DatabaseResponseModel
				.CreateSuccess();

			Mock<IUserAccountAccessor> userAccountAccessorMock = new();

			userAccountAccessorMock.Setup(
				x => x.RegisterUserAsync(It.IsAny<UserAccountDataModel>()))
				.ReturnsAsync(databaseResponseModel);

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

			JwtDto response = await authorizationManager.RegisterUserAsync(userDto);

			Assert.IsNotNull(response);
			Assert.IsFalse(response.Success);
			Assert.IsTrue(string.IsNullOrWhiteSpace(response.BearerToken));
			Assert.AreEqual(
				response.ErrorMessage,
				"tester is not a valid email address.");
			Assert.AreEqual(response.Status, HttpStatusCode.BadRequest);
		}
	}
}
