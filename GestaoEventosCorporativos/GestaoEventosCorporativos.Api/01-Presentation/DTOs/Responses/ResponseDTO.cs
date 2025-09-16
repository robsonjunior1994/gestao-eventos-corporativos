namespace GestaoEventosCorporativos.Api._01_Presentation.DTOs.Responses
{
    public class ResponseDTO
    {
        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }
        public string StatusCode { get; private set; }
        public object Data { get; private set; }
        public object Errors { get; private set; }

        public void Success(string message, string statusCode, object data = null)
        {
            IsSuccess = true;
            Message = message;
            StatusCode = statusCode;
            Data = data;
            Errors = null;
        }

        public void Failure(string message, string statusCode, object errors = null)
        {
            IsSuccess = false;
            Message = message;
            StatusCode = statusCode;
            Errors = errors;
            Data = null;
        }
    }
}
