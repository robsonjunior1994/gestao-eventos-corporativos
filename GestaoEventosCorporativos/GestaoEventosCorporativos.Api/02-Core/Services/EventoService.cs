using GestaoEventosCorporativos.Api._01_Presentation.DTOs.Responses;
using GestaoEventosCorporativos.Api._01_Presentation.Helpers;
using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Repositories;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Services;
using GestaoEventosCorporativos.Api._02_Core.Shared;

namespace GestaoEventosCorporativos.Api._02_Core.Services
{
    public class EventoService : IEventoService
    {
        private readonly IEventoRepository _eventoRepository;
        private readonly ITipoEventoRepository _tipoEventoRepository;
        private readonly IParticipanteRepository _participanteRepository;
        private readonly IFornecedorRepository _fornecedorRepository;
        public EventoService(IEventoRepository eventoRepository, 
            ITipoEventoRepository tipoEventoRepository,
            IParticipanteRepository participanteRepository,
            IFornecedorRepository fornecedorRepository)
        {
            _eventoRepository = eventoRepository;
            _tipoEventoRepository = tipoEventoRepository;
            _participanteRepository = participanteRepository;
            _fornecedorRepository = fornecedorRepository;
        }
        public async Task<Result<Evento>> AddAsync(Evento evento)
        {
            try
            {
                // Regra de negócio 1: Data início < fim (ignorar horas)
                if (evento.DataInicio.Date >= evento.DataFim.Date)
                    return Result<Evento>.Failure(
                        "A data de início deve ser anterior à data de fim.",
                        ErrorCode.VALIDATION_ERROR
                    );

                // Regra de negócio 2: Lotação > 0
                if (evento.LotacaoMaxima < EventoRegras.LOTACAO_MINIMA)
                    return Result<Evento>.Failure("A lotação máxima deve ser maior que zero.", ErrorCode.VALIDATION_ERROR);

                // Regra de negócio 3: Orçamento > 0
                if (evento.OrcamentoMaximo < EventoRegras.ORCAMENTO_MINIMO)
                    return Result<Evento>.Failure("O orçamento deve ser maior que zero.", ErrorCode.VALIDATION_ERROR);

                // Regra de negócio 4: TipoEvento precisa existir
                var tipoEvento = await _tipoEventoRepository.GetByIdAsync(evento.TipoEventoId);
                if (tipoEvento == null)
                    return Result<Evento>.Failure("O tipo de evento informado não existe.", ErrorCode.NOT_FOUND);

                await _eventoRepository.AddAsync(evento);

                return Result<Evento>.Success(evento);
            }
            catch (Exception)
            {
                // logar erro aqui (ex: Serilog, Console, etc.)
                return Result<Evento>.Failure("Ocorreu um erro ao criar o evento.", ErrorCode.DATABASE_ERROR);
            }
        }


        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var evento = await _eventoRepository.GetByIdAsync(id);

                if (evento == null)
                    return Result<bool>.Failure("Evento não encontrado.", ErrorCode.NOT_FOUND);

                await _eventoRepository.DeleteAsync(evento);

