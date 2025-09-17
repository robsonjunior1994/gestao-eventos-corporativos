using GestaoEventosCorporativos.Api._01_Presentation.Helpers;
using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Repositories;
using GestaoEventosCorporativos.Api._02_Core.Services;
using GestaoEventosCorporativos.Api._02_Core.Shared;
using Moq;
using Xunit;

namespace GestaoEventosCorporativos.Tests._02_Core.Services
{
    public class FornecedorServiceTests
    {
        private readonly Mock<IFornecedorRepository> _repoMock;
        private readonly FornecedorService _service;

        public FornecedorServiceTests()
        {
            _repoMock = new Mock<IFornecedorRepository>();
            _service = new FornecedorService(_repoMock.Object);
        }

        [Fact]
        public async Task AddAsync_DeveFalhar_QuandoNomeServicoInvalido()
        {
            // Arrange
            var fornecedor = new Fornecedor { NomeServico = "", CNPJ = "12345678000199", ValorBase = 100 };

            // Act
            var result = await _service.AddAsync(fornecedor);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("O nome do serviço é obrigatório.", result.ErrorMessage);
        }

        [Fact]
        public async Task AddAsync_DeveFalhar_QuandoCnpjInvalido()
        {
            // Arrange
            var fornecedor = new Fornecedor { NomeServico = "Som", CNPJ = "", ValorBase = 100 };

            // Act
            var result = await _service.AddAsync(fornecedor);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("O CNPJ é obrigatório.", result.ErrorMessage);
        }

        [Fact]
        public async Task AddAsync_DeveFalhar_QuandoValorBaseInvalido()
        {
            // Arrange
            var fornecedor = new Fornecedor { NomeServico = "Som", CNPJ = "12345678000199", ValorBase = 0 };

            // Act
            var result = await _service.AddAsync(fornecedor);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("O valor base deve ser maior que zero.", result.ErrorMessage);
        }

        [Fact]
        public async Task AddAsync_DeveFalhar_QuandoCnpjDuplicado()
        {
            // Arrange
            var fornecedor = new Fornecedor { NomeServico = "Som", CNPJ = "12345678000199", ValorBase = 200 };

            _repoMock.Setup(r => r.GetByCnpjAsync(fornecedor.CNPJ))
                     .ReturnsAsync(new Fornecedor { Id = 1, CNPJ = fornecedor.CNPJ });

            // Act
            var result = await _service.AddAsync(fornecedor);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.RESOURCE_ALREADY_EXISTS, result.ErrorCode);
            Assert.Equal("Já existe um fornecedor com este CNPJ.", result.ErrorMessage);
        }

