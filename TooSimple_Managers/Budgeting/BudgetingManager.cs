using TooSimple_DataAccessors.Database.Accounts;
using TooSimple_DataAccessors.Database.Goals;
using TooSimple_Poco.Enums;
using TooSimple_Poco.Models.DataModels;
using TooSimple_Poco.Models.Entities;

namespace TooSimple_Managers.Budgeting
{
    public class BudgetingManager : IBudgetingManager
    {
        private readonly IGoalAccessor _goalAccessor;
        private readonly IAccountAccessor _accountAccessor;

        public BudgetingManager(IGoalAccessor goalAccessor,
            IAccountAccessor accountAccessor)
        {
            _goalAccessor = goalAccessor;
            _accountAccessor = accountAccessor;
        }

        public async Task<decimal> GetUserReadyToSpendAsync(string userId)
        {
            IEnumerable<PlaidAccountDataModel> accounts = await _accountAccessor.GetPlaidAccountsByUserIdAsync(userId);
            IEnumerable<Goal> goals = await _goalAccessor.GetGoalsByUserIdAsync(userId);

            decimal accountTotal = accounts
                .Where(account => account.IsActiveForBudgetingFeatures
                    && account.PlaidAccountTypeId != (int)PlaidAccountType.CreditCard)
                .Select(account => account.AvailableBalance)
                .Sum() ?? 0;

            decimal creditTotal = accounts
                .Where(account => account.IsActiveForBudgetingFeatures
                    && account.PlaidAccountTypeId == (int)PlaidAccountType.CreditCard)
                .Select(account => account.CurrentBalance)
                .Sum() ?? 0;

            //decimal goalTotal = goals
            //    .Where(goal => !goal.IsArchived)
            //    .Select(goal => GetGoalBalance(goal))
            //    .Sum();

            decimal readyToSpend = accountTotal - creditTotal;
            return readyToSpend;
        }
    }
}
