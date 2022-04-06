using TooSimple_Poco.Models.Auth;

namespace TooSimple_Managers.Authorization
{
    public interface IAuthorizationManager
	{
		Task<UserDto> LoginUserAsync(string emailAddress, string password)
	}
}

