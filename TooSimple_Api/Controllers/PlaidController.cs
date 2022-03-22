using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TooSimple_DataAccessors.Database.Logging;
using TooSimple_Managers.Plaid.AccountUpdate;
using TooSimple_Managers.Plaid.TokenExchange;
using TooSimple_Poco.Models.Plaid.TokenExchange.PlaidLinkTokenModels;

namespace TooSimple_Api.Controllers
{
    /// <summary>
    /// Request handler for adding & updating plaid accounts.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PlaidController : ControllerBase
    {
        private readonly ITokenExchangeManager _tokenExchangeManager;
        private readonly IAccountUpdateManager _accountUpdateManager;
        private readonly ILoggingAccessor _loggingAccessor;

        public PlaidController(ITokenExchangeManager tokenExchangeManager,
            IAccountUpdateManager accountUpdateManager)
        {
            _tokenExchangeManager = tokenExchangeManager;
            _accountUpdateManager = accountUpdateManager;
        }

        /// <summary>
        /// Calls plaid API to create a link token for adding new accounts.
        /// </summary>
        /// <param name="userId">User's Too Simple ID.</param>
        /// <returns>Dto with link token.</returns>
        [HttpGet("createLinkToken")]
        public async Task<CreateLinkTokenDto> CreateLinkToken(string userId)
        {
            return await _tokenExchangeManager.GetCreateLinkTokenAsync(userId);
        }

        /// <summary>
        /// Plaid sends this when there are new transactions to use.
        /// </summary>
        /// <param name="json"></param>
        [HttpPost("newTransactions")]
        public async Task NewTransactions([FromBody] JsonElement json)
        {
            var response = await _accountUpdateManager.UpdateAccountBalancesByItemIdAsync(json);
        }
    }
}
