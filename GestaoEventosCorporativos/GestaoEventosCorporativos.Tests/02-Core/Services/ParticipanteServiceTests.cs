using GestaoEventosCorporativos.Api._01_Presentation.Helpers;
using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Enums;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Repositories;
using GestaoEventosCorporativos.Api._02_Core.Services;
using GestaoEventosCorporativos.Api._02_Core.Shared;
using Moq;

namespace GestaoEventosCorporativos.Tests._02_Core.Services
{
    public class ParticipanteServiceTests
    {
        private readonly Mock<IParticipanteRepository> _participanteRepoMock;
        private readonly ParticipanteService _service;

        public ParticipanteServiceTests()
        {
            _participanteRepoMock = new Mock<IParticipanteRepository>();
            _service = new ParticipanteService(_participanteRepoMock.Object);
        }

        [Fact]
        public async Task AddAsync_DeveRetornarErro_QuandoNomeVazio()
        {
            var participante = new Participante { CPF = "12345678901", Tipo = TipoParticipante.Interno };

            var result = await _service.AddAsync(participante);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
        }

        [Fact]
        public async Task AddAsync_DeveRetornarErro_QuandoCpfVazio()
        {
            var participante = new Participante { NomeCompleto = "Robson", Tipo = TipoParticipante.Interno };

            var result = await _service.AddAsync(participante);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
        }

        [Fact]
        public async Task AddAsync_DeveRetornarErro_QuandoTipoInvalido()
        {
            var participante = new Participante { NomeCompleto = "Robson", CPF = "12345678901", Tipo = (TipoParticipante)999 };

            var result = await _service.AddAsync(participante);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
        }

        [Fact]
        public async Task AddAsync_DeveRetornarErro_QuandoCpfExistente()
        {
            var participante = new Participante { NomeCompleto = "Robson", CPF = "12345678901", Tipo = TipoParticipante.Interno };

            _participanteRepoMock.Setup(r => r.GetByCpfAsync(participante.CPF))
                                 .ReturnsAsync(participante);

            var result = await _service.AddAsync(participante);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.RESOURCE_ALREADY_EXISTS, result.ErrorCode);
        }

        [Fact]
        public async Task AddAsync_DeveCriarParticipante_QuandoValido()
        {
            var participante = new Participante { NomeCompleto = "Robson", CPF = "12345678901", Tipo = TipoParticipante.Interno };

            _participanteRepoMock.Setup(r => r.GetByCpfAsync(participante.CPF))
                                 .ReturnsAsync((Participante)null);

            var result = await _service.AddAsync(participante);

            Assert.True(result.IsSuccess);
            Assert.Equal(participante, result.Data);
            _participanteRepoMock.Verify(r => r.AddAsync(participante), Times.Once);
        }

        [Fact]
        public async Task AddAsync_DeveRetornarDatabaseError_QuandoRepositorioLancarExcecao()
        {
            // Arrange
            var participante = new Participante
            {
                Id = 1,
                NomeCompleto = "João Silva",
                CPF = "12345678901",
                Telefone = "11999999999",
                Tipo = TipoParticipante.Interno
            };

            _participanteRepoMock
                .Setup(r => r.GetByCpfAsync(participante.CPF))
                .ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            var result = await _service.AddAsync(participante);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.DATABASE_ERROR, result.ErrorCode);
            Assert.Equal("Erro ao criar participante.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateAsync_DeveRetornarErro_QuandoParticipanteNaoExiste()
        {
            var participante = new Participante { Id = 1, NomeCompleto = "Robson", CPF = "12345678901", Tipo = TipoParticipante.Interno };

            _participanteRepoMock.Setup(r => r.GetByIdAsync(participante.Id))
                                 .ReturnsAsync((Participante)null);

            var result = await _service.UpdateAsync(participante);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
        }

        [Fact]
        public async Task UpdateAsync_DeveRetornarErro_QuandoCpfExistente()
        {
            var participante = new Participante { Id = 1, NomeCompleto = "Robson", CPF = "12345678901", Tipo = TipoParticipante.Interno };

            _participanteRepoMock.Setup(r => r.GetByIdAsync(participante.Id))
                                 .ReturnsAsync(new Participante { Id = 1, CPF = "99999999999" });

            _participanteRepoMock.Setup(r => r.GetByCpfAsync(participante.CPF))
                                 .ReturnsAsync(new Participante { Id = 2, CPF = participante.CPF });

            var result = await _service.UpdateAsync(participante);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.RESOURCE_ALREADY_EXISTS, result.ErrorCode);
        }

