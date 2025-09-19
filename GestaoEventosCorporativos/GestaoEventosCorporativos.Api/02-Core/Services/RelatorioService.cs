using GestaoEventosCorporativos.Api._01_Presentation.DTOs.Responses;
using GestaoEventosCorporativos.Api._01_Presentation.Helpers;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Repositories;
using GestaoEventosCorporativos.Api._02_Core.Shared;
using GestaoEventosCorporativos.Api._03_Infrastructure.Repositories;

namespace GestaoEventosCorporativos.Api._02_Core.Services
{
    public class RelatorioService : IRelatorioService
    {
        private readonly IParticipanteRepository _participanteRepository;
        private readonly IFornecedorRepository _fornecedorRepository;
        private readonly IEventoRepository _eventoRepository;

        public RelatorioService(
            IParticipanteRepository participanteRepository,
            IFornecedorRepository fornecedorRepository,
            IEventoRepository eventoRepository)
        {
            _participanteRepository = participanteRepository;
            _fornecedorRepository = fornecedorRepository;
            _eventoRepository = eventoRepository;
        }

        public async Task<Result<AgendaParticipanteResponse>> GetAgendaParticipanteAsync(string cpf)
        {
            try
            {
                var participante = await _participanteRepository.GetByCpfWithEventosAsync(cpf);
                if (participante == null)
                    return Result<AgendaParticipanteResponse>.Failure("Participante não encontrado.", ErrorCode.NOT_FOUND);

                var response = new AgendaParticipanteResponse
                {
                    NomeParticipante = participante.NomeCompleto,
                    CPF = participante.CPF,
                    Eventos = participante.Eventos
                        .Select(e => new EventoAgendaResponse
                        {
                            EventoId = e.EventoId,
                            NomeEvento = e.Evento.Nome,
                            DataInicio = e.Evento.DataInicio,
                            DataFim = e.Evento.DataFim,
                            Local = e.Evento.Local
                        })
                        .OrderBy(e => e.DataInicio)
                        .ToList()
                };

                return Result<AgendaParticipanteResponse>.Success(response);
            }
            catch (Exception)
            {
                return Result<AgendaParticipanteResponse>.Failure("Erro ao gerar a agenda do participante.", ErrorCode.DATABASE_ERROR);
            }
        }

        public async Task<Result<IEnumerable<FornecedorUtilizacaoResponse>>> GetFornecedoresMaisUtilizadosAsync(
            int pageNumber = 1, int pageSize = 1000) // default grande p/ buscar tudo
        {
            try
            {
                var (eventos, _) = await _eventoRepository.GetAllWithAggregatesAsync(pageNumber, pageSize);

                var fornecedores = eventos
                    .SelectMany(e => e.Fornecedores)
                    .GroupBy(f => new { f.Fornecedor.Id, f.Fornecedor.NomeServico, f.Fornecedor.CNPJ })
                    .Select(g => new FornecedorUtilizacaoResponse
                    {
                        NomeServico = g.Key.NomeServico,
                        CNPJ = g.Key.CNPJ,
                        QuantidadeEventos = g.Count(),
                        ValorTotalContratado = g.Sum(x => x.ValorContratado)
                    })
                    .OrderByDescending(f => f.QuantidadeEventos)
                    .ThenByDescending(f => f.ValorTotalContratado)
                    .ToList();

                return Result<IEnumerable<FornecedorUtilizacaoResponse>>.Success(fornecedores);
            }
            catch (Exception)
            {
                return Result<IEnumerable<FornecedorUtilizacaoResponse>>.Failure(
                    "Erro ao gerar relatório de fornecedores mais utilizados.",
                    ErrorCode.DATABASE_ERROR
                );
            }
        }

        public async Task<Result<IEnumerable<TipoParticipanteFrequenciaResponse>>> GetTiposParticipantesMaisFrequentesAsync(int pageNumber = 1, int pageSize = 1000)
        {
            try
            {
                var (eventos, _) = await _eventoRepository.GetAllWithAggregatesAsync(pageNumber, pageSize);

                var tipos = eventos
                    .SelectMany(e => e.Participantes)
                    .Where(p => p.Participante != null)
                    .GroupBy(p => p.Participante.Tipo)
                    .Select(g => new TipoParticipanteFrequenciaResponse
                    {
                        Tipo = g.Key.ToString(),
                        Quantidade = g.Count()
                    })
                    .OrderByDescending(t => t.Quantidade)
                    .ToList();

                return Result<IEnumerable<TipoParticipanteFrequenciaResponse>>.Success(tipos);
            }
            catch (Exception)
            {
                return Result<IEnumerable<TipoParticipanteFrequenciaResponse>>.Failure(
                    "Erro ao gerar relatório de tipos de participantes.",
                    ErrorCode.DATABASE_ERROR
                );
            }
        }

        public async Task<Result<IEnumerable<SaldoEventoResponse>>> GetSaldoOrcamentoEventosAsync(int pageNumber = 1, int pageSize = 1000)
        {
            try
            {
                var (eventos, _) = await _eventoRepository.GetAllWithAggregatesAsync(pageNumber, pageSize);

                var saldos = eventos
                    .Select(e => new SaldoEventoResponse
                    {
                        EventoId = e.Id,
                        Nome = e.Nome,
                        OrcamentoMaximo = e.OrcamentoMaximo,
                        ValorTotalFornecedores = e.ValorTotalFornecedores,
                        SaldoOrcamento = e.SaldoOrcamento
                    })
                    .ToList();

                return Result<IEnumerable<SaldoEventoResponse>>.Success(saldos);
            }
            catch (Exception)
            {
                return Result<IEnumerable<SaldoEventoResponse>>.Failure(
                    "Erro ao gerar relatório de saldos de orçamento dos eventos.",
                    ErrorCode.DATABASE_ERROR
                );
            }
        }


    }
}
