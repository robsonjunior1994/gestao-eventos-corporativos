using GestaoEventosCorporativos.Api._02_Core.Entities;

namespace GestaoEventosCorporativos.Api._02_Core.Interfaces.Repositories
{
    public interface IParticipanteRepository
    {
        Task<IEnumerable<Participante>> GetAllAsync();
        Task<Participante> GetByIdAsync(int id);
        Task<Participante> GetByCpfAsync(string cpf);
        Task<Participante> GetByCpfWithEventosAsync(string cpf);
        Task AddAsync(Participante participante);
        Task UpdateAsync(Participante participante);
        Task DeleteAsync(Participante participante);
    }
}
