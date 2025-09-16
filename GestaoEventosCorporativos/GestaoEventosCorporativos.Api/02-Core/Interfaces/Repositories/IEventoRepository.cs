using GestaoEventosCorporativos.Api._02_Core.Entities;

namespace GestaoEventosCorporativos.Api._02_Core.Interfaces.Repositories
{
    public interface IEventoRepository
    {
        Task<IEnumerable<Evento>> GetAllAsync();
        Task<IEnumerable<Evento>> GetAllWithAggregatesAsync();
        Task<Evento> GetByIdAsync(int id);
        Task<Evento> GetByIdWithAggregatesAsync(int id);
        Task AddAsync(Evento evento);
        Task UpdateAsync(Evento evento);
        Task DeleteAsync(Evento evento);
    }
}
