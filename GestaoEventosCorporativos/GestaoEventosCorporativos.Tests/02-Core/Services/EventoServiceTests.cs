using GestaoEventosCorporativos.Api._01_Presentation.Helpers;
using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Repositories;
using GestaoEventosCorporativos.Api._02_Core.Services;
using Moq;

namespace GestaoEventosCorporativos.Tests._02_Core.Services
{
    public class EventoServiceTests
    {
        private readonly Mock<IEventoRepository> _eventoRepoMock;
        private readonly Mock<ITipoEventoRepository> _tipoEventoRepoMock;
        private readonly Mock<IParticipanteRepository> _participanteRepoMock;
        private readonly Mock<IFornecedorRepository> _fornecedorRepoMock;
        private readonly EventoService _service;

        public EventoServiceTests()
        {
            _eventoRepoMock = new Mock<IEventoRepository>();
            _tipoEventoRepoMock = new Mock<ITipoEventoRepository>();
            _participanteRepoMock = new Mock<IParticipanteRepository>();
            _fornecedorRepoMock = new Mock<IFornecedorRepository>();

            _service = new EventoService(
                _eventoRepoMock.Object,
                _tipoEventoRepoMock.Object,
                _participanteRepoMock.Object,
                _fornecedorRepoMock.Object
            );
        }

        [Fact]
        public async Task AddAsync_DeveFalhar_QuandoDataInicioMaiorOuIgualDataFim()
        {
            // Arrange
            var evento = new Evento("Teste", DateTime.Now.AddDays(1), DateTime.Now, "RJ", "Endereço", "Obs", 100, 1000, 1);

            // Act
            var result = await _service.AddAsync(evento);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Contains("A data de início deve ser anterior à data de fim.", result.ErrorMessage);
        }

        [Fact]
        public async Task AddAsync_DeveFalhar_QuandoLotacaoMenorQueMinimo()
        {
            // Arrange
            var evento = new Evento("Teste", DateTime.Now, DateTime.Now.AddDays(1), "RJ", "Endereço", "Obs", 0, 1000, 1);

            // Act
            var result = await _service.AddAsync(evento);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Contains("A lotação máxima deve ser maior que zero.", result.ErrorMessage);
        }

        [Fact]
        public async Task AddAsync_DeveFalhar_QuandoOrcamentoMenorQueMinimo()
        {
            // Arrange
            var evento = new Evento("Teste", DateTime.Now, DateTime.Now.AddDays(1), "RJ", "Endereço", "Obs", 10, 0, 1);

            // Act
            var result = await _service.AddAsync(evento);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Contains("O orçamento deve ser maior que zero.", result.ErrorMessage);
        }

        [Fact]
        public async Task AddAsync_DeveFalhar_QuandoTipoEventoNaoExiste()
        {
            // Arrange
            var evento = new Evento("Teste", DateTime.Now, DateTime.Now.AddDays(1), "RJ", "Endereço", "Obs", 10, 1000, 99);

            _tipoEventoRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((TipoEvento)null);

            // Act
            var result = await _service.AddAsync(evento);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Contains("O tipo de evento informado não existe.", result.ErrorMessage);
        }

        [Fact]
        public async Task AddAsync_DeveCriarEvento_QuandoDadosValidos()
        {
            // Arrange
            var evento = new Evento("Teste", DateTime.Now, DateTime.Now.AddDays(1), "RJ", "Endereço", "Obs", 10, 1000, 1);

            _tipoEventoRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new TipoEvento { Id = 1, Descricao = "Congresso" });

