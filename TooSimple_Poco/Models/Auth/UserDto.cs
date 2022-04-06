using TooSimple_Poco.Models.Shared;

namespace TooSimple_Poco.Models.Auth
{
    public class UserDto
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
