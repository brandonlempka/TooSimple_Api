
namespace TooSimple_Poco.Models.Shared
{
    public class DatabaseResponseModel
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public static DatabaseResponseModel CreateSuccess(string? successMessage = null)
        {
            DatabaseResponseModel responseModel = new()
            {
                Success = true,
                SuccessMessage = successMessage
            };

            return responseModel;
        }

        public static DatabaseResponseModel CreateError(string error)
        {
            if (string.IsNullOrWhiteSpace(error))
                error = "An unexpected error occurred.";

            DatabaseResponseModel responseModel = new()
            {
                ErrorMessage = error
            };

            return responseModel;
        }

        public static DatabaseResponseModel CreateError(Exception ex)
        {
            if (ex == null)
                return CreateError("Error: An unexpected error occurred.");

            string? errorMessage = ex.Message;

            if (string.IsNullOrWhiteSpace(errorMessage) && ex.InnerException != null)
                errorMessage = ex.InnerException.Message;
            if (string.IsNullOrWhiteSpace(errorMessage))
                errorMessage = "An unexpected error occurred.";

            DatabaseResponseModel responseModel = new()
            {
                ErrorMessage = "Error: " + errorMessage
            };

            return responseModel;
        }

    }
}