                return Result<bool>.Success(true);
            }
            catch (Exception)
            {
                // logar erro aqui (Serilog, Console, etc.) se der tempo.
                return Result<bool>.Failure("Ocorreu um erro ao excluir o evento.", ErrorCode.DATABASE_ERROR);
            }
        }

        public async Task<Result<PagedResult<Evento>>> GetAllAsync(int pageNumber, int pageSize)
        {
            try
            {
                var (eventos, totalCount) = await _eventoRepository.GetAllWithAggregatesAsync(pageNumber, pageSize);

                var pagedResult = new PagedResult<Evento>
                {
                    Items = eventos,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };

                return Result<PagedResult<Evento>>.Success(pagedResult);
            }
            catch (Exception)
            {
                return Result<PagedResult<Evento>>.Failure("Erro ao buscar eventos.", ErrorCode.DATABASE_ERROR);
            }
        }

        public async Task<Result<Evento>> GetByIdAsync(int id)
        {
            try
            {
                var evento = await _eventoRepository.GetByIdWithAggregatesAsync(id);

                if (evento == null)
                    return Result<Evento>.Failure("Evento não encontrado.", ErrorCode.NOT_FOUND);

                return Result<Evento>.Success(evento);
            }
            catch (Exception)
            {
                // logar erro aqui (ex: Serilog, Console, etc.)
                return Result<Evento>.Failure("Ocorreu um erro ao buscar o evento.", ErrorCode.DATABASE_ERROR);
            }
        }

        public async Task<Result<Evento>> UpdateAsync(Evento evento)
        {
            try
            {
                // Regra de negócio 1: Data início < fim
                if (evento.DataInicio >= evento.DataFim)
                    return Result<Evento>.Failure("A data de início deve ser anterior à data de fim.", ErrorCode.VALIDATION_ERROR);

                // Regra de negócio 2: Lotação > 0
                if (evento.LotacaoMaxima < EventoRegras.LOTACAO_MINIMA)
                    return Result<Evento>.Failure("A lotação máxima deve ser maior que zero.", ErrorCode.VALIDATION_ERROR);

                // Regra de negócio 3: Orçamento > 0
                if (evento.OrcamentoMaximo < EventoRegras.ORCAMENTO_MINIMO)
                    return Result<Evento>.Failure("O orçamento deve ser maior que zero.", ErrorCode.VALIDATION_ERROR);

                // Regra de negócio 4: TipoEvento precisa existir, não estava especificado mas faz sentido então criei
                var tipoEvento = await _tipoEventoRepository.GetByIdAsync(evento.TipoEventoId);
                if (tipoEvento == null)
                    return Result<Evento>.Failure("O tipo de evento informado não existe.", ErrorCode.NOT_FOUND);

                await _eventoRepository.UpdateAsync(evento);
                return Result<Evento>.Success(evento);
            }
            catch (Exception)
            {
                // logar erro aqui (ex: Serilog, Console, etc.) se der tempo.
                return Result<Evento>.Failure("Ocorreu um erro ao atualizar o evento.", ErrorCode.DATABASE_ERROR);
            }
        }

        public async Task<Result<Participante>> AddParticipanteByCpfAsync(int eventoId, string cpf)
        {
            try
            {
                var evento = await _eventoRepository.GetByIdWithAggregatesAsync(eventoId);
                if (evento == null)
                    return Result<Participante>.Failure("Evento não encontrado.", ErrorCode.NOT_FOUND);

                var participante = await _participanteRepository.GetByCpfWithEventosAsync(cpf);
                if (participante == null)
                    return Result<Participante>.Failure("Participante não encontrado.", ErrorCode.NOT_FOUND);

                if (evento.Participantes.Any(p => p.ParticipanteId == participante.Id))
                    return Result<Participante>.Failure("Participante já está vinculado a este evento.", ErrorCode.RESOURCE_ALREADY_EXISTS);

                if (evento.Participantes.Count >= evento.LotacaoMaxima)
                    return Result<Participante>.Failure(
                        "Não é possível adicionar mais participantes. A lotação máxima do evento já foi atingida.",
                        ErrorCode.VALIDATION_ERROR
                    );

                foreach (var pe in participante.Eventos)
                {
                    var outroEvento = pe.Evento;
                    bool conflitoDatas = evento.DataInicio < outroEvento.DataFim && evento.DataFim > outroEvento.DataInicio;

                    if (conflitoDatas)
                    {
                        return Result<Participante>.Failure(
                            $"O participante já está inscrito no evento '{outroEvento.Nome}' que ocorre nas mesmas datas.",
                            ErrorCode.VALIDATION_ERROR
                        );
                    }
                }

                var participanteEvento = new ParticipanteEvento
                {
                    EventoId = evento.Id,
                    ParticipanteId = participante.Id,
                    Participante = participante
                };

                evento.Participantes.Add(participanteEvento);

                await _eventoRepository.UpdateAsync(evento);

                return Result<Participante>.Success(participante);
            }
            catch (Exception)
            {
                return Result<Participante>.Failure("Erro ao adicionar participante ao evento.", ErrorCode.DATABASE_ERROR);
            }
        }

        public async Task<Result<Fornecedor>> AddFornecedorByCnpjAsync(int eventoId, string cnpj)
        {
            try
            {
                var evento = await _eventoRepository.GetByIdWithAggregatesAsync(eventoId);
                if (evento == null)
                    return Result<Fornecedor>.Failure("Evento não encontrado.", ErrorCode.NOT_FOUND);

                if (string.IsNullOrWhiteSpace(cnpj))
                    return Result<Fornecedor>.Failure("O CNPJ é obrigatório.", ErrorCode.VALIDATION_ERROR);

                var fornecedor = await _fornecedorRepository.GetByCnpjAsync(cnpj);
                if (fornecedor == null)
                    return Result<Fornecedor>.Failure("Fornecedor não encontrado.", ErrorCode.NOT_FOUND);

                if (evento.Fornecedores.Any(f => f.FornecedorId == fornecedor.Id))
                    return Result<Fornecedor>.Failure("Fornecedor já está vinculado a este evento.", ErrorCode.RESOURCE_ALREADY_EXISTS);

                if (fornecedor.ValorBase > evento.SaldoOrcamento)
                    return Result<Fornecedor>.Failure("O valor base do fornecedor excede o saldo do orçamento do evento.", ErrorCode.VALIDATION_ERROR);

                var eventoFornecedor = new EventoFornecedor
                {
                    EventoId = evento.Id,
                    FornecedorId = fornecedor.Id,
                    ValorContratado = fornecedor.ValorBase,
                    Fornecedor = fornecedor
                };

                evento.Fornecedores.Add(eventoFornecedor);

                await _eventoRepository.UpdateAsync(evento);

                return Result<Fornecedor>.Success(fornecedor);
            }
            catch (Exception)
            {
                return Result<Fornecedor>.Failure("Erro ao adicionar fornecedor ao evento.", ErrorCode.DATABASE_ERROR);
            }
        }

        public async Task<Result<bool>> RemoveParticipanteByCpfAsync(int eventoId, string cpf)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cpf))
                    return Result<bool>.Failure("O CPF é obrigatório.", ErrorCode.VALIDATION_ERROR);

                var evento = await _eventoRepository.GetByIdWithAggregatesAsync(eventoId);
                if (evento == null)
                    return Result<bool>.Failure("Evento não encontrado.", ErrorCode.NOT_FOUND);

                var participante = await _participanteRepository.GetByCpfWithEventosAsync(cpf);
                if (participante == null)
                    return Result<bool>.Failure("Participante não encontrado.", ErrorCode.NOT_FOUND);

                var participanteEvento = evento.Participantes
                    .FirstOrDefault(p => p.ParticipanteId == participante.Id);

                if (participanteEvento == null)
                    return Result<bool>.Failure("Participante não está vinculado a este evento.", ErrorCode.NOT_FOUND);

                evento.Participantes.Remove(participanteEvento);

                await _eventoRepository.UpdateAsync(evento);

                return Result<bool>.Success(true);
            }
            catch (Exception)
            {
                return Result<bool>.Failure("Erro ao remover participante do evento.", ErrorCode.DATABASE_ERROR);
            }
        }

        public async Task<Result<bool>> RemoveFornecedorByCnpjAsync(int eventoId, string cnpj)
        {
            try
            {
                var evento = await _eventoRepository.GetByIdWithAggregatesAsync(eventoId);
                if (evento == null)
                    return Result<bool>.Failure("Evento não encontrado.", ErrorCode.NOT_FOUND);

                if (string.IsNullOrWhiteSpace(cnpj))
                    return Result<bool>.Failure("O CNPJ é obrigatório.", ErrorCode.VALIDATION_ERROR);

                var fornecedor = await _fornecedorRepository.GetByCnpjAsync(cnpj);
                if (fornecedor == null)
                    return Result<bool>.Failure("Fornecedor não encontrado.", ErrorCode.NOT_FOUND);

                var eventoFornecedor = evento.Fornecedores
                    .FirstOrDefault(f => f.FornecedorId == fornecedor.Id);

                if (eventoFornecedor == null)
                    return Result<bool>.Failure("Fornecedor não está vinculado a este evento.", ErrorCode.NOT_FOUND);

                evento.Fornecedores.Remove(eventoFornecedor);

                await _eventoRepository.UpdateAsync(evento);

                return Result<bool>.Success(true);
            }
            catch (Exception)
            {
                return Result<bool>.Failure("Erro ao remover fornecedor do evento.", ErrorCode.DATABASE_ERROR);
            }
        }
    }
}
