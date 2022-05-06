using TooSimple_Poco.Models.Shared;

namespace TooSimple_Poco.Models.Dtos.Auth
{
    public class JwtDto : BaseHttpResponse
    {
        public string? BearerToken { get; set; }
    }
}
