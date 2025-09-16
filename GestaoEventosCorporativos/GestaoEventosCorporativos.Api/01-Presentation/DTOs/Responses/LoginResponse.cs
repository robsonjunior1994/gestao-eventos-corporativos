namespace GestaoEventosCorporativos.Api._01_Presentation.DTOs.Responses
{
    public class LoginResponse
    {
        public string Token { get; set; }

        public LoginResponse(string token)
        {
            Token = token;
        }
    }
}
