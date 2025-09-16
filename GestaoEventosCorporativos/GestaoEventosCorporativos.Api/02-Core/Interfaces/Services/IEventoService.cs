using GestaoEventosCorporativos.Api._01_Presentation.DTOs.Requests;
using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Shared;

namespace GestaoEventosCorporativos.Api._02_Core.Interfaces.Services
{
    public interface IEventoService
    {
        Task<Result<IEnumerable<Evento>>> GetAllAsync();
        Task<Result<Evento>> GetByIdAsync(int id);
        Task<Result<Evento>> AddAsync(Evento evento);
        Task<Result<Evento>> UpdateAsync(Evento evento);
        Task<Result<bool>> DeleteAsync(int id);
        Task<Result<Participante>> AddParticipanteByCpfAsync(int eventoId, string cpf);
    }
}
