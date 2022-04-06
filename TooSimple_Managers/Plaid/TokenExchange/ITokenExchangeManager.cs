using TooSimple_Poco.Models.Plaid.TokenExchange.PlaidLinkTokenModels;

namespace TooSimple_Managers.Plaid.TokenExchange
{
    public interface ITokenExchangeManager
    {
        Task<CreateLinkTokenDto> GetCreateLinkTokenAsync(string userId);
    }
}
