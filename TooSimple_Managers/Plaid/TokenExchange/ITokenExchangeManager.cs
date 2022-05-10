using TooSimple_Poco.Models.Plaid.TokenExchange;
using TooSimple_Poco.Models.Plaid.TokenExchange.PlaidLinkTokenModels;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Managers.Plaid.TokenExchange
{
    public interface ITokenExchangeManager
    {
        Task<CreateLinkTokenDto> GetCreateLinkTokenAsync(string userId);
        Task<BaseHttpResponse> PublicTokenExchangeAsync(string userId, TokenExchangeDataModel dataModel);
    }
}
