using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Shared;

namespace GestaoEventosCorporativos.Api._02_Core.Interfaces.Services
{
    public interface ITipoEventoService
    {
        Task<Result<IEnumerable<TipoEvento>>> GetAllAsync();
        Task<Result<TipoEvento>> GetByIdAsync(int id);
        Task<Result<TipoEvento>> AddAsync(TipoEvento tipoEvento);
        Task<Result<TipoEvento>> UpdateAsync(TipoEvento tipoEvento);
        Task<Result<bool>> DeleteAsync(int id);
    }
}
