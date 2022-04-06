using TooSimple_Poco.Models.Auth;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Managers.Authorization
{
    public interface IAuthorizationManager
	{
		Task<BaseHttpResponse> RegisterUserAsync(UserDto userDto);
		Task<JwtDto> LoginUserAsync(UserDto userDto);
	}
}