        [Fact]
        public async Task AddAsync_DeveCriarFornecedor_QuandoValido()
        {
            // Arrange
            var fornecedor = new Fornecedor { NomeServico = "Som", CNPJ = "12345678000199", ValorBase = 200 };

            _repoMock.Setup(r => r.GetByCnpjAsync(fornecedor.CNPJ)).ReturnsAsync((Fornecedor)null);

            // Act
            var result = await _service.AddAsync(fornecedor);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(fornecedor, result.Data);
            _repoMock.Verify(r => r.AddAsync(fornecedor), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeveFalhar_QuandoNaoEncontrado()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Fornecedor)null);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Fornecedor não encontrado.", result.ErrorMessage);
        }

        [Fact]
        public async Task DeleteAsync_DeveExcluir_QuandoEncontrado()
        {
            // Arrange
            var fornecedor = new Fornecedor { Id = 1, NomeServico = "Som", CNPJ = "12345678000199", ValorBase = 200 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fornecedor);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.True(result.IsSuccess);
            _repoMock.Verify(r => r.DeleteAsync(fornecedor), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarListaPaginada_QuandoExistiremFornecedores()
        {
            // Arrange
            var fornecedores = new List<Fornecedor>
            {
                new Fornecedor { Id = 1, NomeServico = "Som", CNPJ = "12345678000199", ValorBase = 200 },
                new Fornecedor { Id = 2, NomeServico = "Buffet", CNPJ = "98765432000188", ValorBase = 500 }
            };

            _repoMock.Setup(r => r.GetAllAsync(1, 10))
                     .ReturnsAsync((fornecedores, fornecedores.Count));

            // Act
            var result = await _service.GetAllAsync(1, 10);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.TotalCount);
            Assert.Equal(1, result.Data.PageNumber);
            Assert.Equal(10, result.Data.PageSize);
            Assert.Equal(fornecedores, result.Data.Items);
        }
        [Fact]
        public async Task GetAllAsync_DeveRetornarListaVazia_QuandoNaoExistiremFornecedores()
        {
            // Arrange
            var fornecedores = new List<Fornecedor>();

            _repoMock.Setup(r => r.GetAllAsync(1, 10))
                     .ReturnsAsync((fornecedores, 0));

            // Act
            var result = await _service.GetAllAsync(1, 10);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data.Items);
            Assert.Equal(0, result.Data.TotalCount);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarFornecedor_QuandoEncontrado()
        {
            // Arrange
            var fornecedor = new Fornecedor { Id = 1, NomeServico = "Som", CNPJ = "12345678000199", ValorBase = 200 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fornecedor);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(fornecedor, result.Data);
        }

        [Fact]
        public async Task GetByIdAsync_DeveFalhar_QuandoNaoEncontrado()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Fornecedor)null);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Fornecedor não encontrado.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateAsync_DeveFalhar_QuandoNomeServicoInvalido()
        {
            // Arrange
            var fornecedor = new Fornecedor { Id = 1, NomeServico = "", CNPJ = "12345678000199", ValorBase = 200 };

            _repoMock.Setup(r => r.GetByIdAsync(fornecedor.Id)).ReturnsAsync(fornecedor);
            _repoMock.Setup(r => r.GetByCnpjAsync(fornecedor.CNPJ)).ReturnsAsync(fornecedor);

            // Act
            var result = await _service.UpdateAsync(fornecedor);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("O nome do serviço é obrigatório.", result.ErrorMessage);
        }

        public async Task UpdateAsync_DeveFalhar_QuandoCnpjInvalido()
        {
            // Arrange
            var fornecedor = new Fornecedor { Id = 1, NomeServico = "Som", CNPJ = "", ValorBase = 200 };

            _repoMock.Setup(r => r.GetByIdAsync(fornecedor.Id)).ReturnsAsync(fornecedor);
            _repoMock.Setup(r => r.GetByCnpjAsync(fornecedor.CNPJ)).ReturnsAsync(fornecedor);

            // Act
            var result = await _service.UpdateAsync(fornecedor);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("O CNPJ é obrigatório.", result.ErrorMessage);
        }

        public async Task UpdateAsync_DeveFalhar_QuandoValorBaseInvalido()
        {
            // Arrange
            var fornecedor = new Fornecedor { Id = 1, NomeServico = "Som", CNPJ = "12345678000199", ValorBase = 0 };

            _repoMock.Setup(r => r.GetByIdAsync(fornecedor.Id)).ReturnsAsync(fornecedor);
            _repoMock.Setup(r => r.GetByCnpjAsync(fornecedor.CNPJ)).ReturnsAsync(fornecedor);

            // Act
            var result = await _service.UpdateAsync(fornecedor);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("O valor base deve ser maior que zero.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateAsync_DeveFalhar_QuandoFornecedorNaoExiste()
        {
            // Arrange
            var fornecedor = new Fornecedor { Id = 1, NomeServico = "Som", CNPJ = "12345678000199", ValorBase = 200 };
            _repoMock.Setup(r => r.GetByIdAsync(fornecedor.Id)).ReturnsAsync((Fornecedor)null);

            // Act
            var result = await _service.UpdateAsync(fornecedor);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Fornecedor não encontrado.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateAsync_DeveFalhar_QuandoCnpjDuplicado()
        {
            // Arrange
            var fornecedor = new Fornecedor { Id = 1, NomeServico = "Som", CNPJ = "12345678000199", ValorBase = 200 };

            _repoMock.Setup(r => r.GetByIdAsync(fornecedor.Id)).ReturnsAsync(fornecedor);
            _repoMock.Setup(r => r.GetByCnpjAsync(fornecedor.CNPJ))
                     .ReturnsAsync(new Fornecedor { Id = 2, CNPJ = fornecedor.CNPJ });

            // Act
            var result = await _service.UpdateAsync(fornecedor);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.RESOURCE_ALREADY_EXISTS, result.ErrorCode);
            Assert.Equal("Já existe outro fornecedor com este CNPJ.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateAsync_DeveAtualizar_QuandoValido()
        {
            // Arrange
            var fornecedor = new Fornecedor { Id = 1, NomeServico = "Som", CNPJ = "12345678000199", ValorBase = 200 };

            _repoMock.Setup(r => r.GetByIdAsync(fornecedor.Id)).ReturnsAsync(fornecedor);
            _repoMock.Setup(r => r.GetByCnpjAsync(fornecedor.CNPJ)).ReturnsAsync(fornecedor);

            // Act
            var result = await _service.UpdateAsync(fornecedor);

            // Assert
            Assert.True(result.IsSuccess);
            _repoMock.Verify(r => r.UpdateAsync(fornecedor), Times.Once);
        }
    }
}
