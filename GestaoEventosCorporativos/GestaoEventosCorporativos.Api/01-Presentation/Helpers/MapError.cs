namespace GestaoEventosCorporativos.Api._01_Presentation.Helpers
{
    public static class MapError
    {
        public static int MapErrorToStatusCode(string errorCode)
        {
            return errorCode switch
            {
                ErrorCode.NOT_FOUND => StatusCodes.Status404NotFound,
                ErrorCode.VALIDATION_ERROR => StatusCodes.Status400BadRequest,
                ErrorCode.RESOURCE_ALREADY_EXISTS => StatusCodes.Status409Conflict,
                ErrorCode.DATABASE_ERROR => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError
            };
        }
    }

    public static class ErrorCode
    {
        public const string NOT_FOUND = "NOT_FOUND";
        public const string VALIDATION_ERROR = "VALIDATION_ERROR";
        public const string RESOURCE_ALREADY_EXISTS = "RESOURCE_ALREADY_EXISTS";
        public const string DATABASE_ERROR = "DATABASE_ERROR";
    }
}
