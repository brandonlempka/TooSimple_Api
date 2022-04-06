using Microsoft.AspNetCore.Mvc;
using TooSimple_Managers.Budgeting;
using TooSimple_Poco.Models.Budgeting;

namespace TooSimple_Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
    public class BudgetingController : ControllerBase
	{
		private readonly IBudgetingManager _budgetingManager;

		public BudgetingController(IBudgetingManager budgetingManager)
		{
			_budgetingManager = budgetingManager;
		}

		[HttpGet("getReadyToSpend")]
		public async Task<ActionResult<decimal>> GetReadyToSpend(string userId)
        {
            decimal readyToSpend = await _budgetingManager.GetUserReadyToSpendAsync(userId);
			return Ok(readyToSpend);
        }

		[HttpGet("updateGoalFunding")]
		public async Task UpdateGoalFunding(string userId)
        {
			await _budgetingManager.UpdateBudgetByUserId(userId);
        }
 	}
}

