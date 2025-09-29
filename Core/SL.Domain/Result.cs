using SL.Domain.Enums.Common;

namespace SL.Domain
{
    public record Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public ErrorCode Code { get; }
        public string ErrorMessage { get; }
        protected Result(bool isSuccess, string errorMessage, ErrorCode code)
        {
            if (isSuccess && (errorMessage != null || code != ErrorCode.None))
                throw new InvalidOperationException("Başarılı bir sonuç hata bilgisi taşıyamaz.");
            if (!isSuccess && (errorMessage == null || code == ErrorCode.None))
                throw new InvalidOperationException("Başarısız bir sonuç ErrorCode içermelidir.");
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            Code = code;
        }
        public static Result Success() => new(true, null, ErrorCode.None);
        public static Result Failure(string errorMessage, ErrorCode code = ErrorCode.InternalServerError)
        {
            if (code == ErrorCode.None)
            {
                code = ErrorCode.InternalServerError;
            }
            return new Result(false, errorMessage, code);
        }
    }
    public record Result<T> : Result
    {
        public T Value { get; }
        private Result(bool isSuccess, T value, string errorMessage, ErrorCode code)
            : base(isSuccess, errorMessage, code)
        {
            Value = value;
        }
        public static Result<T> Success(T value) => new(true, value, null, ErrorCode.None);
        public new static Result<T> Failure(string errorMessage, ErrorCode code = ErrorCode.InternalServerError)
        {
            return new Result<T>(false, default, errorMessage, code);
        }
    }
}