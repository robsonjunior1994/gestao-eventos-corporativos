using GestaoEventosCorporativos.Api._01_Presentation.Helpers;
using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Services;
using GestaoEventosCorporativos.Api._02_Core.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GestaoEventosCorporativos.Api._02_Core.Services
{
    public class JwtService : IJwtService
    {
        private readonly string _secret;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiresInHours;

        public JwtService(IConfiguration configuration)
        {
            _secret = configuration["Jwt:Secret"]
                      ?? throw new ArgumentNullException("Jwt:Secret not configured");
            _issuer = configuration["Jwt:Issuer"]
                      ?? throw new ArgumentNullException("Jwt:Issuer not configured");
            _audience = configuration["Jwt:Audience"]
                      ?? throw new ArgumentNullException("Jwt:Audience not configured");
            _expiresInHours = int.Parse(configuration["Jwt:ExpiresInHours"] ?? "24");
        }

        public Result<string> GenerateToken(User user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secret);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(_expiresInHours),
                    Issuer = _issuer,
                    Audience = _audience,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return Result<string>.Success(tokenHandler.WriteToken(token));
            }
            catch (Exception)
            {
                return Result<string>.Failure("Erro ao gerar token JWT.", ErrorCode.INTERNAL_ERROR);
            }
        }

        public Result<ClaimsPrincipal> ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secret);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return Result<ClaimsPrincipal>.Success(principal);
            }
            catch (SecurityTokenException ex)
            {
                return Result<ClaimsPrincipal>.Failure($"Token inválido: {ex.Message}", ErrorCode.UNAUTHORIZED);
            }
            catch (Exception)
            {
                return Result<ClaimsPrincipal>.Failure("Erro ao validar token.", ErrorCode.INTERNAL_ERROR);
            }
        }

        public bool IsTokenValid(string token)
        {
            var result = ValidateToken(token);
            return result.IsSuccess;
        }

        public string GetUserIdFromToken(string token)
        {
            var result = ValidateToken(token);
            return result.IsSuccess
                ? result.Data?.FindFirst(ClaimTypes.NameIdentifier)?.Value
                : null;
        }

        public string GetUserEmailFromToken(string token)
        {
            var result = ValidateToken(token);
            return result.IsSuccess
                ? result.Data?.FindFirst(ClaimTypes.Email)?.Value
                : null;
        }

        public string GetUserNameFromToken(string token)
        {
            var result = ValidateToken(token);
            return result.IsSuccess
                ? result.Data?.FindFirst(ClaimTypes.Name)?.Value
                : null;
        }
    }
}
