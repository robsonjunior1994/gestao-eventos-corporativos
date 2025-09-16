using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Repositories;
using GestaoEventosCorporativos.Api._03_Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;

namespace GestaoEventosCorporativos.Api._03_Infrastructure.Repositories
{
    public class EventoRepository : IEventoRepository
    {
        private readonly AppDbContext _context;

        public EventoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Evento>> GetAllAsync()
        {
            return await _context.Eventos
                .Include(e => e.TipoEvento)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Evento> GetByIdAsync(int id)
        {
            return await _context.Eventos
                .Include(e => e.TipoEvento)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Evento?> GetByIdWithAggregatesAsync(int id)
        {
            return await _context.Eventos
                .Include(e => e.TipoEvento)
                .Include(e => e.Participantes)
                    .ThenInclude(pe => pe.Participante)
                .Include(e => e.Fornecedores)
                    .ThenInclude(ef => ef.Fornecedor)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task AddAsync(Evento evento)
        {
            await _context.Eventos.AddAsync(evento);
            await _context.SaveChangesAsync(); 
        }

        public async Task UpdateAsync(Evento evento)
        {
            _context.Eventos.Update(evento);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Evento evento)
        {
            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();
        }
    }
}
