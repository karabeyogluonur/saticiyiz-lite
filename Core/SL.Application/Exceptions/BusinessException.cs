namespace SL.Application.Exceptions
{
    public class BusinessException : AppException
    {
        public BusinessException(string message, string userMessage, object? details = null)
            : base("BUSINESS_ERROR", message, userMessage, details)
        {
        }
    }
}
