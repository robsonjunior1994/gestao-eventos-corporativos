using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Shared;

namespace GestaoEventosCorporativos.Api._02_Core.Interfaces.Services
{
    public interface IParticipanteService
    {
        Task<Result<IEnumerable<Participante>>> GetAllAsync();
        Task<Result<Participante>> GetByIdAsync(int id);
        Task<Result<Participante>> AddAsync(Participante participante);
        Task<Result<Participante>> UpdateAsync(Participante participante);
        Task<Result<bool>> DeleteAsync(int id);
    }
}
