using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TooSimple_Managers.Plaid.AccountUpdate;
using TooSimple_Managers.Plaid.TokenExchange;
using TooSimple_Poco.Models.Plaid.TokenExchange;
using TooSimple_Poco.Models.Plaid.TokenExchange.PlaidLinkTokenModels;
using TooSimple_Poco.Models.Shared;

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

        public PlaidController(
            ITokenExchangeManager tokenExchangeManager,
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
        [HttpGet("createLinkToken/{userId}")]
        public async Task<CreateLinkTokenDto> CreateLinkToken(string userId)
        {
            CreateLinkTokenDto tokenDto = await _tokenExchangeManager.GetCreateLinkTokenAsync(userId);
            return tokenDto;
        }

        /// <summary>
        /// Plaid link calls this with public token to exchange for access token
        /// which we store for future plaid requests.
        /// </summary>
        /// <param name="userId">
        /// User Id to attach account to.
        /// </param>
        /// <param name="dataModel">
        /// <see cref="TokenExchangeDataModel"/> from plaid link with
        /// account Ids user selected.
        /// </param>
        /// <returns>
        /// <see cref="BaseHttpResponse"/> base response indicating success
        /// or failure.
        /// </returns>
        [HttpPost("publicTokenExchange/{userId}")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<BaseHttpResponse> PlaidLinkTokenExchange(
            string userId,
            [FromBody] TokenExchangeDataModel dataModel)
        {
            BaseHttpResponse response = await _tokenExchangeManager.PublicTokenExchangeAsync(userId, dataModel);
            return response;
        }

        /// <summary>
        /// Plaid sends this when there are new transactions to use.
        /// </summary>
        /// <param name="json">
        /// Data from plaid.
        /// </param>
        /// <returns>        
        /// We always return a 200 because Plaid will keep retrying 
        /// until it gets a success.
        /// </returns>
        [HttpPost("webhookHandler")]
        public async Task<ActionResult> WebhookHandler([FromBody] JsonElement json)
        {
            _ = await _accountUpdateManager.PlaidSyncByItemIdAsync(json);

            return Ok();
        }
    }
}
