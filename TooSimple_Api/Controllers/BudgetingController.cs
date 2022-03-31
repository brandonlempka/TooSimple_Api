using Microsoft.AspNetCore.Mvc;
using TooSimple_Managers.Budgeting;
using TooSimple_Poco.Models.Budgeting;
using TooSimple_Poco.Models.Database;

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

		/// <summary>
        /// Retrieves all goals for user.
        /// </summary>
        /// <param name="userId">
        /// ID of user to return goals for.
        /// </param>
        /// <returns>
        /// DTO containing IEnumerable of goals.
        /// </returns>
		[HttpGet("goals/userId/{userId}")]
		public async Task<GetGoalsDto> GetGoalsByUserId(string userId)
        {
            GetGoalsDto goalsDto = await _budgetingManager.GetGoalsByUserIdAsync(userId);
			return goalsDto;
        }

        /// <summary>
        /// Retrieves single goal and history by goal ID.
        /// </summary>
        /// <param name="goalId">
        /// Goal ID to return.
        /// </param>
        /// <returns>
        /// DTO containing goal and IEnumerable of goal contribution history.
        /// </returns>
        [HttpGet("goal/goalId/{goalId}")]
        public async Task<GetGoalDto> GetGoalByGoalId(string goalId)
        {
            GetGoalDto goalDto = await _budgetingManager.GetGoalByGoalIdAsync(goalId);
            return goalDto;
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

