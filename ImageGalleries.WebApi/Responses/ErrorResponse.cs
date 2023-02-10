namespace ImageGalleries.WebApi.Responses
{
    public class ErrorResponse
    {
        public ErrorResponse() : this(new List<string>()) { }

        public ErrorResponse(string errorMessage) : this(new List<string>() { errorMessage }) { }

        public ErrorResponse(IEnumerable<string> errorMessages)
        {
            ErrorMessages = errorMessages;
        }

        public IEnumerable<string> ErrorMessages { get; set; }
    }
}
