using TooSimple_DataAccessors.Plaid.TokenExchange;
using TooSimple_Poco.Models.Plaid.TokenExchange.PlaidLinkTokenModels;
using System.Net;
using TooSimple_Poco.Models.Shared;
using TooSimple_Poco.Models.Plaid.TokenExchange;
using TooSimple_DataAccessors.Database.PlaidAccounts;
using TooSimple_Poco.Models.Entities;
using TooSimple_Poco.Enums;

namespace TooSimple_Managers.Plaid.TokenExchange
{
    /// <summary>
    /// Logic layer for plaid account sign up and password refresh.
    /// </summary>
    public class TokenExchangeManager : ITokenExchangeManager
    {
        private readonly ITokenExchangeAccessor _tokenExchangeAccessor;
        private readonly IPlaidAccountAccessor _plaidAccountAccessor;
        public TokenExchangeManager(
            ITokenExchangeAccessor tokenExchangeAccessor,
            IPlaidAccountAccessor plaidAccountAccessor)
        {
            _tokenExchangeAccessor = tokenExchangeAccessor;
            _plaidAccountAccessor = plaidAccountAccessor;
        }

        /// <summary>
        /// Calls plaid API to retrieve plaid link token.
        /// </summary>
        /// <param name="userId">User's Too Simple ID.</param>
        /// <returns>Dto with plaid link token.</returns>
        public async Task<CreateLinkTokenDto> GetCreateLinkTokenAsync(string userId)
        {
            CreateLinkTokenResponseModel plaidResponse = await _tokenExchangeAccessor.
                CreateLinkTokenAsync(userId);

            if (string.IsNullOrWhiteSpace(plaidResponse.LinkToken))
            {
                CreateLinkTokenDto errorResponse = new()
                {
                    ErrorMessage = "Unable to contact plaid at this time.",
                    Success = false,
                    Status = HttpStatusCode.InternalServerError
                };

                if (plaidResponse is not null
                    && !string.IsNullOrWhiteSpace(plaidResponse.ErrorMessage))
                {
                    errorResponse.ErrorMessage = plaidResponse.ErrorMessage;
                }

                return errorResponse;
            }

            CreateLinkTokenDto responseDto = new()
            {
                LinkToken = plaidResponse.LinkToken,
                Success = true,
                Status = HttpStatusCode.OK
            };

            return responseDto;
        }

        /// <summary>
        /// Calls plaid to exchange public token for access token
        /// then saves output to local db.
        /// </summary>
        /// <param name="userId">
        /// User Id to associate new account to.
        /// </param>
        /// <param name="publicToken">
        /// Token exchange data model from client.
        /// </param>
        /// <returns>
        /// <see cref="BaseHttpResponse"/> indicating success or failure.
        /// </returns>
        public async Task<BaseHttpResponse> PublicTokenExchangeAsync(
            string userId,
            TokenExchangeDataModel publicToken)
        {
            if (string.IsNullOrWhiteSpace(publicToken.PublicToken)
                || publicToken.Accounts is null
                || publicToken.Accounts.Count <= 0)
            {
                return new BaseHttpResponse
                {
                    Status = HttpStatusCode.BadRequest,
                    ErrorMessage = "Public token was not formed correctly."
                };
            }

            TokenExchangeRequestModel requestModel = new(publicToken.PublicToken);
            TokenExchangeResponseModel response = await _tokenExchangeAccessor.PublicTokenExchangeAsync(requestModel);

            if (!string.IsNullOrWhiteSpace(response.ErrorMessage)
                || string.IsNullOrWhiteSpace(response.AccessToken)
                || string.IsNullOrWhiteSpace(response.ItemId))
            {
                return new BaseHttpResponse
                {
                    Status = HttpStatusCode.InternalServerError,
                    ErrorMessage = response.ErrorMessage
                };
            }

            string accessToken = response.AccessToken;

            foreach (TokenExchangeAccount account in publicToken.Accounts)
            {
                if (string.IsNullOrWhiteSpace(account.Id))
                {
                    return new BaseHttpResponse
                    {
                        ErrorMessage = "Plaid account ID did not return correctly.",
                        Status = HttpStatusCode.InternalServerError
                    };
                }

                PlaidAccountType accountType;
                switch (account.Subtype)
                {
                    case "checking":
                        accountType = PlaidAccountType.Checking;
                        break;
                    case "savings":
                        accountType = PlaidAccountType.Savings;
                        break;
                    case "credit card":
                        accountType = PlaidAccountType.CreditCard;
                        break;
                    default:
                        accountType = PlaidAccountType.UNKNOWN;
                        break;
                }

                PlaidAccount newAccount = new()
                {
                    AccessToken = accessToken,
                    AvailableBalance = 0M,
                    CurrentBalance = 0M,
                    PlaidAccountTypeId = (int)accountType,
                    IsActiveForBudgetingFeatures = true,
                    UserAccountId = userId,
                    PlaidAccountId = account.Id,
                    CurrencyCode = "USD",
                    ItemId = response.ItemId,
                    Name = account.Name ?? "New Account",
                    Mask = account.Mask ?? string.Empty,
                };

                DatabaseResponseModel dbResponse = await _plaidAccountAccessor.InsertNewAccountAsync(newAccount);
                if (!dbResponse.Success)
                {
                    return new()
                    {
                        ErrorMessage = dbResponse.ErrorMessage ?? "Something went while saving new account.",
                        Status = HttpStatusCode.InternalServerError
                    };
                }
            }

            return new()
            {
                Success = true,
                Status = HttpStatusCode.Created
            };
        }
    }
}
