namespace SL.Application.Exceptions
{
    public class ValidationException : AppException
    {
        public ValidationException(string message, object? details = null)
            : base("VALIDATION_ERROR", message, "Girilen bilgiler geçersiz", details)
        {
        }
    }
}
