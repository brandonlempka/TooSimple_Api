using TooSimple_Poco.Models.Auth;

namespace TooSimple_Managers.Authorization
{
    public interface IAuthorizationManager
	{
		Task<JwtDto> RegisterUserAsync(UserDto userDto);
		Task<JwtDto> LoginUserAsync(UserDto userDto);
	}
}

