using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Shared;

namespace GestaoEventosCorporativos.Api._02_Core.Interfaces.Services
{
    public interface IFornecedorService
    {
        Task<Result<IEnumerable<Fornecedor>>> GetAllAsync();
        Task<Result<Fornecedor>> GetByIdAsync(int id);
        Task<Result<Fornecedor>> AddAsync(Fornecedor fornecedor);
        Task<Result<Fornecedor>> UpdateAsync(Fornecedor fornecedor);
        Task<Result<bool>> DeleteAsync(int id);
    }
}
