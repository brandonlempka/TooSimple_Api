﻿using TooSimple_Poco.Enums;
using TooSimple_Poco.Models.Entities;

namespace TooSimple_Poco.Models.DataModels
{
    public class GoalDataModel
    {
        public string GoalId { get; set; } = string.Empty;
        public string GoalName { get; set; } = string.Empty;
        public decimal GoalAmount { get; set; }
        public DateTime DesiredCompletionDate { get; set; }
        public string UserAccountId { get; set; } = string.Empty;
        public string FundingScheduleId { get; set; } = string.Empty;
        public FundingScheduleDataModel? FundingSchedule { get; set; }
        public bool IsExpense { get; set; }
        public int RecurrenceTimeFrame { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsPaused { get; set; }
        public string AutoSpendMerchantName { get; set; } = string.Empty;
        public decimal AmountContributed { get; set; }
        public decimal AmountSpent { get; set; }
        public bool IsAutoRefillEnabled { get; set; }
        public decimal NextContributionAmount { get; set; }
        public DateTime NextContributionDate { get; set; }
        public bool IsContributionFixed { get; set; }
        public bool IsArchived { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GoalDataModel() { }

        /// <summary>
        /// Constructor for converting from <see cref="Goal"/> entity to data model.
        /// </summary>
        /// <param name="goal">
        /// <see cref="Goal"/> entity from database.
        /// </param>
        public GoalDataModel(Goal goal)
        {
            GoalId = goal.GoalId;
            GoalName = goal.GoalName;
            GoalAmount = goal.GoalAmount;
            DesiredCompletionDate = goal.DesiredCompletionDate;
            UserAccountId = goal.UserAccountId;
            FundingScheduleId = goal.FundingScheduleId;
            IsExpense = goal.IsExpense;
            RecurrenceTimeFrame = goal.RecurrenceTimeFrame;
            CreationDate = goal.CreationDate;
            IsPaused = goal.IsPaused;
            AutoSpendMerchantName = goal.AutoSpendMerchantName;
            AmountContributed = goal.AmountContributed;
            AmountSpent = goal.AmountSpent;
            IsAutoRefillEnabled = goal.IsAutoRefillEnabled;
            NextContributionAmount = goal.NextContributionAmount;
            NextContributionDate = goal.NextContributionDate;
            IsContributionFixed = goal.IsContributionFixed;
            IsArchived = goal.IsArchived;
        }

        /// <summary>
        /// Determines the next contribution amount & date.
        /// </summary>
        /// <param name="currentDate">
        /// Optional parameter for unit testing against different dates.
        /// Ignore in caller to use DateTime.UtcNow.
        /// </param>
        /// <exception cref="Exception">
        /// If Funding schedule is not set correctly will throw error for logging.
        /// </exception>
        public void GetNextContribution(DateTime? currentDate = null)
        {
            if (FundingSchedule is null)
            {
                return;
            }

            if (IsPaused || IsArchived)
            {
                NextContributionAmount = 0.00M;
                NextContributionDate = DateTime.MinValue;
                return;
            }

            if (IsExpense)
                CalculateExpenseContributions();

            DateTime now = currentDate ?? DateTime.UtcNow.Date;

            if (now < DesiredCompletionDate.Date)
            {
                int numberOfDaysToCompletion = (DesiredCompletionDate - now).Days;
                int numberOfContributionsRemaining;
                DateTime nextContributionDate;

                switch (FundingSchedule.Frequency)
                {
                    default:
                        throw new Exception("Recurrence time frame was not set.");
                    case (int)ContributionSchedule.UNKNOWN:
                        throw new Exception("Unknown funding schedule.");
                    case (int)ContributionSchedule.Weekly:
                        numberOfContributionsRemaining = numberOfDaysToCompletion / 7;
                        nextContributionDate = FundingSchedule.FirstContributionDate;

                        while (nextContributionDate <= now)
                        {
                            nextContributionDate = nextContributionDate.AddDays(7);
                        }

                        break;
                    case (int)ContributionSchedule.BiWeekly:
                        numberOfContributionsRemaining = numberOfDaysToCompletion / 14;
                        nextContributionDate = FundingSchedule.FirstContributionDate;

                        while (nextContributionDate <= now)
                        {
                            nextContributionDate = nextContributionDate.AddDays(14);
                        }

                        break;
                    case (int)ContributionSchedule.Monthly:
                        numberOfContributionsRemaining = CalculateNumberOfMonthsBetween(
                            ContributionSchedule.Monthly,
                            now);
                        nextContributionDate = FundingSchedule.FirstContributionDate;

                        while (nextContributionDate <= now)
                        {
                            nextContributionDate = nextContributionDate.AddMonths(1);
                        }

                        break;
                    case (int)ContributionSchedule.BiMonthly:
                        numberOfContributionsRemaining = CalculateNumberOfMonthsBetween(
                            ContributionSchedule.BiMonthly,
                            now);
                        nextContributionDate = FundingSchedule.FirstContributionDate;

                        while (nextContributionDate <= now)
                        {
                            nextContributionDate = nextContributionDate.AddMonths(2);
                        }

                        break;
                    case (int)ContributionSchedule.LastDayOfMonth:
                        numberOfContributionsRemaining = CalculateNumberOfMonthsBetween(
                            ContributionSchedule.LastDayOfMonth,
                            now);
                        nextContributionDate = FundingSchedule.FirstContributionDate;

                        while (nextContributionDate < now)
                        {
                            nextContributionDate = nextContributionDate.AddMonths(1);
                            nextContributionDate = new DateTime(nextContributionDate.Year, nextContributionDate.Month, 1)
                                .AddMonths(1)
                                .AddDays(-1);
                        }

                        break;
                }

                NextContributionDate = nextContributionDate;

                if (IsContributionFixed)
                {
                    return;
                }

                if (IsAutoRefillEnabled)
                {
                    decimal amountLeftToSave = GoalAmount - AmountContributed + AmountSpent;
                    NextContributionAmount = Math.Round(amountLeftToSave / numberOfContributionsRemaining, 2);
                }
                else
                {
                    decimal amountLeftToSave = GoalAmount - AmountContributed;
                    NextContributionAmount = Math.Round(amountLeftToSave / numberOfContributionsRemaining, 2);
                }
            }
        }

        private void CalculateExpenseContributions()
        {

        }

        private int CalculateNumberOfMonthsBetween(
            ContributionSchedule fundingSchedule,
            DateTime currentDate)
        {
            int counter = 0;
            int numberOfMonthsToSkip = fundingSchedule == ContributionSchedule.BiMonthly
                ? 2
                : 1;

            while (currentDate < DesiredCompletionDate)
            {
                counter++;
                currentDate = currentDate.AddMonths(numberOfMonthsToSkip);
            }

            return counter;
        }
    }
}
