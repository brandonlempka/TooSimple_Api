using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TooSimple_Poco.Models.Shared;

namespace TooSimple_Api.Filters
{
    public class ResponseMap : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // This is required by IActionFilter.
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is not ObjectResult result
                || result.Value is not BaseHttpResponse baseHttpResponse
                || baseHttpResponse.Status == HttpStatusCode.OK)
            {
                return;
            }

            string responseName = baseHttpResponse
                .Status
                .ToString();
            int statusCode = (int)Enum.Parse(
                typeof(HttpStatusCode),
                responseName);

            result.StatusCode = statusCode;
        }
    }
}
