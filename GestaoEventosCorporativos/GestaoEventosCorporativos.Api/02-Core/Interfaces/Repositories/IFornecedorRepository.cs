using GestaoEventosCorporativos.Api._02_Core.Entities;

namespace GestaoEventosCorporativos.Api._02_Core.Interfaces.Repositories
{
    public interface IFornecedorRepository
    {
        Task<(IEnumerable<Fornecedor> Fornecedores, int TotalCount)> GetAllAsync(int pageNumber, int pageSize);
        Task<Fornecedor> GetByIdAsync(int id);
        Task<Fornecedor> GetByCnpjAsync(string cnpj);

        Task AddAsync(Fornecedor fornecedor);
        Task UpdateAsync(Fornecedor fornecedor);
        Task DeleteAsync(Fornecedor fornecedor);
    }
}
