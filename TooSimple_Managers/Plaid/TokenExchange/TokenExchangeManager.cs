using TooSimple_DataAccessors.Plaid.TokenExchange;
using TooSimple_Poco.Models.Plaid.TokenExchange.PlaidLinkTokenModels;
using System.Net;
using TooSimple_Poco.Models.Shared;
using TooSimple_Poco.Models.Plaid.TokenExchange;

namespace TooSimple_Managers.Plaid.TokenExchange
{
    /// <summary>
    /// Logic layer for plaid account sign up and password refresh.
    /// </summary>
    public class TokenExchangeManager : ITokenExchangeManager
    {
        private readonly ITokenExchangeAccessor _tokenExchangeAccessor;

        public TokenExchangeManager(ITokenExchangeAccessor tokenExchangeAccessor)
        {
            _tokenExchangeAccessor = tokenExchangeAccessor;
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

        public async Task<BaseHttpResponse> PublicTokenExchangeAsync(
            string userId,
            TokenExchangeDataModel dataModel)
        {
            if (string.IsNullOrWhiteSpace(dataModel.PublicToken))
                return new BaseHttpResponse
                {
                    Status = HttpStatusCode.BadRequest,
                    ErrorMessage = "Public token was not formed correctly."
                };
            
            TokenExchangeRequestModel requestModel = new(dataModel.PublicToken);
            TokenExchangeResponseModel response = await _tokenExchangeAccessor.PublicTokenExchangeAsync(requestModel);

            if (!string.IsNullOrWhiteSpace(response.ErrorMessage))
                return new BaseHttpResponse
                {
                    Status = HttpStatusCode.InternalServerError,
                    ErrorMessage = response.ErrorMessage
                }
        }
    }
}
