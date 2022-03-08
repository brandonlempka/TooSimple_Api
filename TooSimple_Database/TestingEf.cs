using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooSimple_Database.Entities;

namespace TooSimple_Database
{
    public class TestingEf : ITestingEf
    {
        private TooSimpleDatabaseContext _db;

        public TestingEf(TooSimpleDatabaseContext db)
        {
            _db = db;
        }

        public async Task<Account> GetData()
        {
            var data = await _db.Accounts.FirstOrDefaultAsync();
            return data;
        }
    }
}
