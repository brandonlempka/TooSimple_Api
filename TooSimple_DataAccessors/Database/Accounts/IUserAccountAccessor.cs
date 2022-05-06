using TooSimple_Poco.Models.DataModels;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_DataAccessors.Database.Accounts
{
    public interface IUserAccountAccessor
	{
		Task<DatabaseResponseModel> RegisterUserAsync(UserAccountDataModel userAccount);
		Task<UserAccountDataModel?> GetUserAccountAsync(string normalizedEmailAddress);
	}
}

