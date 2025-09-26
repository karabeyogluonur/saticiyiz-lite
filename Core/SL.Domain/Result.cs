using System;
namespace SL.Domain
{
    public record Result
    {
        public bool IsSuccess { get; }
        public string ErrorMessage { get; }

        public static Result Success() => new(true, null);
        public static Result Failure(string errorMessage) => new(false, errorMessage);

        protected Result(bool isSuccess, string errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }
    }

    public record Result<T> : Result
    {
        public T Value { get; }

        public static Result<T> Success(T value) => new(true, value, null);
        public static Result<T> Failure(string errorMessage) => new(false, default, errorMessage);

        private Result(bool isSuccess, T value, string errorMessage) : base(isSuccess, errorMessage)
        {
            Value = value;
        }
    }
}

