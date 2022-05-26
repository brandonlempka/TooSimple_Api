using System.Net;
using TooSimple_DataAccessors.Database.FundingSchedules;
using TooSimple_DataAccessors.Database.Goals;
using TooSimple_DataAccessors.Database.PlaidAccounts;
using TooSimple_DataAccessors.Database.Transactions;
using TooSimple_Poco.Enums;
using TooSimple_Poco.Models.ApiRequestModels;
using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.DataModels;
using TooSimple_Poco.Models.Dtos.Budgeting;
using TooSimple_Poco.Models.Entities;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Managers.Budgeting
{
    public class BudgetingManager : IBudgetingManager
    {
        private readonly IGoalAccessor _goalAccessor;
        private readonly IPlaidAccountAccessor _plaidAccountAccessor;
        private readonly IPlaidTransactionAccessor _plaidTransactionAccessor;
        private readonly IFundingScheduleAccessor _fundingScheduleAccessor;

        public BudgetingManager(
            IGoalAccessor goalAccessor,
            IPlaidAccountAccessor plaidAccountAccessor,
            IPlaidTransactionAccessor plaidTransactionAccessor,
            IFundingScheduleAccessor fundingScheduleAccessor)
        {
            _goalAccessor = goalAccessor;
            _plaidAccountAccessor = plaidAccountAccessor;
            _plaidTransactionAccessor = plaidTransactionAccessor;
            _fundingScheduleAccessor = fundingScheduleAccessor;
        }

        /// <summary>
        /// Retrieves user's Dashboard information.
        /// </summary>
        /// <param name="userId">
        /// User Id to run against.
        /// </param>
        /// <returns>
        /// <see cref="GetDashboardDto"/> dto with user's overview information.
        /// </returns>
        public async Task<GetDashboardDto> GetUserDashboardAsync(string userId)
        {
            IEnumerable<PlaidAccount> accounts = await _plaidAccountAccessor
                .GetPlaidAccountsByUserIdAsync(userId);

            IEnumerable<Goal> goals = await _goalAccessor.GetGoalsByUserIdAsync(userId);

            IEnumerable<PlaidAccount> creditAccounts = accounts
                .Where(account => account.IsActiveForBudgetingFeatures
                    && account.PlaidAccountTypeId == (int)PlaidAccountType.CreditCard);

            decimal accountTotal = accounts
                .Where(account => account.IsActiveForBudgetingFeatures
                    && account.PlaidAccountTypeId != (int)PlaidAccountType.CreditCard)
                .Select(account => account.AvailableBalance)
                .Sum() ?? 0;

            decimal creditTotal = accounts
                .Where(account => account.IsActiveForBudgetingFeatures
                    && account.PlaidAccountTypeId == (int)PlaidAccountType.CreditCard)
                .Select(account => account.CreditLimit - account.AvailableBalance)
                .Sum() ?? 0M;

            decimal goalTotal = goals
                .Where(goal => !goal.IsArchived
                    && !goal.IsExpense)
                .Select(goal => goal.AmountContributed - goal.AmountSpent)
                .Sum();

            decimal expenseTotal = goals
                .Where(expense => !expense.IsArchived
                    && expense.IsExpense)
                .Select(expense => expense.AmountContributed)
                .Sum();

            DateTime lastUpdated = accounts
                .Where(account => account.IsActiveForBudgetingFeatures)
                .Max(account => account.LastUpdated) ?? DateTime.MinValue;

            GetTransactionsRequestModel transactionsRequestModel = new()
            {
                UserId = userId
            };

            IEnumerable<PlaidTransaction> plaidTransactions = await _plaidTransactionAccessor
                .GetPlaidTransactionsByUserIdAsync(transactionsRequestModel);

            GetDashboardDto responseModel = new()
            {
                DepositoryAmount = accountTotal,
                CreditAmount = creditTotal,
                GoalAmount = goalTotal,
                ExpenseAmount = expenseTotal,
                ReadyToSpend = accountTotal - creditTotal - goalTotal - expenseTotal,
                Transactions = plaidTransactions.Select(transaction => new TransactionDataModel(transaction, accounts)),
                Goals = goals
                    .Where(goal => !goal.IsArchived)
                    .Select(goal => new GoalDataModel(goal)),
                Success = true,
                LastUpdated = lastUpdated,
                Status = HttpStatusCode.OK
            };

            return responseModel;
        }

        /// <summary>
        /// Saves a transfer request from client application.
        /// </summary>
        /// <param name="fundingHistoryDataModel">
        /// <see cref="FundingHistoryDataModel"/> data model with source/destination
        /// goal ID. If one of these is null it is giving money back to ready to spend.
        /// </param>
        /// <returns>
        /// <see cref="BaseHttpResponse"/> indicating success or failure and
        /// any error messages.
        /// </returns>
        public async Task<BaseHttpResponse> MoveMoneyAsync(FundingHistoryDataModel fundingHistoryDataModel)
        {
            if (string.IsNullOrWhiteSpace(fundingHistoryDataModel.SourceGoalId)
                && string.IsNullOrWhiteSpace(fundingHistoryDataModel.DestinationGoalId))
            {
                BaseHttpResponse invalidResponse = new()
                {
                    ErrorMessage = "Either Source or Destination is required.",
                    Status = HttpStatusCode.BadRequest,
                };

                return invalidResponse;
            }

            BaseHttpResponse serverError = new()
            {
                ErrorMessage = "Something went wrong while retrieving goals.",
                Status = HttpStatusCode.InternalServerError,
            };

            FundingHistory fundingHistory = new(fundingHistoryDataModel)
            {
                FundingHistoryId = Guid.NewGuid().ToString()
            };

            DatabaseResponseModel databaseResponseModel = await _goalAccessor.SaveMoveMoneyAsync(fundingHistory);

            if (!databaseResponseModel.Success)
            {
                return BaseHttpResponse.CreateResponseFromDb(databaseResponseModel);
            }

            DatabaseResponseModel nextContributionResponse = new();

            if (!string.IsNullOrWhiteSpace(fundingHistoryDataModel.SourceGoalId))
            {
                nextContributionResponse = await GetNextContributions(fundingHistoryDataModel.SourceGoalId);
            }

            if (!string.IsNullOrWhiteSpace(fundingHistoryDataModel.DestinationGoalId))
            {
                nextContributionResponse = await GetNextContributions(fundingHistoryDataModel.DestinationGoalId);
            }

            return BaseHttpResponse.CreateResponseFromDb(nextContributionResponse);
        }

        /// <summary>
        /// Updates the goals with next contributions.
        /// </summary>
        /// <param name="goalId">
        /// Goal ID to update.
        /// </param>
        /// <returns>
        /// <see cref="DatabaseResponseModel"/> indicating success or failure.
        /// </returns>
        private async Task<DatabaseResponseModel> GetNextContributions(string goalId)
        {
            Goal? goal = await _goalAccessor.GetGoalByGoalIdAsync(goalId);
            if (goal is null)
            {
                return new DatabaseResponseModel()
                {
                    ErrorMessage = "Something went wrong while getting next contribution.",
                    Success = false
                };
            }

            FundingSchedule fundingSchedule = await _fundingScheduleAccessor.GetFundingSchedulesByScheduleIdAsync(goal.FundingScheduleId);
            GoalDataModel goalDataModel = new(goal)
            {
                FundingSchedule = new(fundingSchedule)
            };

            goalDataModel.GetNextContribution();
            goal = new(goalDataModel);

            DatabaseResponseModel response = await _goalAccessor.UpdateGoalAsync(goal);
            return response;
        }
    }
}