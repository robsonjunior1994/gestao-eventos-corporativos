using GestaoEventosCorporativos.Api._01_Presentation.Helpers;
using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Repositories;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Services;
using GestaoEventosCorporativos.Api._02_Core.Services;
using GestaoEventosCorporativos.Api._02_Core.Shared;
using Moq;

namespace GestaoEventosCorporativos.Tests._02_Core.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<IEncryptionPasswordService> _encryptionServiceMock;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _jwtServiceMock = new Mock<IJwtService>();
            _encryptionServiceMock = new Mock<IEncryptionPasswordService>();

            _service = new UserService(
                _userRepoMock.Object,
                _jwtServiceMock.Object,
                _encryptionServiceMock.Object
            );
        }

        [Fact]
        public async Task AddAsync_DeveRetornarErro_QuandoNomeVazio()
        {
            //Arrange
            var user = new User { Name = "", Email = "test@email.com", Password = "123" };

            //Act
            var result = await _service.AddAsync(user);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("O nome é obrigatório.", result.ErrorMessage);
        }

        [Fact]
        public async Task AddAsync_DeveRetornarErro_QuandoEmailVazio()
        {
            //Arrange
            var user = new User { Name = "Robson", Email = "", Password = "123" };

            //Act
            var result = await _service.AddAsync(user);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("O e-mail é obrigatório.", result.ErrorMessage);
        }

        [Fact]
        public async Task AddAsync_DeveRetornarErro_QuandoSenhaVazia()
        {
            //Arrange
            var user = new User { Name = "Robson", Email = "robson@email.com", Password = "" };

            //Act
            var result = await _service.AddAsync(user);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("A senha é obrigatória.", result.ErrorMessage);
        }

        [Fact]
        public async Task AddAsync_DeveRetornarErro_QuandoEmailExistente()
        {
            //Arrange
            var user = new User { Name = "Robson", Email = "robson@email.com", Password = "123" };
            _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email))
                         .ReturnsAsync(new User { Id = 1, Email = user.Email });

            //Act
            var result = await _service.AddAsync(user);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.RESOURCE_ALREADY_EXISTS, result.ErrorCode);
            Assert.Equal("Já existe um usuário com este e-mail.", result.ErrorMessage);
        }

        [Fact]
        public async Task AddAsync_DeveRetornarDatabaseError_QuandoRepositorioLancarExcecao()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Name = "Robson",
                Email = "robson@example.com",
                Password = "senha123"
            };

            _userRepoMock
                .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Falha inesperada no banco"));

            // Act
            var result = await _service.AddAsync(user);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.DATABASE_ERROR, result.ErrorCode);
            Assert.Equal("Erro ao criar usuário.", result.ErrorMessage);
        }

        [Fact]
        public async Task AddAsync_DeveCriarUsuario_QuandoValido()
        {
            //Arrange
            var user = new User { Name = "Robson", Email = "robson@email.com", Password = "123" };

            _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email))
                         .ReturnsAsync((User)null);

            _encryptionServiceMock.Setup(e => e.EncryptPassword(user.Password))
                                  .Returns("hashed_password");

            //Act
            var result = await _service.AddAsync(user);

            //Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("hashed_password", result.Data.Password);
            _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_DeveRetornarErro_QuandoUsuarioNaoEncontrado()
        {
            //Arrange
            _userRepoMock.Setup(r => r.GetByEmailAsync("robson@email.com"))
                         .ReturnsAsync((User)null);

            //Act
            var result = await _service.LoginAsync("robson@email.com", "123");

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Usuário não encontrado.", result.ErrorMessage);
        }

        [Fact]
        public async Task LoginAsync_DeveRetornarErro_QuandoSenhaInvalida()
        {
            //Arrange
            var user = new User { Id = 1, Email = "robson@email.com", Password = "hashed" };

            _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
            _encryptionServiceMock.Setup(e => e.ValidatePassword("123", user.Password))
                                  .Returns(false);

            //Act
            var result = await _service.LoginAsync(user.Email, "123");

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("Senha inválida.", result.ErrorMessage);
        }

        [Fact]
        public async Task LoginAsync_DeveRetornarToken_QuandoSucesso()
        {
            //Arrange
            var user = new User { Id = 1, Name = "Robson", Email = "robson@email.com", Password = "hashed" };

            _userRepoMock.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
            _encryptionServiceMock.Setup(e => e.ValidatePassword("123", user.Password))
                                  .Returns(true);

            _jwtServiceMock.Setup(j => j.GenerateToken(user))
                           .Returns(Result<string>.Success("jwt_token"));

            //Act
            var result = await _service.LoginAsync(user.Email, "123");

            //Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("jwt_token", result.Data);
        }

        [Fact]
        public async Task LoginAsync_DeveRetornarDatabaseError_QuandoRepositorioLancarExcecao()
        {
            // Arrange
            var email = "robson@example.com";
            var senha = "senha123";

            _userRepoMock
                .Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("Falha inesperada no banco"));

            // Act
            var result = await _service.LoginAsync(email, senha);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.DATABASE_ERROR, result.ErrorCode);
            Assert.Equal("Erro ao realizar login.", result.ErrorMessage);
        }
    }
}
