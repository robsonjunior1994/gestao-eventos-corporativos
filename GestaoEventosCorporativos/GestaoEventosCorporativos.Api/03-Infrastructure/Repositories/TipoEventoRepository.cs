using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Repositories;
using GestaoEventosCorporativos.Api._03_Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace GestaoEventosCorporativos.Api._03_Infrastructure.Repositories
{
    public class TipoEventoRepository : ITipoEventoRepository
    {
        private readonly AppDbContext _context;

        public TipoEventoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TipoEvento tipoEvento)
        {
            await _context.TiposEventos.AddAsync(tipoEvento);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TipoEvento tipoEvento)
        {
            _context.TiposEventos.Remove(tipoEvento);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TipoEvento>> GetAllAsync()
        {
            return await _context.TiposEventos
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<TipoEvento> GetByIdAsync(int id)
        {
            return await _context.TiposEventos
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task UpdateAsync(TipoEvento tipoEvento)
        {
            _context.TiposEventos.Update(tipoEvento);
            await _context.SaveChangesAsync();
        }
        public async Task<TipoEvento> GetByDescricaoAsync(string descricao)
        {
            return await _context.TiposEventos
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Descricao.ToLower() == descricao.ToLower());
        }
    }
}
