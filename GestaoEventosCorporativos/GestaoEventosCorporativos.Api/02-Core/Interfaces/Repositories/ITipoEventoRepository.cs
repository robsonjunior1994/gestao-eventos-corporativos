using GestaoEventosCorporativos.Api._02_Core.Entities;

namespace GestaoEventosCorporativos.Api._02_Core.Interfaces.Repositories
{
    public interface ITipoEventoRepository
    {
        Task<(IEnumerable<TipoEvento> Tipos, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
        Task<TipoEvento> GetByIdAsync(int id);
        Task<TipoEvento> GetByDescricaoAsync(string descricao);
        Task AddAsync(TipoEvento tipoEvento);
        Task UpdateAsync(TipoEvento tipoEvento);
        Task DeleteAsync(TipoEvento tipoEvento);
    }
}
