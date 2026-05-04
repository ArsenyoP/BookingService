namespace Booking.Domain.Common
{
    public interface IResult
    {
        bool IsSuccess { get; }
    }

    public class Result<T> : IResult
    {
        public bool IsSuccess { get; }
        public Error Error { get; }
        public T? Value { get; }

        public Result(bool isSuccess, Error error, T? value)
        {
            IsSuccess = isSuccess;
            Error = error;
            Value = value;
        }

        public static Result<T> Success(T value) => new Result<T>(true, Error.None, value);

        public static Result<T> Failure(Error error) => new Result<T>(false, error, default);
    }
    public class Result : IResult
    {
        public bool IsSuccess { get; }
        public Error Error { get; }

        private Result(bool isSuccess, Error error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, Error.None);
        public static Result Failure(Error error) => new(false, error);
    }
}
