using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Repositories;
using GestaoEventosCorporativos.Api._03_Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace GestaoEventosCorporativos.Api._03_Infrastructure.Repositories
{
    public class FornecedorRepository : IFornecedorRepository
    {
        private readonly AppDbContext _context;

        public FornecedorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Fornecedor> Fornecedores, int TotalCount)> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Fornecedores.AsNoTracking();

            int totalCount = await query.CountAsync();

            var fornecedores = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (fornecedores, totalCount);
        }


        public async Task<Fornecedor> GetByIdAsync(int id)
        {
            return await _context.Fornecedores
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Fornecedor> GetByCnpjAsync(string cnpj)
        {
            return await _context.Fornecedores
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.CNPJ == cnpj);
        }

        public async Task AddAsync(Fornecedor fornecedor)
        {
            await _context.Fornecedores.AddAsync(fornecedor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Fornecedor fornecedor)
        {
            _context.Fornecedores.Update(fornecedor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Fornecedor fornecedor)
        {
            _context.Fornecedores.Remove(fornecedor);
            await _context.SaveChangesAsync();
        }
    }
}
