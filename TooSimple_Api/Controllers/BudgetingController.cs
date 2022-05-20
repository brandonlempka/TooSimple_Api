using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TooSimple_Managers.Budgeting;
using TooSimple_Managers.Plaid.AccountUpdate;
using TooSimple_Poco.Models.Dtos.Budgeting;
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

		/// <summary>
        /// Forces a refresh of plaid transactions.
        /// </summary>
        /// <param name="userId">
        /// User Id to run against.
        /// </param>
        /// <returns>
        /// <see cref="BaseHttpResponse"/> including response info
        /// & error messages if applicable.
        /// </returns>
		[HttpGet("userId/{userId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<BaseHttpResponse> ForcePlaidSync(string userId)
        {
			BaseHttpResponse response = await _accountUpdateManager.PlaidSyncByUserIdAsync(userId);
			return response;
        }

		/// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
		[HttpGet("getDashboard/{userId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(500)]
		public async Task<GetDashboardDto> GetDashboardDto(string userId)
        {
            GetDashboardDto response = await _budgetingManager.GetUserDashboardAsync(userId);
			return response;
        }
 	}
}
