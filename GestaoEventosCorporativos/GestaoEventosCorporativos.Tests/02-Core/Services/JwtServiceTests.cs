using GestaoEventosCorporativos.Api._01_Presentation.Helpers;
using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Services;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace GestaoEventosCorporativos.Tests._02_Core.Services
{
    public class JwtServiceTests
    {
        private readonly JwtService _jwtService;

        public JwtServiceTests()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Jwt:Secret", "supersecretkeyforsigningjwttokens12345"}, // precisa ter tamanho suficiente
                {"Jwt:Issuer", "test-issuer"},
                {"Jwt:Audience", "test-audience"},
                {"Jwt:ExpiresInHours", "1"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _jwtService = new JwtService(configuration);
        }

        private User CreateTestUser() =>
            new User { Id = 1, Email = "test@example.com", Name = "Test User", Password = "hashedpwd" };

        [Fact]
        public void GenerateToken_DeveRetornarErro_QuandoExcecaoLancada()
        {
            // Arrange: configuração inválida (chave secreta vazia causa erro)
            var inMemorySettings = new Dictionary<string, string>
            {
                {"Jwt:Secret", ""}, // inválido
                {"Jwt:Issuer", "test_issuer"},
                {"Jwt:Audience", "test_audience"},
                {"Jwt:ExpiresInHours", "1"}
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var jwtService = new JwtService(configuration);

            var user = new User { Id = 1, Email = "test@test.com", Name = "Test User" };

            // Act
            var result = jwtService.GenerateToken(user);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.INTERNAL_ERROR, result.ErrorCode);
            Assert.Equal("Erro ao gerar token JWT.", result.ErrorMessage);
        }

        [Fact]
        public void GenerateToken_DeveRetornarToken_QuandoUsuarioValido()
        {
            // Arrange
            var user = CreateTestUser();

            // Act
            var result = _jwtService.GenerateToken(user);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(string.IsNullOrEmpty(result.Data));
        }

        [Fact]
        public void ValidateToken_DeveRetornarPrincipal_QuandoTokenValido()
        {
            // Arrange
            var tokenResult = _jwtService.GenerateToken(CreateTestUser());
            var token = tokenResult.Data;

            // Act
            var result = _jwtService.ValidateToken(token);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("test@example.com", result.Data.FindFirst(ClaimTypes.Email)?.Value);
        }

        [Fact]
        public void ValidateToken_DeveRetornarErro_QuandoTokenInvalido()
        {
            // Arrange
            var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIn0.invalidsignature";

            // Act
            var result = _jwtService.ValidateToken(token);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.UNAUTHORIZED, result.ErrorCode);
        }

        [Fact]
        public void IsTokenValid_DeveRetornarTrue_QuandoTokenValido()
        {
            var token = _jwtService.GenerateToken(CreateTestUser()).Data;

            var isValid = _jwtService.IsTokenValid(token);

            Assert.True(isValid);
        }

        [Fact]
        public void IsTokenValid_DeveRetornarFalse_QuandoTokenInvalido()
        {
            var isValid = _jwtService.IsTokenValid("token_invalido");

            Assert.False(isValid);
        }

        [Fact]
        public void GetUserIdFromToken_DeveRetornarId_QuandoTokenValido()
        {
            var token = _jwtService.GenerateToken(CreateTestUser()).Data;

            var userId = _jwtService.GetUserIdFromToken(token);

            Assert.Equal("1", userId);
        }

        [Fact]
        public void GetUserEmailFromToken_DeveRetornarEmail_QuandoTokenValido()
        {
            var token = _jwtService.GenerateToken(CreateTestUser()).Data;

            var email = _jwtService.GetUserEmailFromToken(token);

            Assert.Equal("test@example.com", email);
        }

        [Fact]
        public void GetUserNameFromToken_DeveRetornarNome_QuandoTokenValido()
        {
            var token = _jwtService.GenerateToken(CreateTestUser()).Data;

            var name = _jwtService.GetUserNameFromToken(token);

            Assert.Equal("Test User", name);
        }
    }
}
