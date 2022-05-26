using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using TooSimple_Poco.Models.Entities;

namespace TooSimple_DataAccessors.Database.FundingSchedules
{
    public class FundingScheduleAccessor : IFundingScheduleAccessor
    {
        private readonly string _connectionString;

        public FundingScheduleAccessor(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TooSimpleMySql");
        }

        /// <summary>
        /// Retrieves funding histories from database by user id.
        /// </summary>
        /// <param name="userId">
        /// User ID to run against.
        /// </param>
        /// <returns>
        /// IEnumerable of Funding History.
        /// </returns>
        public async Task<IEnumerable<FundingSchedule>> GetFundingSchedulesByUserIdAsync(string userId)
        {
            IEnumerable<FundingSchedule> fundingSchedules = Enumerable.Empty<FundingSchedule>();
            using (MySqlConnection connection = new(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"SELECT
                                    FundingScheduleId
                                    , FundingScheduleName
                                    , Frequency
                                    , FirstContributionDate
                                    , UserAccountId
                                FROM FundingSchedules
                                WHERE UserAccountId = @userId;";

                fundingSchedules = await connection.QueryAsync<FundingSchedule>(
                    query
                    , new
                    {
                        userId
                    });
            }

            return fundingSchedules;
        }

        /// <summary>
        /// Retrieves single funding history by its Id.
        /// </summary>
        /// <param name="scheduleId">
        /// Funding Schedule ID to run against.
        /// </param>
        /// <returns>
        /// <see cref="FundingSchedule"/>
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<FundingSchedule> GetFundingSchedulesByScheduleIdAsync(string scheduleId)
        {
            FundingSchedule fundingSchedule = new();
            using (MySqlConnection connection = new(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"SELECT
                                    FundingScheduleId
                                    , FundingScheduleName
                                    , Frequency
                                    , FirstContributionDate
                                    , UserAccountId
                                FROM FundingSchedules
                                WHERE FundingScheduleId = @scheduleId;";

                fundingSchedule = await connection.QueryFirstOrDefaultAsync<FundingSchedule>(
                    query
                    , new
                    {
                        scheduleId
                    });
            }

            return fundingSchedule;
        }
    }
}