        [Fact]
        public async Task DeleteAsync_DeveRetornarErro_QuandoNaoEncontrado()
        {
            _participanteRepoMock.Setup(r => r.GetByIdAsync(1))
                                 .ReturnsAsync((Participante)null);

            var result = await _service.DeleteAsync(1);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Participante não encontrado.", result.ErrorMessage);
        }

        [Fact]
        public async Task DeleteAsync_DeveRemoverParticipante_QuandoEncontrado()
        {
            var participante = new Participante { Id = 1, NomeCompleto = "Robson", CPF = "12345678901" };

            _participanteRepoMock.Setup(r => r.GetByIdAsync(participante.Id))
                                 .ReturnsAsync(participante);

            var result = await _service.DeleteAsync(participante.Id);

            Assert.True(result.IsSuccess);
            _participanteRepoMock.Verify(r => r.DeleteAsync(participante), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeveRetornarDatabaseError_QuandoRepositorioLancarExcecao()
        {
            // Arrange
            int participanteId = 1;

            _participanteRepoMock
                .Setup(r => r.GetByIdAsync(participanteId))
                .ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            var result = await _service.DeleteAsync(participanteId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.DATABASE_ERROR, result.ErrorCode);
            Assert.Equal("Erro ao excluir participante.", result.ErrorMessage);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarParticipantesComSucesso()
        {
            // Arrange
            int pageNumber = 1, pageSize = 2;
            var participantes = new List<Participante>
            {
                new Participante { Id = 1, NomeCompleto = "João da Silva", CPF = "12345678901" },
                new Participante { Id = 2, NomeCompleto = "Maria Souza", CPF = "98765432100" }
            };

            _participanteRepoMock
                .Setup(r => r.GetAllAsync(pageNumber, pageSize))
                .ReturnsAsync((participantes, participantes.Count));

            // Act
            var result = await _service.GetAllAsync(pageNumber, pageSize);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.TotalCount);
            Assert.Equal(pageNumber, result.Data.PageNumber);
            Assert.Equal(pageSize, result.Data.PageSize);
            Assert.Equal(2, result.Data.Items.Count());
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarErro_QuandoRepositorioLancarExcecao()
        {
            // Arrange
            _participanteRepoMock
                .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Erro no banco"));

            // Act
            var result = await _service.GetAllAsync(1, 10);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.DATABASE_ERROR, result.ErrorCode);
            Assert.Equal("Erro ao buscar participantes.", result.ErrorMessage);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarNotFound_QuandoParticipanteNaoExiste()
        {
            // Arrange
            _participanteRepoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync((Participante)null);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Participante não encontrado.", result.ErrorMessage);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarSucesso_QuandoParticipanteEncontrado()
        {
            // Arrange
            var participante = new Participante { Id = 1, NomeCompleto = "Robson Junior", CPF = "12345678900" };
            _participanteRepoMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(participante);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("Robson Junior", result.Data.NomeCompleto);
            Assert.Equal(1, result.Data.Id);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarDatabaseError_QuandoRepositorioLancarExcecao()
        {
            // Arrange
            _participanteRepoMock
                .Setup(r => r.GetByIdAsync(1))
                .ThrowsAsync(new Exception("Falha no banco"));

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.DATABASE_ERROR, result.ErrorCode);
            Assert.Equal("Erro ao buscar participante.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateAsync_DeveRetornarErro_QuandoNaoEncontrar()
        {
            //Arrange
            var participante = new Participante { Id = 1, NomeCompleto = "Robson", CPF = "12345678901", Tipo = TipoParticipante.Interno };

            _participanteRepoMock.Setup(r => r.GetByIdAsync(participante.Id))
                                 .ReturnsAsync((Participante)null);

            //Act
            var result = await _service.UpdateAsync(participante);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
        }

        [Fact]
        public async Task UpdateAsync_DeveFalhar_QuandoNomeForVazio()
        {
            var participante = new Participante { Id = 1, NomeCompleto = "", CPF = "12345678901", Tipo = TipoParticipante.Interno };

            var result = await _service.UpdateAsync(participante);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("O nome completo é obrigatório.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateAsync_DeveFalhar_QuandoCpfForVazio()
        {
            var participante = new Participante { Id = 1, NomeCompleto = "João", CPF = "", Tipo = TipoParticipante.Interno };

            var result = await _service.UpdateAsync(participante);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("O CPF é obrigatório.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateAsync_DeveFalhar_QuandoTipoForInvalido()
        {
            var participante = new Participante { Id = 1, NomeCompleto = "João", CPF = "12345678901", Tipo = (TipoParticipante)999 };

            var result = await _service.UpdateAsync(participante);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("Tipo de participante inválido.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateAsync_DeveFalhar_QuandoParticipanteNaoExistir()
        {
            var participante = new Participante { Id = 1, NomeCompleto = "João", CPF = "12345678901", Tipo = TipoParticipante.Interno };

            _participanteRepoMock.Setup(r => r.GetByIdAsync(participante.Id)).ReturnsAsync((Participante)null);

            var result = await _service.UpdateAsync(participante);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Participante não encontrado.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateAsync_DeveFalhar_QuandoCpfJaExistirEmOutroParticipante()
        {
            var participante = new Participante { Id = 1, NomeCompleto = "João", CPF = "11111111111", Tipo = TipoParticipante.Interno };
            var existing = new Participante { Id = 1, CPF = "22222222222" };
            var conflict = new Participante { Id = 2, CPF = "11111111111" };

            _participanteRepoMock.Setup(r => r.GetByIdAsync(participante.Id)).ReturnsAsync(existing);
            _participanteRepoMock.Setup(r => r.GetByCpfAsync(participante.CPF)).ReturnsAsync(conflict);

            var result = await _service.UpdateAsync(participante);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.RESOURCE_ALREADY_EXISTS, result.ErrorCode);
            Assert.Equal("Já existe outro participante com esse CPF.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateAsync_DeveTerSucesso_QuandoDadosForemValidos()
        {
            var participante = new Participante { Id = 1, NomeCompleto = "João", CPF = "12345678901", Tipo = TipoParticipante.Interno };

            _participanteRepoMock.Setup(r => r.GetByIdAsync(participante.Id)).ReturnsAsync(participante);
            _participanteRepoMock.Setup(r => r.UpdateAsync(participante)).Returns(Task.CompletedTask);

            var result = await _service.UpdateAsync(participante);

            Assert.True(result.IsSuccess);
            Assert.Equal(participante, result.Data);
        }

        [Fact]
        public async Task UpdateAsync_DeveFalhar_QuandoRepositorioLancarExcecao()
        {
            var participante = new Participante { Id = 1, NomeCompleto = "João", CPF = "12345678901", Tipo = TipoParticipante.Interno };

            _participanteRepoMock.Setup(r => r.GetByIdAsync(participante.Id)).ThrowsAsync(new Exception("Erro de banco"));

            var result = await _service.UpdateAsync(participante);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.DATABASE_ERROR, result.ErrorCode);
            Assert.Equal("Erro ao atualizar participante.", result.ErrorMessage);
        }
    }
}
