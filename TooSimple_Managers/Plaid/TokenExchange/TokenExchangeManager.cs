using TooSimple_DataAccessors.Plaid.TokenExchange;
using TooSimple_Poco.Models.Plaid.TokenExchange.PlaidLinkTokenModels;

namespace TooSimple_Managers.Plaid.TokenExchange
{
    /// <summary>
    /// Logic layer for plaid account sign up and password refresh.
    /// </summary>
    public class TokenExchangeManager : ITokenExchangeManager
    {
        private readonly ITokenExchangeAccessor _tokenExchangeAccessor;

        public TokenExchangeManager(ITokenExchangeAccessor tokenExchangeAccessor)
        {
            _tokenExchangeAccessor = tokenExchangeAccessor;
        }

        /// <summary>
        /// Calls plaid API to retrieve plaid link token.
        /// </summary>
        /// <param name="userId">User's Too Simple ID.</param>
        /// <returns>Dto with plaid link token.</returns>
        public async Task<CreateLinkTokenDto> GetCreateLinkTokenAsync(string userId)
        {
            var plaidResponse = await _tokenExchangeAccessor.CreateLinkTokenAsync(userId);
            if (plaidResponse is null)
            {
                return new CreateLinkTokenDto();
            }

            CreateLinkTokenDto responseDto = new()
            {
                LinkToken = plaidResponse.LinkToken
            };

            return responseDto;
        }
    }
}
