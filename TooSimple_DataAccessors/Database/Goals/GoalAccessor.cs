using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using TooSimple_Poco.Models.Database;
using TooSimple_Poco.Models.Shared;

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

                goals = await connection.QueryAsync<GoalDataModel>(
                    query
                    , new 
                    { 
                        userId 
                    });
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
        public async Task<GoalDataModel?> GetGoalByGoalIdAsync(string goalId)
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

                goal = await connection.QueryFirstOrDefaultAsync<GoalDataModel>(
                    query, 
                    new 
                    { 
                        GoalId = goalId 
                    });
            }

            return goal;
        }

        /// <summary>
        /// Adds new goal to database.
        /// </summary>
        /// <param name="goalDataModel">
        /// Goal data model from user.
        /// </param>
        /// <returns>
        /// <see cref="DatabaseResponseModel"/> with success or any error messages.
        /// </returns>
        public async Task<DatabaseResponseModel> AddNewGoalAsync(GoalDataModel goalDataModel)
        {
            using MySqlConnection connection = new(_connectionString);
            await connection.OpenAsync();
            using IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                string query = @"insert into Goals
                            (
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
                            )
                            values
                            (
                                  @GoalId
                                , @GoalName
                                , @GoalAmount
                                , @DesiredCompletionDate
                                , @UserAccountId
                                , @FundingScheduleId
                                , @IsExpense
                                , @RecurrenceTimeFrame
                                , @CreationDate
                                , @IsPaused
                                , @AutoSpendMerchantName
                                , @AmountContributed
                                , @AmountSpent
                                , @IsAutoRefillEnabled
                                , @NextContributionAmount
                                , @NextContributionDate
                                , @IsContributionFixed
                                , @IsArchived
                            );";

                await connection.ExecuteAsync(
                    query,
                    new
                    {
                        GoalId = Guid.NewGuid(),
                        goalDataModel.GoalName,
                        goalDataModel.GoalAmount,
                        goalDataModel.DesiredCompletionDate,
                        goalDataModel.UserAccountId,
                        goalDataModel.FundingScheduleId,
                        goalDataModel.IsExpense,
                        goalDataModel.RecurrenceTimeFrame,
                        goalDataModel.CreationDate,
                        goalDataModel.IsPaused,
                        goalDataModel.AutoSpendMerchantName,
                        goalDataModel.AmountContributed,
                        goalDataModel.AmountSpent,
                        goalDataModel.IsAutoRefillEnabled,
                        goalDataModel.NextContributionAmount,
                        goalDataModel.NextContributionDate,
                        goalDataModel.IsContributionFixed,
                        goalDataModel.IsArchived,
                    },
                    transaction);

                transaction.Commit();
                return DatabaseResponseModel.CreateSuccess();

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return DatabaseResponseModel.CreateError(ex);
            }
        }

        /// <summary>
        /// Updates a goal with new data provided by user.
        /// </summary>
        /// <param name="goal">
        /// Goal with new data.
        /// </param>
        /// <returns>
        /// <see cref="DatabaseResponseModel"/> with success or any error messages.
        /// </returns>
        public async Task<DatabaseResponseModel> UpdateGoalAsync(GoalDataModel goal)
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

                await connection.ExecuteAsync(
                    query,
                    new
                    {
                        goal.GoalName,
                        goal.GoalAmount,
                        goal.FundingScheduleId,
                        goal.RecurrenceTimeFrame,
                        goal.IsPaused,
                        goal.AutoSpendMerchantName,
                        goal.AmountContributed,
                        goal.AmountSpent,
                        goal.IsAutoRefillEnabled,
                        goal.NextContributionAmount,
                        goal.NextContributionDate,
                        goal.IsContributionFixed,
                        goal.IsArchived,
                        goal.GoalId
                    },
                    transaction);

                transaction.Commit();
                return DatabaseResponseModel.CreateSuccess();

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return DatabaseResponseModel.CreateError(ex);
            }
        }

        /// <summary>
        /// Deletes a goal and removes reference from transactions.
        /// </summary>
        /// <param name="goalId">
        /// Goal ID to remove.
        /// </param>
        /// <returns>
        /// <see cref="DatabaseResponseModel"/> with success or any error messages.
        /// </returns>
        public async Task<DatabaseResponseModel> DeleteGoalAsync(string goalId)
        {
            using MySqlConnection connection = new(_connectionString);
            await connection.OpenAsync();
            using IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                string goalSql = "delete from Goals where goalId = @goalId";
                // todo once transactions are done. Also, remove from funding history?
                // affects other goals. hmm.
                // string transactionSql = @"update transactions"
                    //"set "
                await connection.ExecuteAsync(
                    goalSql,
                    new
                    {
                        goalId
                    },
                    transaction);

                transaction.Commit();
                return DatabaseResponseModel.CreateSuccess();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return DatabaseResponseModel.CreateError(ex);
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

