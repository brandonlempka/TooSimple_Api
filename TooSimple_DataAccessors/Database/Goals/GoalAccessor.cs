using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using TooSimple_Poco.Models.Entities;
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
        public async Task<IEnumerable<Goal>> GetGoalsByUserIdAsync(string userId)
        {
            IEnumerable<Goal> goals = Enumerable.Empty<Goal>();
            using (MySqlConnection connection = new(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"SELECT
                    g.GoalId
                    , g.GoalName
                    , g.GoalAmount
                    , g.DesiredCompletionDate
                    , g.UserAccountId
                    , g.FundingScheduleId
                    , g.IsExpense
                    , g.RecurrenceTimeFrame
                    , g.CreationDate
                    , g.IsPaused
                    , g.AutoSpendMerchantName
                    , g.AmountContributed
                    , g.AmountSpent
                    , g.IsAutoRefillEnabled
                    , g.NextContributionAmount
                    , g.NextContributionDate
                    , g.IsContributionFixed
                    , g.IsArchived
                    FROM Goals g
                    WHERE g.UserAccountId = @userId";

                goals = await connection.QueryAsync<Goal>(
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
        public async Task<Goal?> GetGoalByGoalIdAsync(string goalId)
        {
            Goal goal = new();
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

                goal = await connection.QueryFirstOrDefaultAsync<Goal>(
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
        /// <param name="goal">
        /// Goal data model from user.
        /// </param>
        /// <returns>
        /// <see cref="DatabaseResponseModel"/> with success or any error messages.
        /// </returns>
        public async Task<DatabaseResponseModel> AddNewGoalAsync(Goal goal)
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
                        goal.GoalId,
                        goal.GoalName,
                        goal.GoalAmount,
                        goal.DesiredCompletionDate,
                        goal.UserAccountId,
                        goal.FundingScheduleId,
                        goal.IsExpense,
                        goal.RecurrenceTimeFrame,
                        goal.CreationDate,
                        goal.IsPaused,
                        goal.AutoSpendMerchantName,
                        goal.AmountContributed,
                        goal.AmountSpent,
                        goal.IsAutoRefillEnabled,
                        goal.NextContributionAmount,
                        goal.NextContributionDate,
                        goal.IsContributionFixed,
                        goal.IsArchived,
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
        public async Task<DatabaseResponseModel> UpdateGoalAsync(Goal goal)
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
        public async Task<IEnumerable<FundingHistory>> GetFundingHistoryByGoalId(string goalId)
        {
            IEnumerable<FundingHistory> histories = Enumerable
                .Empty<FundingHistory>();
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

                histories = await connection.QueryAsync<FundingHistory>(query, new { GoalId = goalId });
            }

            return histories;
        }

        /// <summary>
        /// Saves a move money request to move money between goals or to/from
        /// ready to spend (if source or destination is null).
        /// </summary>
        /// <param name="fundingHistory">
        /// <see cref="FundingHistory"/> information about source & destination goals.
        /// </param>
        /// <returns>
        /// <see cref="DatabaseResponseModel"/> with success or any error messages.
        /// </returns>
        public async Task<DatabaseResponseModel> SaveMoveMoneyAsync(FundingHistory fundingHistory)
        {
            using MySqlConnection connection = new(_connectionString);
            await connection.OpenAsync();
            using IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                if (!string.IsNullOrWhiteSpace(fundingHistory.SourceGoalId))
                {
                    Goal goal = new();
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

                    goal = await connection.QueryFirstOrDefaultAsync<Goal>(
                        query,
                        new
                        {
                            GoalId = fundingHistory.SourceGoalId
                        });

                    string sourceUpdateQuery = string.Empty;

                    if (goal.IsExpense)
                    {
                        sourceUpdateQuery = @"UPDATE Goals
                                            SET AmountContributed = AmountContributed - @Amount
                                            WHERE GoalId = @SourceGoalId";
                    }
                    else
                    {
                        sourceUpdateQuery = @"UPDATE Goals
                                            SET AmountSpent = AmountSpent + @Amount
                                            WHERE GoalId = @SourceGoalId";
                    }

                    await connection.ExecuteAsync(
                        sourceUpdateQuery,
                        new
                        {
                            fundingHistory.Amount,
                            fundingHistory.SourceGoalId
                        },
                        transaction);
                }

                if (!string.IsNullOrWhiteSpace(fundingHistory.DestinationGoalId))
                {

                    string destinationUpdateQuery = @"UPDATE Goals
                                            SET AmountContributed = AmountContributed + @Amount
                                            WHERE GoalId = @DestinationGoalId";

                    await connection.ExecuteAsync(
                        destinationUpdateQuery,
                        new
                        {
                            fundingHistory.Amount,
                            fundingHistory.DestinationGoalId
                        },
                        transaction);
                }

                string fundingHistoryQuery = @"INSERT INTO FundingHistory
                                            (
                                                FundingHistoryId
                                                , SourceGoalId
                                                , DestinationGoalId
                                                , Amount
                                                , TransferDate
                                                , Note
                                                , IsAutomatedTransfer
                                            )
                                            VALUES
                                            (
                                                @FundingHistoryId
                                                , @SourceGoalId
                                                , @DestinationGoalId
                                                , @Amount
                                                , @TransferDate
                                                , @Note
                                                , @IsAutomatedTransfer
                                            );";

                await connection.ExecuteAsync(
                    fundingHistoryQuery,
                    new
                    {
                        fundingHistory.FundingHistoryId,
                        fundingHistory.SourceGoalId,
                        fundingHistory.DestinationGoalId,
                        fundingHistory.Amount,
                        fundingHistory.TransferDate,
                        fundingHistory.Note,
                        fundingHistory.IsAutomatedTransfer
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
    }
}
