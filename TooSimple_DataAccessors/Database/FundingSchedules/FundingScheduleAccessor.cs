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
            IEnumerable<FundingSchedule> fundingHistories = Enumerable.Empty<FundingSchedule>();
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

                fundingHistories = await connection.QueryAsync<FundingSchedule>(
                    query
                    , new
                    {
                        userId
                    });
            }

            return fundingHistories;
        }
    }
}
