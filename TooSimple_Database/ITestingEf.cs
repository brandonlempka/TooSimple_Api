using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooSimple_Database.Entities;

namespace TooSimple_Database
{
    public interface ITestingEf
    {
        Task<Account> GetData();

    }
}
