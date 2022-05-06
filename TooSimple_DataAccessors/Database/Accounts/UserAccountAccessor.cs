using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using TooSimple_Poco.Models.DataModels;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_DataAccessors.Database.Accounts
{
    public class UserAccountAccessor : IUserAccountAccessor
    {
        private readonly string _connectionString;

        public UserAccountAccessor(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TooSimpleMySql");
        }

        public async Task<DatabaseResponseModel> RegisterUserAsync(UserAccountDataModel userAccount)
        {
            using MySqlConnection connection = new(_connectionString);
            await connection.OpenAsync();
            using IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                string query = @"insert into UserAccounts 
                                (
                                    UserAccountId
                                    , UserName
                                    , NormalizedUserName
                                    , Email
                                    , NormalizedEmail
                                    , IsEmailConfirmed
                                    , PasswordHash
                                    , PasswordSalt
                                    , IsTwoFactorEnabled
                                    , FailedLoginCount
                                )
                                values 
                                (
                                    @UserAccountId
                                    , @UserName
                                    , @NormalizedUserName
                                    , @Email
                                    , @NormalizedEmail
                                    , @IsEmailConfirmed
                                    , @PasswordHash
                                    , @PasswordSalt
                                    , @IsTwoFactorEnabled
                                    , @FailedLoginCount
                                );";

                await connection.ExecuteAsync(
                    query,
                    new
                    {
                        userAccount.UserAccountId,
                        userAccount.UserName,
                        userAccount.NormalizedUserName,
                        userAccount.Email,
                        userAccount.NormalizedEmail,
                        userAccount.IsEmailConfirmed,
                        userAccount.PasswordHash,
                        userAccount.PasswordSalt,
                        userAccount.IsTwoFactorEnabled,
                        userAccount.FailedLoginCount,
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
        /// Retrieves a user by email address for logging in purposes.
        /// </summary>
        /// <param name="normalizedEmailAddress">
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
                    , PasswordSalt
                    , PhoneNumber
                    , IsPhoneConfirmed
                    , IsTwoFactorEnabled
                    , FailedLoginCount
                    FROM UserAccounts
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
