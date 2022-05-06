using TooSimple_Poco.Models.Shared;

namespace TooSimple_Poco.Models.Dtos.Auth
{
    public class UserRequestDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
