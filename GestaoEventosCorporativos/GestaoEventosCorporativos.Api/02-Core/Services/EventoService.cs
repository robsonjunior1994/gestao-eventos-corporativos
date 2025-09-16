using GestaoEventosCorporativos.Api._01_Presentation.Helpers;
using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Repositories;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Services;
using GestaoEventosCorporativos.Api._02_Core.Shared;
using GestaoEventosCorporativos.Api._03_Infrastructure.Repositories;

namespace GestaoEventosCorporativos.Api._02_Core.Services
{
    public class EventoService : IEventoService
    {
        private readonly IEventoRepository _eventoRepository;
        private readonly ITipoEventoRepository _tipoEventoRepository;

        public EventoService(IEventoRepository eventoRepository, ITipoEventoRepository tipoEventoRepository)
        {
            _eventoRepository = eventoRepository;
            _tipoEventoRepository = tipoEventoRepository;
        }
        public async Task<Result<Evento>> AddAsync(Evento evento)
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

                // Regra de negócio 4: TipoEvento precisa existir
                var tipoEvento = await _tipoEventoRepository.GetByIdAsync(evento.TipoEventoId);
                if (tipoEvento == null)
                    return Result<Evento>.Failure("O tipo de evento informado não existe.", ErrorCode.NOT_FOUND);

                // Persistência
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

        public async Task<Result<IEnumerable<Evento>>> GetAllAsync()
        {
            try
            {
                var eventos = await _eventoRepository.GetAllAsync();

                return Result<IEnumerable<Evento>>.Success(eventos);
            }
            catch (Exception)
            {
                // logar erro aqui (ex: Serilog, Console, etc.)
                return Result<IEnumerable<Evento>>.Failure("Ocorreu um erro ao buscar os eventos.", ErrorCode.DATABASE_ERROR);
            }
        }


        public async Task<Result<Evento>> GetByIdAsync(int id)
        {
            try
            {
                var evento = await _eventoRepository.GetByIdAsync(id);

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

                // Regra de negócio 4: TipoEvento precisa existir
                var tipoEvento = await _tipoEventoRepository.GetByIdAsync(evento.TipoEventoId);
                if (tipoEvento == null)
                    return Result<Evento>.Failure("O tipo de evento informado não existe.", ErrorCode.NOT_FOUND);

                // Persistência
                await _eventoRepository.UpdateAsync(evento);
                return Result<Evento>.Success(evento);
            }
            catch (Exception)
            {
                // logar erro aqui (ex: Serilog, Console, etc.) se der tempo.
                return Result<Evento>.Failure("Ocorreu um erro ao atualizar o evento.", ErrorCode.DATABASE_ERROR);
            }
        }
    }
}
