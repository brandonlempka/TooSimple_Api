using Microsoft.AspNetCore.Mvc;
using TooSimple_DataAccessors.Plaid.TokenExchange;
using TooSimple_Managers.Plaid.TokenExchange;
using TooSimple_Poco.Models.Plaid.TokenExchange.PlaidLinkTokenModels;

namespace TooSimple_Api.Controllers
{
    /// <summary>
    /// Request handler for adding & updating plaid accounts.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class PlaidController : ControllerBase
    {
        private readonly ITokenExchangeManager _tokenExchangeManager;

        public PlaidController(ITokenExchangeManager tokenExchangeManager)
        {
            _tokenExchangeManager = tokenExchangeManager;
        }

        /// <summary>
        /// Calls plaid API to create a link token for adding new accounts.
        /// </summary>
        /// <param name="userId">User's Too Simple ID.</param>
        /// <returns>Dto with link token.</returns>
        [HttpGet("CreateLinkToken")]
        public async Task<CreateLinkTokenDto> CreateLinkToken(string userId)
        {
            return await _tokenExchangeManager.GetCreateLinkTokenAsync(userId);
        }
    }
}
