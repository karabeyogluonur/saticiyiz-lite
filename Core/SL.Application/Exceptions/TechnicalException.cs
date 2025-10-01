namespace SL.Application.Exceptions
{
    public class TechnicalException : AppException
    {
        public TechnicalException(string message, object? details = null)
            : base("TECHNICAL_ERROR", message, "Sistem hatası oluştu", details)
        {
        }
    }
}