            _eventoRepoMock.Setup(r => r.AddAsync(evento))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.AddAsync(evento);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(evento, result.Data);
        }
        [Fact]
        public async Task AddAsync_DeveRetornarDatabaseError_QuandoRepositorioLancarExcecao()
        {
            // Arrange
            var evento = new Evento(
                nome: "Evento Teste",
                dataInicio: DateTime.Now,
                dataFim: DateTime.Now.AddDays(1),
                local: "Local Teste",
                endereco: "Endereço Teste",
                observacoes: "Observações",
                lotacaoMaxima: 100,
                orcamentoMaximo: 1000,
                tipoEventoId: 1
            );

            _tipoEventoRepoMock
                .Setup(r => r.GetByIdAsync(evento.TipoEventoId))
                .ReturnsAsync(new TipoEvento { Id = 1, Descricao = "Workshop" });

            _eventoRepoMock
                .Setup(r => r.AddAsync(evento))
                .ThrowsAsync(new Exception("Erro inesperado no banco"));

            // Act
            var result = await _service.AddAsync(evento);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.DATABASE_ERROR, result.ErrorCode);
            Assert.Equal("Ocorreu um erro ao criar o evento.", result.ErrorMessage);
        }

        [Fact]
        public async Task DeleteAsync_DeveFalhar_QuandoEventoNaoEncontrado()
        {
            // Arrange
            _eventoRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Evento)null);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Evento não encontrado.", result.ErrorMessage);
        }

        [Fact]
        public async Task DeleteAsync_DeveExcluir_QuandoEventoExiste()
        {
            // Arrange
            var evento = new Evento("Teste", DateTime.Now, DateTime.Now.AddDays(1), "RJ", "Endereço", "Obs", 10, 1000, 1);

            _eventoRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(evento);

            _eventoRepoMock.Setup(r => r.DeleteAsync(evento))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task DeleteAsync_DeveRetornarDatabaseError_QuandoRepositorioLancarExcecao()
        {
            // Arrange
            int eventoId = 1;

            _eventoRepoMock
                .Setup(r => r.GetByIdAsync(eventoId))
                .ThrowsAsync(new Exception("Erro inesperado no banco"));

            // Act
            var result = await _service.DeleteAsync(eventoId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.DATABASE_ERROR, result.ErrorCode);
            Assert.Equal("Ocorreu um erro ao excluir o evento.", result.ErrorMessage);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarSucesso_QuandoRepositorioRetornaEventos()
        {
            // Arrange
            var eventos = new List<Evento>
            {
                new Evento("Evento A", DateTime.Today, DateTime.Today.AddDays(1),
                           "Local A", "Endereco A", "Obs A", 100, 5000, 1),
                new Evento("Evento B", DateTime.Today.AddDays(2), DateTime.Today.AddDays(3),
                           "Local B", "Endereco B", "Obs B", 50, 2000, 2)
            };
            int totalCount = eventos.Count;

            _eventoRepoMock
                .Setup(r => r.GetAllWithAggregatesAsync(1, 10))
                .ReturnsAsync((eventos, totalCount));

            // Act
            var result = await _service.GetAllAsync(1, 10);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(totalCount, result.Data.TotalCount);
            Assert.Equal(2, result.Data.Items.Count());
            Assert.Equal("Evento A", result.Data.Items.First().Nome);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarSucesso_QuandoRepositorioNaoRetornaEventos()
        {
            // Arrange
            var eventos = new List<Evento>();
            int totalCount = 0;

            _eventoRepoMock
                .Setup(r => r.GetAllWithAggregatesAsync(1, 10))
                .ReturnsAsync((eventos, totalCount));

            // Act
            var result = await _service.GetAllAsync(1, 10);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data.Items);
            Assert.Equal(0, result.Data.TotalCount);
            Assert.Equal(1, result.Data.PageNumber);
            Assert.Equal(10, result.Data.PageSize);
        }

        [Fact]
        public async Task GetAllAsync_DeveRetornarErro_QuandoRepositorioLancarExcecao()
        {
            // Arrange
            _eventoRepoMock
                .Setup(r => r.GetAllWithAggregatesAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception("Erro no banco"));

            // Act
            var result = await _service.GetAllAsync(1, 10);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.DATABASE_ERROR, result.ErrorCode);
            Assert.Equal("Erro ao buscar eventos.", result.ErrorMessage);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarNotFound_QuandoEventoNaoExiste()
        {
            // Arrange
            _eventoRepoMock
                .Setup(r => r.GetByIdWithAggregatesAsync(1))
                .ReturnsAsync((Evento)null);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Evento não encontrado.", result.ErrorMessage);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarSucesso_QuandoEventoExiste()
        {
            // Arrange
            var evento = new Evento("Evento Teste", DateTime.Today, DateTime.Today.AddDays(1),
                "Local", "Endereco", "Obs", 100, 5000, 1);

            _eventoRepoMock
                .Setup(r => r.GetByIdWithAggregatesAsync(1))
                .ReturnsAsync(evento);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal("Evento Teste", result.Data.Nome);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarErroDatabase_QuandoRepositorioLancarExcecao()
        {
            // Arrange
            _eventoRepoMock
                .Setup(r => r.GetByIdWithAggregatesAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Erro no banco"));

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.DATABASE_ERROR, result.ErrorCode);
            Assert.Equal("Ocorreu um erro ao buscar o evento.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateAsync_DeveFalhar_QuandoDataInicioMaiorOuIgualDataFim()
        {
            // Arrange
            var evento = new Evento("Teste", DateTime.Today, DateTime.Today,
                "Local", "Endereco", "Obs", 10, 1000, 1);

            // Act
            var result = await _service.UpdateAsync(evento);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("A data de início deve ser anterior à data de fim.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateAsync_DeveFalhar_QuandoLotacaoMenorQueMinima()
        {
            // Arrange
            var evento = new Evento("Teste", DateTime.Today, DateTime.Today.AddDays(1),
                "Local", "Endereco", "Obs", 0, 1000, 1);

            // Act
            var result = await _service.UpdateAsync(evento);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("A lotação máxima deve ser maior que zero.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateAsync_DeveFalhar_QuandoOrcamentoMenorQueMinimo()
        {
            // Arrange
            var evento = new Evento("Teste", DateTime.Today, DateTime.Today.AddDays(1),
                "Local", "Endereco", "Obs", 10, 0, 1);

            // Act
            var result = await _service.UpdateAsync(evento);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("O orçamento deve ser maior que zero.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateAsync_DeveFalhar_QuandoTipoEventoNaoExistir()
        {
            // Arrange
            var evento = new Evento("Teste", DateTime.Today, DateTime.Today.AddDays(1),
                "Local", "Endereco", "Obs", 10, 1000, 1);

            _tipoEventoRepoMock
                .Setup(r => r.GetByIdAsync(evento.TipoEventoId))
                .ReturnsAsync((TipoEvento)null);

            // Act
            var result = await _service.UpdateAsync(evento);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("O tipo de evento informado não existe.", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateAsync_DeveRetornarSucesso_QuandoEventoValido()
        {
            // Arrange
            var evento = new Evento("Teste", DateTime.Today, DateTime.Today.AddDays(1),
                "Local", "Endereco", "Obs", 10, 1000, 1);

            _tipoEventoRepoMock
                .Setup(r => r.GetByIdAsync(evento.TipoEventoId))
                .ReturnsAsync(new TipoEvento { Id = 1, Descricao = "Tipo Teste" });

            // Act
            var result = await _service.UpdateAsync(evento);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(evento, result.Data);
            _eventoRepoMock.Verify(r => r.UpdateAsync(evento), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_DeveRetornarDatabaseError_QuandoRepositorioLancarExcecao()
        {
            // Arrange
            var evento = new Evento("Teste", DateTime.Today, DateTime.Today.AddDays(1),
                "Local", "Endereco", "Obs", 10, 1000, 1);

            _tipoEventoRepoMock
                .Setup(r => r.GetByIdAsync(evento.TipoEventoId))
                .ThrowsAsync(new Exception("Erro no banco"));

            // Act
            var result = await _service.UpdateAsync(evento);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.DATABASE_ERROR, result.ErrorCode);
            Assert.Equal("Ocorreu um erro ao atualizar o evento.", result.ErrorMessage);
        }

        [Fact]
        public async Task AddParticipanteByCpfAsync_DeveFalhar_QuandoEventoNaoExiste()
        {
            // Arrange
            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(It.IsAny<int>()))
                           .ReturnsAsync((Evento)null);

            // Act
            var result = await _service.AddParticipanteByCpfAsync(1, "12345678901");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Evento não encontrado.", result.ErrorMessage);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Never);
        }

        [Fact]
        public async Task AddParticipanteByCpfAsync_DeveFalhar_QuandoParticipanteNaoExiste()
        {
            // Arrange
            var evento = new Evento("DevConf",
                new DateTime(2025, 1, 10), new DateTime(2025, 1, 11),
                "Centro", "Rua X", "", 100, 1000m, 1)
            { Id = 1 };

            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(1))
                           .ReturnsAsync(evento);

            _participanteRepoMock.Setup(r => r.GetByCpfWithEventosAsync("12345678901"))
                                 .ReturnsAsync((Participante)null);

            // Act
            var result = await _service.AddParticipanteByCpfAsync(1, "12345678901");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Participante não encontrado.", result.ErrorMessage);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Never);
        }

        [Fact]
        public async Task AddParticipanteByCpfAsync_DeveFalhar_QuandoParticipanteJaVinculadoAoEvento()
        {
            // Arrange
            var evento = new Evento("DevConf",
                new DateTime(2025, 1, 10), new DateTime(2025, 1, 11),
                "Centro", "Rua X", "", 100, 1000m, 1)
            { Id = 1 };

            var participante = new Participante
            {
                Id = 7,
                NomeCompleto = "Ana Maria",
                CPF = "12345678901",
                Eventos = new List<ParticipanteEvento>() 
            };

            evento.Participantes.Add(new ParticipanteEvento
            {
                EventoId = evento.Id,
                ParticipanteId = participante.Id,
                Participante = participante,
                Evento = evento
            });

            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(1))
                           .ReturnsAsync(evento);

            _participanteRepoMock.Setup(r => r.GetByCpfWithEventosAsync("12345678901"))
                                 .ReturnsAsync(participante);

            // Act
            var result = await _service.AddParticipanteByCpfAsync(1, "12345678901");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.RESOURCE_ALREADY_EXISTS, result.ErrorCode);
            Assert.Equal("Participante já está vinculado a este evento.", result.ErrorMessage);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Never);
        }

        [Fact]
        public async Task AddParticipanteByCpfAsync_DeveFalhar_QuandoLotacaoMaximaAtingida()
        {
            // Arrange
            var evento = new Evento("DevConf",
                new DateTime(2025, 1, 10), new DateTime(2025, 1, 11),
                "Centro", "Rua X", "", 1, 1000m, 1)
            { Id = 1 };

            // já tem 1 participante (lotação = 1)
            evento.Participantes.Add(new ParticipanteEvento { EventoId = 1, ParticipanteId = 99, Evento = evento });

            var participante = new Participante { Id = 7, NomeCompleto = "Ana", CPF = "12345678901" };

            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(1)).ReturnsAsync(evento);
            _participanteRepoMock.Setup(r => r.GetByCpfWithEventosAsync("12345678901")).ReturnsAsync(participante);

            // Act
            var result = await _service.AddParticipanteByCpfAsync(1, "12345678901");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Contains("Não é possível adicionar mais participantes. A lotação máxima do evento já foi atingida.", result.ErrorMessage);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Never);
        }

        [Fact]
        public async Task AddParticipanteByCpfAsync_DeveFalhar_QuandoExisteConflitoDeDatasComOutroEventoDoParticipante()
        {
            // Arrange
            // evento alvo: 10-11 jan 2025
            var alvo = new Evento("DevConf",
                new DateTime(2025, 1, 10), new DateTime(2025, 1, 11),
                "Centro", "Rua X", "", 100, 1000m, 1)
            { Id = 1 };

            // evento já inscrito: 11 jan 2025 (sobrepõe porque DataInicio < DataFim && DataFim > DataInicio)
            var outro = new Evento("Meetup",
                new DateTime(2025, 1, 10), new DateTime(2025, 1, 10, 23, 59, 59),
                "Auditório", "Rua Y", "", 50, 500m, 1)
            { Id = 2, Nome = "Meetup" };

            var participante = new Participante
            {
                Id = 7,
                NomeCompleto = "Ana",
                CPF = "12345678901",
                Eventos = new List<ParticipanteEvento>
            {
                new ParticipanteEvento { EventoId = 2, Evento = outro }
            }
            };

            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(1)).ReturnsAsync(alvo);
            _participanteRepoMock.Setup(r => r.GetByCpfWithEventosAsync("12345678901")).ReturnsAsync(participante);

            // Act
            var result = await _service.AddParticipanteByCpfAsync(1, "12345678901");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Contains("inscrito no evento 'Meetup'", result.ErrorMessage);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Never);
        }

        [Fact]
        public async Task AddParticipanteByCpfAsync_DeveAdicionar_ComSucesso()
        {
            // Arrange
            var alvo = new Evento("DevConf",
                new DateTime(2025, 1, 10), new DateTime(2025, 1, 11),
                "Centro", "Rua X", "", 2, 1000m, 1)
            { Id = 1 };

            var participante = new Participante
            {
                Id = 7,
                NomeCompleto = "Ana",
                CPF = "12345678901",
                Eventos = new List<ParticipanteEvento>() // sem conflitos
            };

            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(1)).ReturnsAsync(alvo);
            _participanteRepoMock.Setup(r => r.GetByCpfWithEventosAsync("12345678901")).ReturnsAsync(participante);
            _eventoRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Evento>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.AddParticipanteByCpfAsync(1, "12345678901");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(participante, result.Data);
            Assert.Single(alvo.Participantes);
            Assert.Equal(7, alvo.Participantes.First().ParticipanteId);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Once);
        }
        
        [Fact]
        public async Task AddParticipanteByCpfAsync_DeveRetornarDatabaseError_QuandoRepositorioLancarExcecao()
        {
            // Arrange
            int eventoId = 1;
            string cpf = "12345678901";

            _eventoRepoMock
                .Setup(r => r.GetByIdWithAggregatesAsync(eventoId))
                .ThrowsAsync(new Exception("Erro inesperado no banco"));

            // Act
            var result = await _service.AddParticipanteByCpfAsync(eventoId, cpf);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.DATABASE_ERROR, result.ErrorCode);
            Assert.Equal("Erro ao adicionar participante ao evento.", result.ErrorMessage);
        }

        [Fact]
        public async Task AddFornecedorByCnpjAsync_DeveFalhar_QuandoEventoNaoExiste()
        {
            // Arrange
            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(It.IsAny<int>()))
                           .ReturnsAsync((Evento)null);

            // Act
            var result = await _service.AddFornecedorByCnpjAsync(1, "11111111000191");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Evento não encontrado.", result.ErrorMessage);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Never);
        }

        [Fact]
        public async Task AddFornecedorByCnpjAsync_DeveFalhar_QuandoCnpjVazio()
        {
            // Arrange
            var evento = new Evento("DevConf",
                new DateTime(2025, 1, 10), new DateTime(2025, 1, 11),
                "Centro", "Rua X", "", 100, 1000m, 1)
            { Id = 1 };

            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(1)).ReturnsAsync(evento);

            // Act
            var result = await _service.AddFornecedorByCnpjAsync(1, "  ");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("O CNPJ é obrigatório.", result.ErrorMessage);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Never);
        }

        [Fact]
        public async Task AddFornecedorByCnpjAsync_DeveFalhar_QuandoFornecedorNaoExiste()
        {
            // Arrange
            var evento = new Evento("DevConf",
                new DateTime(2025, 1, 10), new DateTime(2025, 1, 11),
                "Centro", "Rua X", "", 100, 1000m, 1)
            { Id = 1 };

            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(1)).ReturnsAsync(evento);
            _fornecedorRepoMock.Setup(r => r.GetByCnpjAsync("11111111000191"))
                               .ReturnsAsync((Fornecedor)null);

            // Act
            var result = await _service.AddFornecedorByCnpjAsync(1, "11111111000191");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Fornecedor não encontrado.", result.ErrorMessage);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Never);
        }

        [Fact]
        public async Task AddFornecedorByCnpjAsync_DeveFalhar_QuandoFornecedorJaVinculado()
        {
            // Arrange
            var evento = new Evento("DevConf",
                new DateTime(2025, 1, 10), new DateTime(2025, 1, 11),
                "Centro", "Rua X", "", 100, 1000m, 1)
            { Id = 1 };

            var fornecedor = new Fornecedor { Id = 10, CNPJ = "11111111000191", NomeServico = "Buffet", ValorBase = 100m };

            evento.Fornecedores.Add(new EventoFornecedor
            {
                EventoId = evento.Id,
                FornecedorId = fornecedor.Id,
                Fornecedor = fornecedor,
                ValorContratado = fornecedor.ValorBase
            });

            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(1)).ReturnsAsync(evento);
            _fornecedorRepoMock.Setup(r => r.GetByCnpjAsync(fornecedor.CNPJ)).ReturnsAsync(fornecedor);

            // Act
            var result = await _service.AddFornecedorByCnpjAsync(1, fornecedor.CNPJ);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.RESOURCE_ALREADY_EXISTS, result.ErrorCode);
            Assert.Equal("Fornecedor já está vinculado a este evento.", result.ErrorMessage);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Never);
        }

        [Fact]
        public async Task AddFornecedorByCnpjAsync_DeveFalhar_QuandoValorBaseExcedeSaldo()
        {
            // Arrange
            var evento = new Evento("DevConf",
                new DateTime(2025, 1, 10), new DateTime(2025, 1, 11),
                "Centro", "Rua X", "", 100, 300m, 1)
            { Id = 1 }; // orçamento baixo

            var fornecedor = new Fornecedor { Id = 10, CNPJ = "11111111000191", NomeServico = "Som & Luz", ValorBase = 500m };

            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(1)).ReturnsAsync(evento);
            _fornecedorRepoMock.Setup(r => r.GetByCnpjAsync(fornecedor.CNPJ)).ReturnsAsync(fornecedor);

            // Act
            var result = await _service.AddFornecedorByCnpjAsync(1, fornecedor.CNPJ);
            
            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("O valor base do fornecedor excede o saldo do orçamento do evento.", result.ErrorMessage);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Never);
        }

        [Fact]
        public async Task AddFornecedorByCnpjAsync_DeveAdicionar_ComSucesso()
        {
            // Arrange
            var evento = new Evento("DevConf",
                new DateTime(2025, 1, 10), new DateTime(2025, 1, 11),
                "Centro", "Rua X", "", 100, 1000m, 1)
            { Id = 1 };

            var fornecedor = new Fornecedor { Id = 10, CNPJ = "11111111000191", NomeServico = "Buffet", ValorBase = 400m };

            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(1)).ReturnsAsync(evento);
            _fornecedorRepoMock.Setup(r => r.GetByCnpjAsync(fornecedor.CNPJ)).ReturnsAsync(fornecedor);
            _eventoRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Evento>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.AddFornecedorByCnpjAsync(1, fornecedor.CNPJ);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(fornecedor, result.Data);
            Assert.Single(evento.Fornecedores);
            Assert.Equal(10, evento.Fornecedores.First().FornecedorId);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Once);
        }

        [Fact]
        public async Task AddFornecedorByCnpjAsync_DeveRetornarDatabaseError_QuandoRepositorioLancarExcecao()
        {
            // Arrange
            int eventoId = 1;
            string cnpj = "12345678000199";

            _eventoRepoMock
                .Setup(r => r.GetByIdWithAggregatesAsync(eventoId))
                .ThrowsAsync(new Exception("Erro inesperado no banco"));

            // Act
            var result = await _service.AddFornecedorByCnpjAsync(eventoId, cnpj);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.DATABASE_ERROR, result.ErrorCode);
            Assert.Equal("Erro ao adicionar fornecedor ao evento.", result.ErrorMessage);
        }

        [Fact]
        public async Task RemoveParticipanteByCpfAsync_DeveRetornarErro_QuandoCpfForVazio()
        {
            // Act
            var result = await _service.RemoveParticipanteByCpfAsync(1, "   ");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("O CPF é obrigatório.", result.ErrorMessage);
        }

        [Fact]
        public async Task RemoveParticipanteByCpfAsync_DeveRetornarErro_QuandoEventoNaoEncontrado()
        {
            // Arrange
            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(It.IsAny<int>()))
                .ReturnsAsync((Evento)null);

            // Act
            var result = await _service.RemoveParticipanteByCpfAsync(1, "12345678901");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Evento não encontrado.", result.ErrorMessage);
        }

        [Fact]
        public async Task RemoveParticipanteByCpfAsync_DeveRetornarErro_QuandoParticipanteNaoEncontrado()
        {
            // Arrange
            var evento = new Evento("Evento Teste", DateTime.Today, DateTime.Today.AddDays(1), "Local", "Endereço", "Obs", 100, 1000, 1);
            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(It.IsAny<int>()))
                .ReturnsAsync(evento);

            _participanteRepoMock.Setup(r => r.GetByCpfWithEventosAsync(It.IsAny<string>()))
                .ReturnsAsync((Participante)null);

            // Act
            var result = await _service.RemoveParticipanteByCpfAsync(1, "12345678901");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Participante não encontrado.", result.ErrorMessage);
        }

        [Fact]
        public async Task RemoveParticipanteByCpfAsync_DeveRetornarErro_QuandoParticipanteNaoVinculado()
        {
            // Arrange
            var participante = new Participante { Id = 1, CPF = "12345678901", NomeCompleto = "João Teste" };
            var evento = new Evento("Evento Teste", DateTime.Today, DateTime.Today.AddDays(1), "Local", "Endereço", "Obs", 100, 1000, 1);

            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(It.IsAny<int>()))
                .ReturnsAsync(evento);
            _participanteRepoMock.Setup(r => r.GetByCpfWithEventosAsync("12345678901"))
                .ReturnsAsync(participante);

            // Act
            var result = await _service.RemoveParticipanteByCpfAsync(1, "12345678901");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Participante não está vinculado a este evento.", result.ErrorMessage);
        }

        [Fact]
        public async Task RemoveParticipanteByCpfAsync_DeveRemoverComSucesso()
        {
            // Arrange
            var participante = new Participante { Id = 1, CPF = "12345678901", NomeCompleto = "João Teste" };
            var evento = new Evento("Evento Teste", DateTime.Today, DateTime.Today.AddDays(1), "Local", "Endereço", "Obs", 100, 1000, 1);

            var participanteEvento = new ParticipanteEvento { EventoId = evento.Id, ParticipanteId = participante.Id, Participante = participante };
            evento.Participantes.Add(participanteEvento);

            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(It.IsAny<int>()))
                .ReturnsAsync(evento);
            _participanteRepoMock.Setup(r => r.GetByCpfWithEventosAsync("12345678901"))
                .ReturnsAsync(participante);

            // Act
            var result = await _service.RemoveParticipanteByCpfAsync(1, "12345678901");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            _eventoRepoMock.Verify(r => r.UpdateAsync(evento), Times.Once);
        }

        [Fact]
        public async Task RemoveParticipanteByCpfAsync_DeveRetornarErro_QuandoOcorreExcecao()
        {
            // Arrange
            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Erro inesperado"));

            // Act
            var result = await _service.RemoveParticipanteByCpfAsync(1, "12345678901");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.DATABASE_ERROR, result.ErrorCode);
            Assert.Equal("Erro ao remover participante do evento.", result.ErrorMessage);
        }

        [Fact]
        public async Task RemoveFornecedorByCnpjAsync_DeveRetornarErro_QuandoEventoNaoEncontrado()
        {
            // Arrange
            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(1))
                .ReturnsAsync((Evento)null);

            // Act
            var result = await _service.RemoveFornecedorByCnpjAsync(1, "12345678000100");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Evento não encontrado.", result.ErrorMessage);
        }

        [Fact]
        public async Task RemoveFornecedorByCnpjAsync_DeveRetornarErro_QuandoCnpjForVazio()
        {
            // Arrange
            var evento = new Evento(
                "Evento Teste",
                DateTime.Now,
                DateTime.Now.AddDays(1),
                "Local",
                "Endereco",
                "Obs",
                100,
                1000,
                1
            );

            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(1))
                .ReturnsAsync(evento);

            // Act
            var result = await _service.RemoveFornecedorByCnpjAsync(1, "   ");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("O CNPJ é obrigatório.", result.ErrorMessage);
        }

        [Fact]
        public async Task RemoveFornecedorByCnpjAsync_DeveRetornarErro_QuandoFornecedorNaoEncontrado()
        {
            // Arrange
            var evento = new Evento(
                "Evento Teste",
                DateTime.Now,
                DateTime.Now.AddDays(1),
                "Local",
                "Endereco",
                "Obs",
                100,
                1000,
                1
            );

            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(1))
                .ReturnsAsync(evento);
            _fornecedorRepoMock.Setup(r => r.GetByCnpjAsync("12345678000100"))
                .ReturnsAsync((Fornecedor)null);

            // Act
            var result = await _service.RemoveFornecedorByCnpjAsync(1, "12345678000100");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Fornecedor não encontrado.", result.ErrorMessage);
        }

        [Fact]
        public async Task RemoveFornecedorByCnpjAsync_DeveRetornarErro_QuandoFornecedorNaoEstaNoEvento()
        {
            // Arrange
            var evento = new Evento(
                "Evento Teste",
                DateTime.Now,
                DateTime.Now.AddDays(1),
                "Local",
                "Endereco",
                "Obs",
                100,
                1000,
                1
            );

            var fornecedor = new Fornecedor { Id = 10, CNPJ = "12345678000100" };

            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(1))
                .ReturnsAsync(evento);
            _fornecedorRepoMock.Setup(r => r.GetByCnpjAsync("12345678000100"))
                .ReturnsAsync(fornecedor);

            // Act
            var result = await _service.RemoveFornecedorByCnpjAsync(1, "12345678000100");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Fornecedor não está vinculado a este evento.", result.ErrorMessage);
        }

        [Fact]
        public async Task RemoveFornecedorByCnpjAsync_DeveRetornarSucesso_QuandoFornecedorRemovido()
        {
            // Arrange
            var fornecedor = new Fornecedor { Id = 10, CNPJ = "12345678000100", ValorBase = 500 };

            var evento = new Evento(
                "Evento Teste",
                DateTime.Now,
                DateTime.Now.AddDays(1),
                "Local",
                "Endereco",
                "Obs",
                100,
                1000,
                1
            );

            evento.Fornecedores.Add(new EventoFornecedor
            {
                EventoId = evento.Id,
                FornecedorId = fornecedor.Id,
                ValorContratado = fornecedor.ValorBase,
                Fornecedor = fornecedor
            });

            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(1))
                .ReturnsAsync(evento);
            _fornecedorRepoMock.Setup(r => r.GetByCnpjAsync("12345678000100"))
                .ReturnsAsync(fornecedor);

            // Act
            var result = await _service.RemoveFornecedorByCnpjAsync(1, "12345678000100");

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
            Assert.Empty(evento.Fornecedores); // foi removido
            _eventoRepoMock.Verify(r => r.UpdateAsync(evento), Times.Once);
        }
        [Fact]
        public async Task RemoveFornecedorByCnpjAsync_DeveRetornarErro_QuandoOcorreExcecao()
        {
            // Arrange
            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Falha inesperada"));

            // Act
            var result = await _service.RemoveFornecedorByCnpjAsync(1, "12345678000100");

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.DATABASE_ERROR, result.ErrorCode);
            Assert.Equal("Erro ao remover fornecedor do evento.", result.ErrorMessage);
        }

    }
}
