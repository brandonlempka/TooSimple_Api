using System.Net;
using System.Text.Json;
using TooSimple_DataAccessors.Database.Logging;

namespace TooSimple_Api.Middleware
{
	public class ExceptionLoggerMiddleware
	{
		private readonly ILoggingAccessor _loggingAccessor;
		private readonly RequestDelegate _request;

		public ExceptionLoggerMiddleware(
			ILoggingAccessor loggingAccessor,
			RequestDelegate request)
		{
			_loggingAccessor = loggingAccessor;
			_request = request;
		}

		public async Task Invoke(HttpContext context)
        {
			try
            {
				await _request(context);
            }
			catch(Exception ex)
            {
				await _loggingAccessor.LogMessageAsync(ex.GetBaseException().Message);
				await HttpException(context, ex);
            }
        }

		private Task HttpException(HttpContext context, Exception exception)
        {
			HttpStatusCode statusCode = HttpStatusCode.InternalServerError;

			context.Response.StatusCode = (int)statusCode;
			context.Response.ContentType = "application/json";

			string responseMessage = JsonSerializer.Serialize(new
			{
				ErrorMessage = exception.GetBaseException().Message
			});

			return context.Response.WriteAsync(
				responseMessage,
				CancellationToken.None);
        }
	}
}
