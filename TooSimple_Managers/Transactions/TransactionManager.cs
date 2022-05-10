using System.Net;
using TooSimple_DataAccessors.Database.Transactions;
using TooSimple_Poco.Models.ApiRequestModels;
using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.DataModels;
using TooSimple_Poco.Models.Dtos.Transactions;

namespace TooSimple_Managers.Transactions
{
    public class TransactionManager : ITransactionManager
    {
        private readonly ITransactionsAccessor _transactionsAccessor;

        public TransactionManager(ITransactionsAccessor transactionsAccessor)
        {
            _transactionsAccessor = transactionsAccessor;
        }

        public async Task<GetTransactionsDto> GetTransactionsByUserIdAsync(GetTransactionsRequestModel requestModel)
        {
            GetTransactionsDto validationResponse = ValidateRequest(requestModel);
            if (!validationResponse.Success)
                return validationResponse;

            IEnumerable<PlaidTransaction> transactions = await _transactionsAccessor
                .GetPlaidTransactionsByUserIdAsync(requestModel);

            if (!transactions.Any())
                return new GetTransactionsDto
                {
                    Success = true,
                    Status = HttpStatusCode.NoContent
                };

            GetTransactionsDto responseDto = new GetTransactionsDto
            {
                Success = true,
                Status = HttpStatusCode.OK,
                Transactions = transactions.Select(transaction => new TransactionDataModel(transaction))
            };

            return responseDto;
        }

        /// <summary>
        /// Validates the request is valid.
        /// </summary>
        /// <param name="requestModel">
        /// <see cref="GetTransactionsRequestModel"/> request model from client app.
        /// </param>
        /// <returns>
        /// <see cref="GetTransactionsDto"/> with errors if any, or success if we
        /// can proceed.
        /// </returns>
        private GetTransactionsDto ValidateRequest(GetTransactionsRequestModel requestModel)
        {
            List<string> errors = new();
            if (string.IsNullOrWhiteSpace(requestModel.UserId))
                errors.Add("User ID is required.");

            if (requestModel.StartDate.HasValue && requestModel.EndDate.HasValue)
            {
                if (requestModel.EndDate.Value < requestModel.StartDate.Value)
                    errors.Add("End date must be after start date.");
            }

            if (errors.Any())
            {
                return new GetTransactionsDto
                {
                    ErrorMessage = string.Join(", ", errors),
                    Status = HttpStatusCode.BadRequest,
                    Success = false
                };
            }

            return new GetTransactionsDto
            {
                Success = true
            };
        }
    }
}
