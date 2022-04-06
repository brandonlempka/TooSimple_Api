﻿using TooSimple_Poco.Models.Plaid.TokenExchange.PlaidLinkTokenModels;

namespace TooSimple_DataAccessors.Plaid.TokenExchange
{
    public interface ITokenExchangeAccessor
    {
        Task<CreateLinkTokenResponseModel> CreateLinkTokenAsync(string userId);
    }
}
