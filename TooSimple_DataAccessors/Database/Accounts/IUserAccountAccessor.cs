using TooSimple_Poco.Models.Database;

namespace TooSimple_DataAccessors.Database.Accounts
{
    public interface IUserAccountAccessor
	{
		Task<UserAccountDataModel> GetUserAccountAsync(string normalizedEmailAddress);
	}
}

