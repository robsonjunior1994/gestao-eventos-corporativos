using GestaoEventosCorporativos.Api._01_Presentation.Helpers;
using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Repositories;
using GestaoEventosCorporativos.Api._02_Core.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoEventosCorporativos.Tests._02_Core.Services
{
    public class TipoEventoServiceTests
    {
        private readonly Mock<ITipoEventoRepository> _repoMock;
        private readonly TipoEventoService _service;

        public TipoEventoServiceTests()
        {
            _repoMock = new Mock<ITipoEventoRepository>();
            _service = new TipoEventoService(_repoMock.Object);
        }

        [Fact]
        public async Task AddAsync_DeveRetornarErro_QuandoDescricaoVazia()
        {
            //Arrange
            var tipoEvento = new TipoEvento { Descricao = "" };

            //Act
            var result = await _service.AddAsync(tipoEvento);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("A descrição do tipo de evento é obrigatória.", result.ErrorMessage);
        }

        [Fact]
        public async Task AddAsync_DeveRetornarErro_QuandoDescricaoDuplicada()
        {
            //Arrange
            var tipoEvento = new TipoEvento { Descricao = "Workshop" };
            _repoMock.Setup(r => r.GetByDescricaoAsync(tipoEvento.Descricao))
                     .ReturnsAsync(new TipoEvento { Id = 1, Descricao = "Workshop" });

            //Act
            var result = await _service.AddAsync(tipoEvento);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.RESOURCE_ALREADY_EXISTS, result.ErrorCode);
            Assert.Equal("Já existe um tipo de evento com essa descrição.", result.ErrorMessage);
        }

        [Fact]
        public async Task AddAsync_DeveCriar_QuandoValido()
        {
            //Arrange
            var tipoEvento = new TipoEvento { Descricao = "Seminário" };
            _repoMock.Setup(r => r.GetByDescricaoAsync(tipoEvento.Descricao))
                     .ReturnsAsync((TipoEvento)null);

            //Act
            var result = await _service.AddAsync(tipoEvento);

            //Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(tipoEvento, result.Data);
            _repoMock.Verify(r => r.AddAsync(tipoEvento), Times.Once);
        }

        [Fact]
        public async Task AddAsync_DeveRetornarDatabaseError_QuandoRepositorioLancarExcecao()
        {
            // Arrange
            var tipoEvento = new TipoEvento { Id = 1, Descricao = "Congresso" };

            _repoMock
                .Setup(r => r.GetByDescricaoAsync(tipoEvento.Descricao))
                .ThrowsAsync(new Exception("Falha no banco"));

            // Act
            var result = await _service.AddAsync(tipoEvento);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.DATABASE_ERROR, result.ErrorCode);
            Assert.Equal("Erro ao criar tipo de evento.", result.ErrorMessage);
        }

        [Fact]
        public async Task DeleteAsync_DeveRetornarErro_QuandoNaoEncontrado()
        {
            //Arrange
            _repoMock.Setup(r => r.GetByIdAsync(1))
                     .ReturnsAsync((TipoEvento)null);

            //Act
            var result = await _service.DeleteAsync(1);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Tipo de evento não encontrado.", result.ErrorMessage);
        }

        [Fact]
        public async Task DeleteAsync_DeveRemover_QuandoEncontrado()
        {
            //Arrange
            var tipoEvento = new TipoEvento { Id = 1, Descricao = "Oficina" };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(tipoEvento);

            //Act
            var result = await _service.DeleteAsync(1);

            //Assert
            Assert.True(result.IsSuccess);
            _repoMock.Verify(r => r.DeleteAsync(tipoEvento), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeveRetornarDatabaseError_QuandoRepositorioLancarExcecao()
        {
            // Arrange
            var tipoEvento = new TipoEvento { Id = 1, Descricao = "Congresso" };

            _repoMock
                .Setup(r => r.GetByIdAsync(tipoEvento.Id))
                .ThrowsAsync(new Exception("Falha inesperada"));

            // Act
            var result = await _service.DeleteAsync(tipoEvento.Id);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.DATABASE_ERROR, result.ErrorCode);
            Assert.Equal("Erro ao excluir tipo de evento.", result.ErrorMessage);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarSucesso_QuandoRepositorioRetornaDados()
        {
            // Arrange
            var tipos = new List<TipoEvento>
            {
                new TipoEvento { Id = 1, Descricao = "Workshop" },
                new TipoEvento { Id = 2, Descricao = "Seminário" }
            };
            int totalCount = tipos.Count;

            _repoMock
                .Setup(r => r.GetAllAsync(1, 10))
                .ReturnsAsync((tipos, totalCount));

            // Act
            var result = await _service.GetAllAsync(1, 10);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(totalCount, result.Data.TotalCount);
            Assert.Equal(2, result.Data.Items.Count());
            Assert.Equal("Workshop", result.Data.Items.First().Descricao);
            _repoMock.Verify(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarErro_QuandoRepositorioLancarExcecao()
        {
            // Arrange
            _repoMock
                .Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Erro de banco"));

            // Act
            var result = await _service.GetAllAsync(1, 10);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.DATABASE_ERROR, result.ErrorCode);
            Assert.Equal("Erro ao buscar tipos de evento.", result.ErrorMessage);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarErro_QuandoNaoEncontrado()
        {
            //Arrange
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((TipoEvento)null);

            // Act
            var result = await _service.GetByIdAsync(1);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Tipo de evento não encontrado.", result.ErrorMessage);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarTipo_QuandoEncontrado()
        {
            //Arrange
            var tipoEvento = new TipoEvento { Id = 1, Descricao = "Congresso" };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(tipoEvento);

            //Act
            var result = await _service.GetByIdAsync(1);

            //Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(tipoEvento, result.Data);
            _repoMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Once());
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarDatabaseError_QuandoRepositorioLancarExcecao()
        {
            // Arrange
            _repoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Falha inesperada no banco"));

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.DATABASE_ERROR, result.ErrorCode);
            Assert.Equal("Erro ao buscar tipo de evento.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateAsync_DeveRetornarErro_QuandoDescricaoVazia()
        {
            //Arrange
            var tipoEvento = new TipoEvento { Id = 1, Descricao = "" };

            //Act
            var result = await _service.UpdateAsync(tipoEvento);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("A descrição do tipo de evento é obrigatória.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateAsync_DeveRetornarErro_QuandoNaoEncontrado()
        {
            //Arrange
            var tipoEvento = new TipoEvento { Id = 1, Descricao = "Novo Nome" };
            _repoMock.Setup(r => r.GetByIdAsync(tipoEvento.Id))
                     .ReturnsAsync((TipoEvento)null);

            //Act
            var result = await _service.UpdateAsync(tipoEvento);

            //Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Tipo de evento não encontrado.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateAsync_DeveAtualizar_QuandoValido()
        {
            //Arrange
            var tipoEvento = new TipoEvento { Id = 1, Descricao = "Palestra" };
            _repoMock.Setup(r => r.GetByIdAsync(tipoEvento.Id))
                     .ReturnsAsync(new TipoEvento { Id = 1, Descricao = "Antigo" });

            //Act
            var result = await _service.UpdateAsync(tipoEvento);

            //Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(tipoEvento.Descricao, result.Data.Descricao);
            _repoMock.Verify(r => r.UpdateAsync(It.IsAny<TipoEvento>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_DeveRetornarDatabaseError_QuandoRepositorioLancarExcecao()
        {
            // Arrange
            var tipoEvento = new TipoEvento { Id = 1, Descricao = "Workshop" };

            _repoMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Falha inesperada no banco"));

            // Act
            var result = await _service.UpdateAsync(tipoEvento);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.DATABASE_ERROR, result.ErrorCode);
            Assert.Equal("Erro ao atualizar tipo de evento.", result.ErrorMessage);
        }
    }
}
