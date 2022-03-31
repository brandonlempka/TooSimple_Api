using System.Net;
using TooSimple_DataAccessors.Database.Accounts;
using TooSimple_DataAccessors.Database.Goals;
using TooSimple_Poco.Enums;
using TooSimple_Poco.Models.Database;

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
            IEnumerable<GoalDataModel> goals = await _goalAccessor.GetGoalsByUserIdAsync(userId);

            decimal accountTotal = accounts
                .Where(account => account.IsActiveForBudgetingFeatures
                    && account.PlaidAccountTypeId != PlaidAccountType.CreditCard)
                .Select(account => account.AvailableBalance)
                .Sum() ?? 0;

            decimal creditTotal = accounts
                .Where(account => account.IsActiveForBudgetingFeatures
                    && account.PlaidAccountTypeId == PlaidAccountType.CreditCard)
                .Select(account => account.CurrentBalance)
                .Sum() ?? 0;

            decimal goalTotal = goals
                .Where(goal => !goal.IsArchived)
                .Select(goal => GetGoalBalance(goal))
                .Sum();

            decimal readyToSpend = accountTotal - creditTotal - goalTotal;
            return readyToSpend;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="today">
        /// This is provided for unit testing purposes.
        /// Optional parameter that can be ignored when calling outside unit tests.
        /// </param>
        /// <returns></returns>
        public async Task UpdateBudgetByUserId(string userId, DateTime? today = null)
        {
            today = today.HasValue
                ? today.Value.Date
                : DateTime.UtcNow.Date;

            IEnumerable<GoalDataModel> goals = await _goalAccessor.GetGoalsByUserIdAsync(userId);
            IEnumerable<FundingScheduleDataModel>? schedules = await _goalAccessor.GetFundingSchedulesByUserId(userId);

            goals = goals.Where(goal => !goal.IsPaused);

            foreach (GoalDataModel goal in goals)
            {
                if ((goal.DesiredCompletionDate >= today || goal.IsExpense)
                    && goal.NextContributionDate <= today)
                {
                    foreach (FundingScheduleDataModel schedule in schedules)
                    {
                        if (schedule.FundingScheduleId == goal.FundingScheduleId)
                        {
                            DateTime nextContributionDate = goal.NextContributionDate;
                            decimal nextContributionAmount = 0;

                            GoalDataModel currentGoal = goal;
                            IEnumerable<FundingHistoryDataModel> fundingHistory = await _goalAccessor.GetFundingHistoryByGoalId(currentGoal.GoalId);
                            IOrderedEnumerable<FundingHistoryDataModel> goalHistory = fundingHistory
                                .Where(g => g.DestinationGoalId == goal.GoalId && g.IsAutomatedTransfer == true)
                                .OrderByDescending(g => g.TransferDate);

                            DateTime lastFunded = today.Value;
                            if (goalHistory.Any())
                            {
                                lastFunded = goalHistory.Max(g => g.TransferDate);
                            }
                            else
                            {
                                lastFunded = DateTime.MinValue;
                            }

                            while (nextContributionDate <= today)
                            {
                                FundingHistoryDataModel requestModel = new()
                                {
                                    Amount = currentGoal.NextContributionAmount,
                                    IsAutomatedTransfer = true,
                                    SourceGoalId = "0",
                                    DestinationGoalId = currentGoal.GoalId,
                                    Note = "Automatic funding from " + schedule.FundingScheduleName,
                                    TransferDate = currentGoal.NextContributionDate
                                };

                                bool response = await _goalAccessor.SaveMoveMoneyAsync(requestModel);
                                lastFunded = currentGoal.NextContributionDate;

                                currentGoal = await _goalAccessor.GetGoalByGoalIdAsync(currentGoal.GoalId);
                                (nextContributionAmount, nextContributionDate) = CalculateNextContribution(currentGoal, schedule, lastFunded);

                                if (!response)
                                    break;


                                currentGoal.NextContributionAmount = nextContributionAmount;
                                currentGoal.NextContributionDate = nextContributionDate;

                                var responseModel = await _goalAccessor.UpdateGoalAsync(currentGoal);
                                if (!responseModel.Success)
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private decimal GetGoalBalance(GoalDataModel goal)
        {
            decimal goalBalance = goal.AmountContributed - goal.AmountSpent;
            return goalBalance;
        }

        private (decimal nextContributionAmount, DateTime nextContributionDate) CalculateNextContribution(
            GoalDataModel goal,
            FundingScheduleDataModel schedule,
            DateTime todayDate)
        {
            var nextContributionDate = goal.NextContributionDate;
            var contributionFrequency = schedule.Frequency;
            var numberOfContributionsRemaining = 0;
            var completionDate = goal.DesiredCompletionDate;
            var nextContributionAmount = 0.00M;
            var recurrence = goal.RecurrenceTimeFrame;

            todayDate = todayDate.Date;

            //if new goal, figure out first contribution date
            if (goal.NextContributionDate == DateTime.MinValue)
            {
                var scheduleDate = schedule.FirstContributionDate;
                if (scheduleDate > todayDate)
                {
                    nextContributionDate = schedule.FirstContributionDate;
                }
                else
                {
                    while (todayDate >= scheduleDate)
                        switch (contributionFrequency)
                        {
                            case 1:
                                scheduleDate = scheduleDate.AddDays(7).Date;
                                break;
                            case 2:
                                scheduleDate = scheduleDate.AddDays(14).Date;
                                break;
                            case 3:
                                scheduleDate = scheduleDate.AddMonths(1).Date;
                                break;
                            case 4:
                                scheduleDate = scheduleDate.AddMonths(2).Date;
                                break;
                                //to do
                                //case 5:
                                //    var newMonth = nextContribution.AddDays(1);
                                //    nextContribution = new DateTime(newMonth.Day,
                                //        newMonth.Month,
                                //        DateTime.DaysInMonth(newMonth.AddDays(1).Year, newMonth.Month));
                                //break;
                        }
                }

                nextContributionDate = scheduleDate;
            }
            else
            {
                switch (contributionFrequency)
                {
                    case 1:
                        nextContributionDate = nextContributionDate.AddDays(7).Date;
                        break;
                    case 2:
                        nextContributionDate = nextContributionDate.AddDays(14).Date;
                        break;
                    case 3:
                        nextContributionDate = nextContributionDate.AddMonths(1).Date;
                        break;
                    case 4:
                        nextContributionDate = nextContributionDate.AddMonths(2).Date;
                        break;
                        //to do
                        //case 5:
                        //    var newMonth = nextContribution.AddDays(1);
                        //    nextContribution = new DateTime(newMonth.Day,
                        //        newMonth.Month,
                        //        DateTime.DaysInMonth(newMonth.AddDays(1).Year, newMonth.Month));
                        //break;
                }
            }

            if (goal.IsExpense)
            {
                if (completionDate <= todayDate)
                {

                    if (recurrence == 1)
                    {
                        while (todayDate >= completionDate)
                        {
                            completionDate = completionDate.AddDays(7);
                        }
                    }
                    else if (recurrence == 2)
                    {
                        while (todayDate >= completionDate)
                        {
                            completionDate = completionDate.AddDays(14);
                        }
                    }
                    else if (recurrence == 3)
                    {
                        while (todayDate >= completionDate)
                        {
                            completionDate = completionDate.AddMonths(1);
                        }
                    }
                    else if (recurrence == 4)
                    {
                        while (todayDate >= completionDate)
                        {
                            completionDate = completionDate.AddMonths(2);
                        }
                    }
                    else if (recurrence == 5)
                    {
                        while (todayDate >= completionDate)
                        {
                            completionDate = completionDate.AddMonths(3);
                        }
                    }
                    else if (recurrence == 6)
                    {
                        while (todayDate >= completionDate)
                        {
                            completionDate = completionDate.AddMonths(6);
                        }
                    }
                    else if (recurrence == 7)
                    {
                        while (todayDate >= completionDate)
                        {
                            completionDate = completionDate.AddYears(1);
                        }
                    }
                }
            }

            var counterDate = nextContributionDate;

            switch (contributionFrequency)
            {
                case 1:
                    while (completionDate >= counterDate)
                    {
                        counterDate = counterDate.AddDays(7);
                        numberOfContributionsRemaining++;
                    }
                    break;
                case 2:
                    while (completionDate >= counterDate)
                    {
                        counterDate = counterDate.AddDays(14);
                        numberOfContributionsRemaining++;
                    }
                    break;
                case 3:
                    while (completionDate >= counterDate)
                    {
                        counterDate = counterDate.AddMonths(1);
                        numberOfContributionsRemaining++;
                    }
                    break;
                case 4:
                    while (completionDate >= counterDate)
                    {
                        counterDate = counterDate.AddMonths(2);
                        numberOfContributionsRemaining++;
                    }
                    break;
            }

            if (numberOfContributionsRemaining > 0)
            {
                nextContributionAmount = Math.Round((goal.GoalAmount - goal.AmountContributed) / numberOfContributionsRemaining, 2);
                if (nextContributionAmount < 0)
                {
                    nextContributionAmount = 0;
                }
            }
            else
            {
                nextContributionAmount = 0;
            }

            return (nextContributionAmount, nextContributionDate);
        }
    }
}
