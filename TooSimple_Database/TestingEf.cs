using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using MySql.Data.MySqlClient;
using TooSimple_Database.Entities;

namespace TooSimple_Database
{
    public class TestingEf : ITestingEf
    {
        string connectionString = "server=192.168.68.84;port=3306;database=Dev_TooSimple;user=brandon;password=0hje3zr!CK:)";
        public async Task<Account> GetData()
        {
            using (IDbConnection conn = new MySqlConnection(connectionString))
            {
                var account = conn.QueryFirstOrDefault<Account>
                    ("SELECT * FROM PlaidAccounts p join UserAccounts a on p.UserAccountId = a.UserAccountId  limit 1");
                return account;
            }   
        }
    }
}
