using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using TooSimple_Poco.Models.Database;

namespace TooSimple_DataAccessors.Database.Accounts
{
    public class UserAccountAccessor : IUserAccountAccessor
    {
        private readonly string _connectionString;

        public UserAccountAccessor(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TooSimpleMySql");
        }

        /// <summary>
        /// Retrieves a user by email address for logging in purposes.
        /// </summary>
        /// <param name="emailAddress">
        /// Email address of user
        /// </param>
        /// <returns>
        /// <see cref="UserAccountDataModel"/> of user data.
        /// </returns>
        public async Task<UserAccountDataModel> GetUserAccountAsync(string normalizedEmailAddress)
        {
            UserAccountDataModel dataModel;
            using (MySqlConnection connection = new(_connectionString))
            {
                await connection.OpenAsync();
                string query = @"SELECT UserAccountId
                    , UserName
                    , NormalizedUserName
                    , Email
                    , NormalizedEmail
                    , IsEmailConfirmed
                    , PasswordHash
                    , PhoneNumber
                    , IsPhoneConfirmed
                    , IsTwoFactorEnabled
                    , FailedLoginCount
                    FROM UserAccounts;
                    WHERE NormalizedEmail = @normalizedEmailAddress";

                dataModel = await connection.QueryFirstOrDefaultAsync<UserAccountDataModel>(
                    query
                    , new
                    {
                        normalizedEmailAddress
                    });
            }

            return dataModel;
        }
    }
}
