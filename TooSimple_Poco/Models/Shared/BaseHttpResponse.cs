using System.Net;

namespace TooSimple_Poco.Models.Shared
{
    public class BaseHttpResponse
	{
		public HttpStatusCode Status { get; set; }
		public string? ErrorMessage { get; set; }
		public bool Success { get; set; }

		/// <summary>
		/// Base constructor.
		/// </summary>
		public BaseHttpResponse() { }

		/// <summary>
		/// Creates a response item from database response.
		/// </summary>
		/// <param name="databaseResponseModel">
		/// <see cref="DatabaseResponseModel"/> Response from database.
		/// </param>
		/// <param name="httpStatusCode">
		/// <see cref="HttpStatusCode"/> optional param if overriding default 200
		/// status code is desired.
		/// </param>
		/// <returns>
		/// <see cref="BaseHttpResponse"/>.
		/// </returns>
		public static BaseHttpResponse CreateResponseFromDb(
			DatabaseResponseModel databaseResponseModel,
			HttpStatusCode httpStatusCode = HttpStatusCode.OK)
        {
			BaseHttpResponse response = new();

			if (!databaseResponseModel.Success)
            {
				response.Success = false;
				response.ErrorMessage = databaseResponseModel.ErrorMessage ?? "An unknown error occurred";
				response.Status = HttpStatusCode.InternalServerError;
				
				return response;
            }

			response.Success = databaseResponseModel.Success;
			response.Status = httpStatusCode;
			
			return response;
        }

		/// <summary>
        /// Creates a generic Ok response.
        /// </summary>
        /// <returns>
        /// <see cref="BaseHttpResponse"/> with <see cref="HttpStatusCode"/>
        /// OK and success set to true.
        /// </returns>
		public static BaseHttpResponse CreateOkResponse()
        {
			BaseHttpResponse response = new()
            {
				Status = HttpStatusCode.OK,
				Success = true
            };

			return response;
        }
	}
}
