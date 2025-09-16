using GestaoEventosCorporativos.Api._01_Presentation.Helpers;
using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Enums;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Repositories;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Services;
using GestaoEventosCorporativos.Api._02_Core.Shared;

namespace GestaoEventosCorporativos.Api._02_Core.Services
{
    public class ParticipanteService : IParticipanteService
    {
        private readonly IParticipanteRepository _participanteRepository;

        public ParticipanteService(IParticipanteRepository participanteRepository)
        {
            _participanteRepository = participanteRepository;
        }

        public async Task<Result<Participante>> AddAsync(Participante participante)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(participante.NomeCompleto))
                    return Result<Participante>.Failure("O nome completo é obrigatório.", ErrorCode.VALIDATION_ERROR);

                if (string.IsNullOrWhiteSpace(participante.CPF))
                    return Result<Participante>.Failure("O CPF é obrigatório.", ErrorCode.VALIDATION_ERROR);

                if (!Enum.IsDefined(typeof(TipoParticipante), participante.Tipo))
                    return Result<Participante>.Failure("Tipo de participante inválido.", ErrorCode.VALIDATION_ERROR);

                var existing = await _participanteRepository.GetByCpfAsync(participante.CPF);
                if (existing != null)
                    return Result<Participante>.Failure("Já existe um participante com esse CPF.", ErrorCode.RESOURCE_ALREADY_EXISTS);

                await _participanteRepository.AddAsync(participante);
                return Result<Participante>.Success(participante);
            }
            catch (Exception)
            {
                return Result<Participante>.Failure("Erro ao criar participante.", ErrorCode.DATABASE_ERROR);
            }
        }


        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var participante = await _participanteRepository.GetByIdAsync(id);
                if (participante == null)
                    return Result<bool>.Failure("Participante não encontrado.", ErrorCode.NOT_FOUND);

                await _participanteRepository.DeleteAsync(participante);
                return Result<bool>.Success(true);
            }
            catch (Exception)
            {
                return Result<bool>.Failure("Erro ao excluir participante.", ErrorCode.DATABASE_ERROR);
            }
        }

        public async Task<Result<IEnumerable<Participante>>> GetAllAsync()
        {
            try
            {
                var participantes = await _participanteRepository.GetAllAsync();
                return Result<IEnumerable<Participante>>.Success(participantes);
            }
            catch (Exception)
            {
                return Result<IEnumerable<Participante>>.Failure("Erro ao buscar participantes.", ErrorCode.DATABASE_ERROR);
            }
        }

        public async Task<Result<Participante>> GetByIdAsync(int id)
        {
            try
            {
                var participante = await _participanteRepository.GetByIdAsync(id);
                if (participante == null)
                    return Result<Participante>.Failure("Participante não encontrado.", ErrorCode.NOT_FOUND);

                return Result<Participante>.Success(participante);
            }
            catch (Exception)
            {
                return Result<Participante>.Failure("Erro ao buscar participante.", ErrorCode.DATABASE_ERROR);
            }
        }

        public async Task<Result<Participante>> UpdateAsync(Participante participante)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(participante.NomeCompleto))
                    return Result<Participante>.Failure("O nome completo é obrigatório.", ErrorCode.VALIDATION_ERROR);

                if (string.IsNullOrWhiteSpace(participante.CPF))
                    return Result<Participante>.Failure("O CPF é obrigatório.", ErrorCode.VALIDATION_ERROR);

                if (!Enum.IsDefined(typeof(TipoParticipante), participante.Tipo))
                    return Result<Participante>.Failure("Tipo de participante inválido.", ErrorCode.VALIDATION_ERROR);

                var existing = await _participanteRepository.GetByIdAsync(participante.Id);
                if (existing == null)
                    return Result<Participante>.Failure("Participante não encontrado.", ErrorCode.NOT_FOUND);

                if (existing.CPF != participante.CPF)
                {
                    var cpfConflict = await _participanteRepository.GetByCpfAsync(participante.CPF);
                    if (cpfConflict != null)
                        return Result<Participante>.Failure("Já existe outro participante com esse CPF.", ErrorCode.RESOURCE_ALREADY_EXISTS);
                }

                await _participanteRepository.UpdateAsync(participante);
                return Result<Participante>.Success(participante);
            }
            catch (Exception)
            {
                return Result<Participante>.Failure("Erro ao atualizar participante.", ErrorCode.DATABASE_ERROR);
            }
        }
    }
}
