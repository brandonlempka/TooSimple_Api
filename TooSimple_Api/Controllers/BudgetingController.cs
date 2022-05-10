using Microsoft.AspNetCore.Mvc;
using TooSimple_Managers.Budgeting;
using TooSimple_Managers.Plaid.AccountUpdate;

namespace TooSimple_Api.Controllers
{
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

		//[HttpGet("updateTransactions")]
		//public async Task UpdateTransactions(string userId = "1d4c76c2-148b-47b5-9a53-c29f3a233c80")
  //      {
		//	await _accountUpdateManager.GetNewTransactionsAsync(userId);
  //      }
 	}
}
