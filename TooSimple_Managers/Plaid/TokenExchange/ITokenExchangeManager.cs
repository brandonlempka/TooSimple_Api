using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooSimple_Poco.Models.Plaid.TokenExchange.PlaidLinkTokenModels;

namespace TooSimple_Managers.Plaid.TokenExchange
{
    public interface ITokenExchangeManager
    {
        Task<CreateLinkTokenDto> GetCreateLinkTokenAsync(string userId);
    }
}
