using GestaoEventosCorporativos.Api._02_Core.Entities;

namespace GestaoEventosCorporativos.Api._02_Core.Interfaces.Repositories
{
    public interface IEventoRepository
    {
        Task<IEnumerable<Evento>> GetAllAsync();
        Task<Evento> GetByIdAsync(int id);
        Task AddAsync(Evento evento);
        Task UpdateAsync(Evento evento);
        Task DeleteAsync(Evento evento);
    }
}
