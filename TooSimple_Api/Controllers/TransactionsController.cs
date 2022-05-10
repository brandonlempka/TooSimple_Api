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
	public class TransactionsController
	{
		private readonly ITransactionManager _transactionManager;
		public TransactionsController(ITransactionManager transactionManager)
		{
			_transactionManager = transactionManager;
		}

		[HttpGet("userId/{userId}")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<GetTransactionsDto> GetTransactionsByUserId([FromBody] GetTransactionsRequestModel requestModel)
		{
            GetTransactionsDto transactionsDto = await _transactionManager.GetTransactionsByUserIdAsync(requestModel);
			return transactionsDto;
		}
	}
}
