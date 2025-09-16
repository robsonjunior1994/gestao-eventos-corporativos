using System.Security.Claims;
using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Shared;

namespace GestaoEventosCorporativos.Api._02_Core.Interfaces.Services
{
    public interface IJwtService
    {
        Result<string> GenerateToken(User user);
        Result<ClaimsPrincipal> ValidateToken(string token);
        bool IsTokenValid(string token);

        string GetUserIdFromToken(string token);
        string GetUserEmailFromToken(string token);
        string GetUserNameFromToken(string token);
    }
}
