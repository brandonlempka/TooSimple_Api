using System.Net;
using TooSimple_DataAccessors.Database.Goals;
using TooSimple_DataAccessors.Database.PlaidAccounts;
using TooSimple_DataAccessors.Database.Transactions;
using TooSimple_Poco.Enums;
using TooSimple_Poco.Models.ApiRequestModels;
using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.DataModels;
using TooSimple_Poco.Models.Dtos.Budgeting;
using TooSimple_Poco.Models.Entities;

namespace TooSimple_Managers.Budgeting
{
    public class BudgetingManager : IBudgetingManager
    {
        private readonly IGoalAccessor _goalAccessor;
        private readonly IPlaidAccountAccessor _plaidAccountAccessor;
        private readonly IPlaidTransactionAccessor _plaidTransactionAccessor;

        public BudgetingManager(
            IGoalAccessor goalAccessor,
            IPlaidAccountAccessor plaidAccountAccessor,
            IPlaidTransactionAccessor plaidTransactionAccessor)
        {
            _goalAccessor = goalAccessor;
            _plaidAccountAccessor = plaidAccountAccessor;
            _plaidTransactionAccessor = plaidTransactionAccessor;
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
    }
}
