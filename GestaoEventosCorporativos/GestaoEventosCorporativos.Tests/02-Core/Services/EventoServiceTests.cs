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
            var evento = new Evento("Teste", DateTime.Now, DateTime.Now, "RJ", "Endereço", "Obs", 100, 1000, 1);

            // Act
            var result = await _service.AddAsync(evento);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
        }

        [Fact]
        public async Task AddAsync_DeveFalhar_QuandoLotacaoMenorQueMinimo()
        {
            var evento = new Evento("Teste", DateTime.Now, DateTime.Now.AddDays(1), "RJ", "Endereço", "Obs", 0, 1000, 1);

            var result = await _service.AddAsync(evento);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
        }

        [Fact]
        public async Task AddAsync_DeveFalhar_QuandoOrcamentoMenorQueMinimo()
        {
            var evento = new Evento("Teste", DateTime.Now, DateTime.Now.AddDays(1), "RJ", "Endereço", "Obs", 10, 0, 1);

            var result = await _service.AddAsync(evento);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
        }

        [Fact]
        public async Task AddAsync_DeveFalhar_QuandoTipoEventoNaoExiste()
        {
            var evento = new Evento("Teste", DateTime.Now, DateTime.Now.AddDays(1), "RJ", "Endereço", "Obs", 10, 1000, 99);

            _tipoEventoRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((TipoEvento)null);

            var result = await _service.AddAsync(evento);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
        }

        [Fact]
        public async Task AddAsync_DeveCriarEvento_QuandoDadosValidos()
        {
            var evento = new Evento("Teste", DateTime.Now, DateTime.Now.AddDays(1), "RJ", "Endereço", "Obs", 10, 1000, 1);

            _tipoEventoRepoMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new TipoEvento { Id = 1, Descricao = "Congresso" });

            _eventoRepoMock.Setup(r => r.AddAsync(evento))
                .Returns(Task.CompletedTask);

            var result = await _service.AddAsync(evento);

            Assert.True(result.IsSuccess);
            Assert.Equal(evento, result.Data);
        }

        [Fact]
        public async Task DeleteAsync_DeveFalhar_QuandoEventoNaoEncontrado()
        {
            _eventoRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Evento)null);

            var result = await _service.DeleteAsync(1);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
        }

        [Fact]
        public async Task DeleteAsync_DeveExcluir_QuandoEventoExiste()
        {
            var evento = new Evento("Teste", DateTime.Now, DateTime.Now.AddDays(1), "RJ", "Endereço", "Obs", 10, 1000, 1);

            _eventoRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(evento);

            _eventoRepoMock.Setup(r => r.DeleteAsync(evento))
                .Returns(Task.CompletedTask);

            var result = await _service.DeleteAsync(1);

            Assert.True(result.IsSuccess);
            Assert.True(result.Data);
        }

        // ---------- AddParticipanteByCpfAsync ----------

        [Fact]
        public async Task AddParticipanteByCpfAsync_DeveFalhar_QuandoEventoNaoExiste()
        {
            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(It.IsAny<int>()))
                           .ReturnsAsync((Evento)null);

            var result = await _service.AddParticipanteByCpfAsync(1, "12345678901");

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Evento não encontrado.", result.ErrorMessage);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Never);
        }

        [Fact]
        public async Task AddParticipanteByCpfAsync_DeveFalhar_QuandoParticipanteNaoExiste()
        {
            var evento = new Evento("DevConf",
                new DateTime(2025, 1, 10), new DateTime(2025, 1, 11),
                "Centro", "Rua X", "", 100, 1000m, 1)
            { Id = 1 };

            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(1))
                           .ReturnsAsync(evento);

            _participanteRepoMock.Setup(r => r.GetByCpfWithEventosAsync("12345678901"))
                                 .ReturnsAsync((Participante)null);

            var result = await _service.AddParticipanteByCpfAsync(1, "12345678901");

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Participante não encontrado.", result.ErrorMessage);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Never);
        }

        [Fact]
        public async Task AddParticipanteByCpfAsync_DeveFalhar_QuandoParticipanteJaVinculadoAoEvento()
        {
            var evento = new Evento("DevConf",
                new DateTime(2025, 1, 10), new DateTime(2025, 1, 11),
                "Centro", "Rua X", "", 100, 1000m, 1)
            { Id = 1 };

            var participante = new Participante
            {
                Id = 7,
                NomeCompleto = "Ana Maria",
                CPF = "12345678901",
                Eventos = new List<ParticipanteEvento>() // pode estar vazio; o vínculo que importa é no evento atual
            };

            // já vinculado
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

            var result = await _service.AddParticipanteByCpfAsync(1, "12345678901");

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.RESOURCE_ALREADY_EXISTS, result.ErrorCode);
            Assert.Equal("Participante já está vinculado a este evento.", result.ErrorMessage);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Never);
        }

        [Fact]
        public async Task AddParticipanteByCpfAsync_DeveFalhar_QuandoLotacaoMaximaAtingida()
        {
            var evento = new Evento("DevConf",
                new DateTime(2025, 1, 10), new DateTime(2025, 1, 11),
                "Centro", "Rua X", "", 1, 1000m, 1)
            { Id = 1 };

            // já tem 1 participante (lotação = 1)
            evento.Participantes.Add(new ParticipanteEvento { EventoId = 1, ParticipanteId = 99, Evento = evento });

            var participante = new Participante { Id = 7, NomeCompleto = "Ana", CPF = "12345678901" };

            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(1)).ReturnsAsync(evento);
            _participanteRepoMock.Setup(r => r.GetByCpfWithEventosAsync("12345678901")).ReturnsAsync(participante);

            var result = await _service.AddParticipanteByCpfAsync(1, "12345678901");

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Contains("lotação máxima", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Never);
        }

        [Fact]
        public async Task AddParticipanteByCpfAsync_DeveFalhar_QuandoExisteConflitoDeDatasComOutroEventoDoParticipante()
        {
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

            var result = await _service.AddParticipanteByCpfAsync(1, "12345678901");

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Contains("inscrito no evento 'Meetup'", result.ErrorMessage);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Never);
        }

        [Fact]
        public async Task AddParticipanteByCpfAsync_DeveAdicionar_ComSucesso()
        {
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

            var result = await _service.AddParticipanteByCpfAsync(1, "12345678901");

            Assert.True(result.IsSuccess);
            Assert.Equal(participante, result.Data);
            Assert.Single(alvo.Participantes);
            Assert.Equal(7, alvo.Participantes.First().ParticipanteId);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Once);
        }

        [Fact]
        public async Task AddFornecedorByCnpjAsync_DeveFalhar_QuandoEventoNaoExiste()
        {
            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(It.IsAny<int>()))
                           .ReturnsAsync((Evento)null);

            var result = await _service.AddFornecedorByCnpjAsync(1, "11111111000191");

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Evento não encontrado.", result.ErrorMessage);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Never);
        }

        [Fact]
        public async Task AddFornecedorByCnpjAsync_DeveFalhar_QuandoCnpjVazio()
        {
            var evento = new Evento("DevConf",
                new DateTime(2025, 1, 10), new DateTime(2025, 1, 11),
                "Centro", "Rua X", "", 100, 1000m, 1)
            { Id = 1 };

            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(1)).ReturnsAsync(evento);

            var result = await _service.AddFornecedorByCnpjAsync(1, "  ");

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("O CNPJ é obrigatório.", result.ErrorMessage);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Never);
        }

        [Fact]
        public async Task AddFornecedorByCnpjAsync_DeveFalhar_QuandoFornecedorNaoExiste()
        {
            var evento = new Evento("DevConf",
                new DateTime(2025, 1, 10), new DateTime(2025, 1, 11),
                "Centro", "Rua X", "", 100, 1000m, 1)
            { Id = 1 };

            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(1)).ReturnsAsync(evento);
            _fornecedorRepoMock.Setup(r => r.GetByCnpjAsync("11111111000191"))
                               .ReturnsAsync((Fornecedor)null);

            var result = await _service.AddFornecedorByCnpjAsync(1, "11111111000191");

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.NOT_FOUND, result.ErrorCode);
            Assert.Equal("Fornecedor não encontrado.", result.ErrorMessage);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Never);
        }

        [Fact]
        public async Task AddFornecedorByCnpjAsync_DeveFalhar_QuandoFornecedorJaVinculado()
        {
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

            var result = await _service.AddFornecedorByCnpjAsync(1, fornecedor.CNPJ);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.RESOURCE_ALREADY_EXISTS, result.ErrorCode);
            Assert.Equal("Fornecedor já está vinculado a este evento.", result.ErrorMessage);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Never);
        }

        [Fact]
        public async Task AddFornecedorByCnpjAsync_DeveFalhar_QuandoValorBaseExcedeSaldo()
        {
            var evento = new Evento("DevConf",
                new DateTime(2025, 1, 10), new DateTime(2025, 1, 11),
                "Centro", "Rua X", "", 100, 300m, 1)
            { Id = 1 }; // orçamento baixo

            var fornecedor = new Fornecedor { Id = 10, CNPJ = "11111111000191", NomeServico = "Som & Luz", ValorBase = 500m };

            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(1)).ReturnsAsync(evento);
            _fornecedorRepoMock.Setup(r => r.GetByCnpjAsync(fornecedor.CNPJ)).ReturnsAsync(fornecedor);

            var result = await _service.AddFornecedorByCnpjAsync(1, fornecedor.CNPJ);

            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorCode.VALIDATION_ERROR, result.ErrorCode);
            Assert.Equal("O valor base do fornecedor excede o saldo do orçamento do evento.", result.ErrorMessage);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Never);
        }

        [Fact]
        public async Task AddFornecedorByCnpjAsync_DeveAdicionar_ComSucesso()
        {
            var evento = new Evento("DevConf",
                new DateTime(2025, 1, 10), new DateTime(2025, 1, 11),
                "Centro", "Rua X", "", 100, 1000m, 1)
            { Id = 1 };

            var fornecedor = new Fornecedor { Id = 10, CNPJ = "11111111000191", NomeServico = "Buffet", ValorBase = 400m };

            _eventoRepoMock.Setup(r => r.GetByIdWithAggregatesAsync(1)).ReturnsAsync(evento);
            _fornecedorRepoMock.Setup(r => r.GetByCnpjAsync(fornecedor.CNPJ)).ReturnsAsync(fornecedor);
            _eventoRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Evento>())).Returns(Task.CompletedTask);

            var result = await _service.AddFornecedorByCnpjAsync(1, fornecedor.CNPJ);

            Assert.True(result.IsSuccess);
            Assert.Equal(fornecedor, result.Data);
            Assert.Single(evento.Fornecedores);
            Assert.Equal(10, evento.Fornecedores.First().FornecedorId);
            _eventoRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Evento>()), Times.Once);
        }
    }
}
