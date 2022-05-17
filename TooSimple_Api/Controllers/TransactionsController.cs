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
		private readonly ITransactionManager _transactionManager;
		public PlaidTransactionsController(ITransactionManager transactionManager)
		{
			_transactionManager = transactionManager;
		}

		[HttpPost("searchTransactions")]
		[ProducesResponseType(200)]
		[ProducesResponseType(204)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<GetTransactionsDto> SearchTransactions ([FromBody] GetTransactionsRequestModel requestModel)
		{
            GetTransactionsDto transactionsDto = await _transactionManager.GetTransactionsByUserIdAsync(requestModel);
			return transactionsDto;
		}
	}
}
