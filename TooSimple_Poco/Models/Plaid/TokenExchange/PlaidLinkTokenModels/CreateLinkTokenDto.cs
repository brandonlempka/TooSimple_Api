using TooSimple_Poco.Models.Shared;

namespace TooSimple_Poco.Models.Plaid.TokenExchange.PlaidLinkTokenModels
{
    public class CreateLinkTokenDto : BaseHttpResponse
    {
        public string? LinkToken { get; set; }
    }
}
