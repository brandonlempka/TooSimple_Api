using TooSimple_Poco.Models.Dtos.Auth;

namespace TooSimple_Managers.Authorization
{
    public interface IAuthorizationManager
	{
		Task<JwtDto> RegisterUserAsync(UserRequestDto userDto);
		Task<JwtDto> LoginUserAsync(UserRequestDto userDto);
	}
}

