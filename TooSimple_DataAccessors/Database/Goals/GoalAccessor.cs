using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using TooSimple_Poco.Models.Database;

namespace TooSimple_DataAccessors.Database.Goals
{
    public class GoalAccessor : IGoalAccessor
    {
        private readonly string _connectionString;

        public GoalAccessor(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TooSimpleMySql");
        }

        /// <summary>
        /// Retrieves goals from database by user id.
        /// </summary>
        /// <param name="userId">
        /// User ID to run against.
        /// </param>
        /// <returns>
        /// IEnumerable of Goals.
        /// </returns>
        public async Task<IEnumerable<GoalDataModel>> GetGoalsByUserIdAsync(string userId)
        {
            IEnumerable<GoalDataModel> goals = Enumerable.Empty<GoalDataModel>();
            using (MySqlConnection connection = new(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"SELECT
                    GoalId
                    , GoalName
                    , GoalAmount
                    , DesiredCompletionDate
                    , UserAccountId
                    , FundingScheduleId
                    , IsExpense
                    , RecurrenceTimeFrame
                    , CreationDate
                    , IsPaused
                    , AutoSpendMerchantName
                    , AmountContributed
                    , AmountSpent
                    , IsAutoRefillEnabled
                    , NextContributionAmount
                    , NextContributionDate
                    , IsContributionFixed
                    , IsArchived
                    FROM Goals
                    WHERE UserAccountId = @userId";

                goals = await connection.QueryAsync<GoalDataModel>(query
                    , new { userId });
            }

            return goals;
        }

        /// <summary>
        /// Gets goal by its goal ID. Returns 1 Goal.
        /// </summary>
        /// <param name="goalId">
        /// Goal ID to return.
        /// </param>
        /// <returns>
        /// Returns 1 Goal.
        /// </returns>
        public async Task<GoalDataModel> GetGoalByGoalIdAsync(string goalId)
        {
            GoalDataModel goal = new();
            using (MySqlConnection connection = new(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"SELECT
                    GoalId
                    , GoalName
                    , GoalAmount
                    , DesiredCompletionDate
                    , UserAccountId
                    , FundingScheduleId
                    , IsExpense
                    , RecurrenceTimeFrame
                    , CreationDate
                    , IsPaused
                    , AutoSpendMerchantName
                    , AmountContributed
                    , AmountSpent
                    , IsAutoRefillEnabled
                    , NextContributionAmount
                    , NextContributionDate
                    , IsContributionFixed
                    , IsArchived
                    FROM Goals
                    WHERE GoalId = @GoalId";

                goal = await connection.QueryFirstOrDefaultAsync<GoalDataModel>(query, new { GoalId = goalId });
            }

            return goal;
        }

        public async Task<bool> UpdateGoalAsync(GoalDataModel goal)
        {
            using MySqlConnection connection = new(_connectionString);
            await connection.OpenAsync();
            using IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                string query = @"UPDATE Goals
                    SET GoalName = @GoalName
                    , GoalAmount = @GoalAmount
                    , FundingScheduleId = @FundingScheduleId
                    , RecurrenceTimeFrame = @RecurrenceTimeFrame
                    , IsPaused = @IsPaused
                    , AutoSpendMerchantName = @AutoSpendMerchantName
                    , AmountContributed = @AmountContributed
                    , AmountSpent = @AmountSpent
                    , IsAutoRefillEnabled = @IsAutoRefillEnabled
                    , NextContributionAmount = @NextContributionAmount
                    , NextContributionDate = @NextContributionDate
                    , IsContributionFixed = @IsContributionFixed
                    , IsArchived = @IsArchived
                    WHERE GoalId = @GoalId";

                await connection.ExecuteAsync(query,
                    new
                    {
                        GoalName = goal.GoalName,
                        GoalAmount = goal.GoalAmount,
                        FundingScheduleId = goal.FundingScheduleId,
                        RecurrenceTimeFrame = goal.RecurrenceTimeFrame,
                        IsPaused = goal.IsPaused,
                        AutoSpendMerchantName = goal.AutoSpendMerchantName,
                        AmountContributed = goal.AmountContributed,
                        AmountSpent = goal.AmountSpent,
                        IsAutoRefillEnabled = goal.IsAutoRefillEnabled,
                        NextContributionAmount = goal.NextContributionAmount,
                        NextContributionDate = goal.NextContributionDate,
                        IsContributionFixed = goal.IsContributionFixed,
                        IsArchived = goal.IsArchived,
                        GoalId = goal.GoalId
                    },
                    transaction);

                transaction.Commit();
                return true;

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return false;
            }
        }

        /// <summary>
        /// Retrieves the funding history for a goal by its goal ID.
        /// </summary>
        /// <param name="goalId">
        /// Goal ID to return history for.
        /// </param>
        /// <returns>
        /// IEnumerable of the history data model.
        /// </returns>
        public async Task<IEnumerable<FundingHistoryDataModel>> GetFundingHistoryByGoalId(string goalId)
        {
            IEnumerable<FundingHistoryDataModel> histories = Enumerable
                .Empty<FundingHistoryDataModel>(); ;
            using (MySqlConnection connection = new(_connectionString))
            {
                string query = @"SELECT f.FundingHistoryId
                    , f.SourceGoalId
                    , (SELECT GoalName FROM Goals WHERE GoalId = f.SourceGoalId)
                        AS SourceGoalName
                    , f.DestinationGoalId
                    , (SELECT GoalName FROM Goals WHERE GoalId = f.DestinationGoalId)
                        AS DestinationGoalName
                    , f.Amount
                    , f.TransferDate
                    , f.Note
                    , f.IsAutomatedTransfer
                    FROM FundingHistory f
                    WHERE SourceGoalId = @GoalId
                    OR DestinationGoalId = @GoalId;";

                histories = await connection.QueryAsync<FundingHistoryDataModel>(query, new { GoalId = goalId });
            }

            return histories;
        }

        public async Task<IEnumerable<FundingScheduleDataModel>> GetFundingSchedulesByUserId(string userId)
        {
            IEnumerable<FundingScheduleDataModel> schedules;
            using (MySqlConnection connection = new(_connectionString))
            {
                string query = @"SELECT FundingScheduleId
                    , FundingScheduleName
                    , Frequency
                    , FirstContributionDate
                    , UserAccountId
                    FROM FundingSchedules
                    WHERE UserAccountId = @UserId;";

                schedules = await connection.QueryAsync<FundingScheduleDataModel>(query, new { UserId = userId });
            }

            return schedules;
        }

        public async Task<bool> SaveMoveMoneyAsync(FundingHistoryDataModel dataModel)
        {
            using MySqlConnection connection = new(_connectionString);
            await connection.OpenAsync();
            using IDbTransaction transaction = connection.BeginTransaction();

            try
            {

                if (dataModel.DestinationGoalId != "0"
                    && dataModel.SourceGoalId != "0")
                {
                    string sourceGoalQuery = @"update Goals
                        set AmountContributed =
                            (select AmountContributed - @ContributionAmount)
                        where GoalId = @GoalId";

                    await connection.ExecuteAsync(sourceGoalQuery,
                        new
                    {
                        ContributionAmount = dataModel.Amount,
                        GoalId = dataModel.SourceGoalId
                    },
                    transaction);

                    string destinationGoalQuery = @"update Goals
                        set AmountContributed =
                            (select AmountContributed + @ContributionAmount)
                        where GoalId = @GoalId";

                    await connection.ExecuteAsync(destinationGoalQuery, new
                    {
                        ContributionAmount = dataModel.Amount,
                        GoalId = dataModel.SourceGoalId
                    },
                    transaction);
                }
                else
                {
                    string operand = string.Empty;
                    string goalId = string.Empty;
                    if (dataModel.SourceGoalId == "0")
                    {
                        operand = "+";
                        goalId = dataModel.DestinationGoalId;
                    }
                    else
                    {
                        operand = "-";
                        goalId = dataModel.SourceGoalId;
                    }

                    string query = @$"update Goals
                        set AmountContributed =
                            (select AmountContributed {operand} @ContributionAmount)
                        where GoalId = @GoalId";

                    await connection.ExecuteAsync(query, new
                    {
                        ContributionAmount = dataModel.Amount,
                        GoalId = goalId
                    },
                    transaction);
                }

                string historyQuery = @"INSERT INTO FundingHistory
                        (FundingHistoryId
                        , SourceGoalId
                        , DestinationGoalId
                        , Amount
                        , TransferDate
                        , Note
                        , IsAutomatedTransfer)
                        VALUES
                        (@FundingHistoryId
                        , @SourceGoalId
                        , @DestinationGoalId
                        , @Amount
                        , @TransferDate
                        , @Note
                        , @IsAutomatedTransfer)";

                await connection.ExecuteAsync(historyQuery, new
                {
                    FundingHistoryId = Guid.NewGuid(),
                    SourceGoalId = dataModel.SourceGoalId,
                    DestinationGoalId = dataModel.DestinationGoalId,
                    Amount = dataModel.Amount,
                    TransferDate = dataModel.TransferDate,
                    Note = dataModel.Note,
                    IsAutomatedTransfer = dataModel.IsAutomatedTransfer
                },
                transaction);

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return false;
            }
        }
    }
}

