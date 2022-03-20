using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TooSimple_Poco.Models.Plaid.TokenExchange.PlaidLinkTokenModels;

namespace TooSimple_DataAccessors.Plaid.TokenExchange
{
    public interface ITokenExchangeAccessor
    {
        Task<CreateLinkTokenResponseModel> CreateLinkTokenAsync(string userId);

    }
}
