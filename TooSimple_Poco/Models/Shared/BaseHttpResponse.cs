using System.Net;

namespace TooSimple_Poco.Models.Shared
{
    public class BaseHttpResponse
	{
		public HttpStatusCode Status { get; set; }
		public string? ErrorMessage { get; set; }
		public bool Success { get; set; }
	}
}

