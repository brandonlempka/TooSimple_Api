using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TooSimple_Managers.Transactions;
using TooSimple_Poco.Models.ApiRequestModels;
using TooSimple_Poco.Models.Dtos.Transactions;

namespace TooSimple_Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class PlaidTransactionsController
	{
		private readonly IPlaidTransactionManager _transactionManager;
		public PlaidTransactionsController(IPlaidTransactionManager transactionManager)
		{
			_transactionManager = transactionManager;
		}

		/// <summary>
        /// Searches transactions using several optional filters.
        /// Only required property is UserId.
        /// </summary>
        /// <param name="requestModel">
        /// <see cref="GetTransactionsRequestModel"/> model with optional filters
        /// & required userId property.
        /// </param>
        /// <returns>
        /// <see cref="GetTransactionsDto"/> Dto with information about request
        /// and transactions.
        /// </returns>
		[HttpPost("searchTransactions")]
		[ProducesResponseType(200)]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<GetTransactionsDto> SearchTransactions ([FromBody] GetTransactionsRequestModel requestModel)
		{
            GetTransactionsDto transactionsDto = await _transactionManager.SearchPlaidTransactionsAsync(requestModel);
			return transactionsDto;
		}
	}
}
