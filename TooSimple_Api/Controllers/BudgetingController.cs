using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TooSimple_Managers.Budgeting;
using TooSimple_Managers.Plaid.AccountUpdate;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Api.Controllers
{
	[Authorize]
	[ApiController]
	[Route("api/[controller]")]
	public class BudgetingController : ControllerBase
	{
		private readonly IBudgetingManager _budgetingManager;
		private readonly IAccountUpdateManager _accountUpdateManager;

		public BudgetingController(
			IBudgetingManager budgetingManager,
			IAccountUpdateManager accountUpdateManager)
		{
			_budgetingManager = budgetingManager;
			_accountUpdateManager = accountUpdateManager;
		}

		[HttpGet("getReadyToSpend")]
		public async Task<ActionResult<decimal>> GetReadyToSpend(string userId)
		{
			decimal readyToSpend = await _budgetingManager.GetUserReadyToSpendAsync(userId);
			return Ok(readyToSpend);
		}

		[HttpGet("userId/{userId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<ActionResult<BaseHttpResponse>> ForcePlaidSync(string userId)
        {
			BaseHttpResponse response = await _accountUpdateManager.PlaidSyncByUserIdAsync(userId);
			return response;
        }
 	}
}
