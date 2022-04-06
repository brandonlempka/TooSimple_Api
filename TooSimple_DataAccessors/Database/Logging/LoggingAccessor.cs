using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System.Data;

namespace TooSimple_DataAccessors.Database.Logging
{
    public class LoggingAccessor : ILoggingAccessor
    {
        private readonly string _connectionString;

        public LoggingAccessor(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TooSimpleMySql");
        }

        public async Task<bool> LogMessageAsync(string message, string? errorCode)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (IDbTransaction transaction = connection.BeginTransaction())
                {
                    string query = @"INSERT INTO Logging
                                    (Code
                                    , Message
                                    , MessageTime) 
                                    VALUES
                                    (@Code
                                    , @Message
                                    , @Time)";

                    try
                    {
                        await connection.ExecuteAsync(
                            query,
                            new
                            {
                                Code = errorCode,
                                Message = message,
                                Time = DateTime.UtcNow
                            },
                            transaction);

                        transaction.Commit();
                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }
    }
}
