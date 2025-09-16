namespace GestaoEventosCorporativos.Api._02_Core.Shared
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public string ErrorMessage { get; }
        public string ErrorCode { get; }
        public T Data { get; }

        private Result(bool isSuccess, T data, string errorMessage, string errorCode)
        {
            IsSuccess = isSuccess;
            Data = data;
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
        }

        public static Result<T> Success(T data) =>
            new Result<T>(true, data, null, null);

        public static Result<T> Failure(string message, string errorCode) =>
            new Result<T>(false, default, message, errorCode);
    }
}
