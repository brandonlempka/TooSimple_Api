using TooSimple_Poco.Models.Shared;

namespace TooSimple_Poco.Models.Auth
{
    public class JwtDto : BaseHttpResponse
    {
        public string? BearerToken { get; set; }
    }
}
