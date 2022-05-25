using System.Net;
using TooSimple_DataAccessors.Database.PlaidAccounts;
using TooSimple_DataAccessors.Database.Transactions;
using TooSimple_Poco.Models.ApiRequestModels;
using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.DataModels;
using TooSimple_Poco.Models.Dtos.Transactions;
using TooSimple_Poco.Models.Entities;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Managers.Transactions
{
    public class PlaidTransactionManager : IPlaidTransactionManager
    {
        private readonly IPlaidTransactionAccessor _transactionsAccessor;
        private readonly IPlaidAccountAccessor _plaidAccountAccessor;
        public PlaidTransactionManager(
            IPlaidTransactionAccessor transactionsAccessor,
            IPlaidAccountAccessor plaidAccountAccessor)
        {
            _transactionsAccessor = transactionsAccessor;
            _plaidAccountAccessor = plaidAccountAccessor;
        }

        /// <summary>
        /// Filters transactions based on multiple optional filters.
        /// If all are omitted will return everything.
        /// </summary>
        /// <param name="requestModel">
        /// <see cref="GetTransactionsRequestModel"/> with optional parameters.
        /// Only UserId is required.
        /// </param>
        /// <returns>
        /// <see cref="GetTransactionsDto"/> Transactions response dto.
        /// </returns>
        public async Task<GetTransactionsDto> SearchPlaidTransactionsAsync(GetTransactionsRequestModel requestModel)
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

            IEnumerable<PlaidAccount> plaidAccounts = await _plaidAccountAccessor
                .GetPlaidAccountsByUserIdAsync(requestModel.UserId);

            if (!plaidAccounts.Any())
                return new GetTransactionsDto
                {
                    ErrorMessage = "Couldn't fetch accounts.",
                    Status = HttpStatusCode.InternalServerError
                };

            GetTransactionsDto responseDto = new()
            {
                Success = true,
                Status = HttpStatusCode.OK,
                Transactions = transactions.Select(transaction => new TransactionDataModel(
                    transaction,
                    plaidAccounts))
            };

            return responseDto;
        }

        /// <summary>
        /// Calls database to update transaction with new spending goal Id.
        /// </summary>
        /// <param name="requestModel">
        /// <see cref="UpdatePlaidTransactionRequestModel"/> request model with transaction Id
        /// & new goal id, if any.
        /// </param>
        /// <returns>
        /// <see cref="BaseHttpResponse"/> indicating success or failure.
        /// </returns>
        public async Task<BaseHttpResponse> UpdatePlaidTransactionAsync(UpdatePlaidTransactionRequestModel requestModel)
        {
            if (string.IsNullOrWhiteSpace(requestModel.PlaidTransactionId))
            {
                return new BaseHttpResponse
                {
                    ErrorMessage = "PlaidTransactionId is required.",
                    Status = HttpStatusCode.BadRequest
                };
            }

            DatabaseResponseModel databaseResponse = await _transactionsAccessor.UpdatePlaidTransactionAsync(requestModel);
            if (!databaseResponse.Success)
            {
                return new BaseHttpResponse
                {
                    ErrorMessage = databaseResponse.ErrorMessage ?? "Something went wrong while updating.",
                    Status = HttpStatusCode.InternalServerError
                };
            }

            return new BaseHttpResponse
            {
                Status = HttpStatusCode.OK,
                Success = true
            };
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
        private static GetTransactionsDto ValidateRequest(GetTransactionsRequestModel requestModel)
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
