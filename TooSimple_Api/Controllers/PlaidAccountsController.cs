using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TooSimple_Managers.PlaidAccounts;
using TooSimple_Poco.Models.Dtos.PlaidAccounts;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Api.Controllers
{
    /// <summary>
    /// Request handler for adding & updating plaid accounts.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PlaidAccountsController : ControllerBase
    {
        private readonly IPlaidAccountManager _plaidAccountManager;

        public PlaidAccountsController(IPlaidAccountManager plaidAccountManager)
        {
            _plaidAccountManager = plaidAccountManager;
        }

        /// <summary>
        /// Retrieves plaid accounts for user.
        /// </summary>
        /// <param name="userId">
        /// User account Id to run against.
        /// </param>
        /// <returns>
        /// <see cref="GetPlaidAccountsDto"/> dto with accounts if successful
        /// or error messages if not.
        /// </returns>
        [HttpGet("userId/{userId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<GetPlaidAccountsDto> GetAccountsByUserAccountId(string userId)
        {
            GetPlaidAccountsDto response = await _plaidAccountManager.GetPlaidAccountsByUserIdAsync(userId);
            return response;
        }

        /// <summary>
        /// Retrieves an account and its transactions.
        /// </summary>
        /// <param name="accountId">
        /// Account Id to return
        /// </param>
        /// <returns></returns>
        [HttpGet("accountId/{accountId}")]
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
