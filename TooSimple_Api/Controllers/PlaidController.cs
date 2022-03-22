using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TooSimple_DataAccessors.Database.Logging;
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
        private readonly ILoggingAccessor _loggingAccessor;

        public PlaidController(ITokenExchangeManager tokenExchangeManager,
            ILoggingAccessor loggingAccessor)
        {
            _tokenExchangeManager = tokenExchangeManager;
            _loggingAccessor = loggingAccessor;
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

        /// <summary>
        /// Plaid sends this when there are new transactions to use.
        /// </summary>
        /// <param name="json"></param>
        [HttpPost("NewTransactions")]
        public async Task NewTransactions([FromBody] JsonElement json)
        {
            //temporarily calling accessor directly
            //to do fix this if it works
            var test = await _loggingAccessor.LogMessageAsync(null, json.ToString());
            var test2 = test;
        }
    }
}
