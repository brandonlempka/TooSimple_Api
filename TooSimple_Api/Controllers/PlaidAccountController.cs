using Microsoft.AspNetCore.Mvc;
using TooSimple_Managers.PlaidAccounts;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Api.Controllers
{
    /// <summary>
    /// Request handler for adding & updating plaid accounts.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PlaidAccountController : ControllerBase
    {
        private readonly IPlaidAccountManager _plaidAccountManager;

        public PlaidAccountController(IPlaidAccountManager plaidAccountManager)
        {
            _plaidAccountManager = plaidAccountManager;
        }

        /// <summary>
        /// Retrieves an account and its transactions.
        /// </summary>
        /// <param name="accountId">
        /// Account Id to return
        /// </param>
        /// <returns></returns>
        [HttpGet]
        public void GetAccountByAccountId(string accountId)
        {
            // todo
            return;
        }

        /// <summary>
        /// Deletes an account and its associated transactions.
        /// </summary>
        /// <param name="accountId">
        /// Account Id to delete
        /// </param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<BaseHttpResponse> DeleteAccountByAccountId(string accountId)
        {
            BaseHttpResponse response = await _plaidAccountManager.DeleteAccountAsync(accountId);
            return response;
        }
    }
}
