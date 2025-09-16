using GestaoEventosCorporativos.Api._01_Presentation.Helpers;
using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Repositories;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Services;
using GestaoEventosCorporativos.Api._02_Core.Shared;

namespace GestaoEventosCorporativos.Api._02_Core.Services
{
    public class TipoEventoService : ITipoEventoService
    {
        private readonly ITipoEventoRepository _tipoEventoRepository;

        public TipoEventoService(ITipoEventoRepository tipoEventoRepository)
        {
            _tipoEventoRepository = tipoEventoRepository;
        }

        public async Task<Result<TipoEvento>> AddAsync(TipoEvento tipoEvento)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tipoEvento.Descricao))
                    return Result<TipoEvento>.Failure("A descrição do tipo de evento é obrigatória.", ErrorCode.VALIDATION_ERROR);

                var existingTipo = await _tipoEventoRepository.GetByDescricaoAsync(tipoEvento.Descricao);

                if (existingTipo != null)
                    return Result<TipoEvento>.Failure("Já existe um tipo de evento com essa descrição.", ErrorCode.RESOURCE_ALREADY_EXISTS);

                await _tipoEventoRepository.AddAsync(tipoEvento);
                return Result<TipoEvento>.Success(tipoEvento);
            }
            catch (Exception)
            {
                return Result<TipoEvento>.Failure("Erro ao criar tipo de evento.", ErrorCode.DATABASE_ERROR);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var tipoEvento = await _tipoEventoRepository.GetByIdAsync(id);

                if (tipoEvento == null)
                    return Result<bool>.Failure("Tipo de evento não encontrado.", ErrorCode.NOT_FOUND);

                await _tipoEventoRepository.DeleteAsync(tipoEvento);
                return Result<bool>.Success(true);
            }
            catch (Exception)
            {
                return Result<bool>.Failure("Erro ao excluir tipo de evento.", ErrorCode.DATABASE_ERROR);
            }
        }

        public async Task<Result<IEnumerable<TipoEvento>>> GetAllAsync()
        {
            try
            {
                var tipos = await _tipoEventoRepository.GetAllAsync();
                return Result<IEnumerable<TipoEvento>>.Success(tipos);
            }
            catch (Exception)
            {
                return Result<IEnumerable<TipoEvento>>.Failure("Erro ao buscar tipos de evento.", ErrorCode.DATABASE_ERROR);
            }
        }

        public async Task<Result<TipoEvento>> GetByIdAsync(int id)
        {
            try
            {
                var tipoEvento = await _tipoEventoRepository.GetByIdAsync(id);

                if (tipoEvento == null)
                    return Result<TipoEvento>.Failure("Tipo de evento não encontrado.", ErrorCode.NOT_FOUND);

                return Result<TipoEvento>.Success(tipoEvento);
            }
            catch (Exception)
            {
                return Result<TipoEvento>.Failure("Erro ao buscar tipo de evento.", ErrorCode.DATABASE_ERROR);
            }
        }

        public async Task<Result<TipoEvento>> UpdateAsync(TipoEvento tipoEvento)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tipoEvento.Descricao))
                    return Result<TipoEvento>.Failure("A descrição do tipo de evento é obrigatória.", ErrorCode.VALIDATION_ERROR);

                var existingTipo = await _tipoEventoRepository.GetByIdAsync(tipoEvento.Id);

                if (existingTipo == null)
                    return Result<TipoEvento>.Failure("Tipo de evento não encontrado.", ErrorCode.NOT_FOUND);

                existingTipo.Descricao = tipoEvento.Descricao;

                await _tipoEventoRepository.UpdateAsync(existingTipo);
                return Result<TipoEvento>.Success(existingTipo);
            }
            catch (Exception)
            {
                return Result<TipoEvento>.Failure("Erro ao atualizar tipo de evento.", ErrorCode.DATABASE_ERROR);
            }
        }
    }
}
