namespace SL.Application.Exceptions
{
    public abstract class AppException : Exception
    {
        public string ErrorCode { get; }
        public string UserMessage { get; }
        public object? Details { get; }

        protected AppException(string errorCode, string message, string userMessage, object? details = null)
            : base(message)
        {
            ErrorCode = errorCode;
            UserMessage = userMessage;
            Details = details;
        }
    }
}
