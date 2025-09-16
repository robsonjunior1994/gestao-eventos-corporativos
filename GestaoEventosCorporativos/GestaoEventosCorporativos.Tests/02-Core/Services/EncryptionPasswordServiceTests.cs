using GestaoEventosCorporativos.Api._02_Core.Services;
using Xunit;

namespace GestaoEventosCorporativos.Tests._02_Core.Services
{
    public class EncryptionPasswordServiceTests
    {
        private readonly EncryptionPasswordService _service;

        public EncryptionPasswordServiceTests()
        {
            _service = new EncryptionPasswordService();
        }

        [Fact]
        public void EncryptPassword_DeveRetornarHashNaoNuloENaoIgualASenha()
        {
            // Arrange
            var senha = "MinhaSenhaForte123";

            // Act
            var hash = _service.EncryptPassword(senha);

            // Assert
            Assert.NotNull(hash);
            Assert.NotEqual(senha, hash);
        }

        [Fact]
        public void ValidatePassword_ComSenhaCorreta_DeveRetornarTrue()
        {
            // Arrange
            var senha = "SenhaSegura123!";
            var hash = _service.EncryptPassword(senha);

            // Act
            var resultado = _service.ValidatePassword(senha, hash);

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public void ValidatePassword_ComSenhaIncorreta_DeveRetornarFalse()
        {
            // Arrange
            var senhaCorreta = "SenhaCorreta123!";
            var senhaErrada = "SenhaErrada456!";
            var hash = _service.EncryptPassword(senhaCorreta);

            // Act
            var resultado = _service.ValidatePassword(senhaErrada, hash);

            // Assert
            Assert.False(resultado);
        }
    }
}
