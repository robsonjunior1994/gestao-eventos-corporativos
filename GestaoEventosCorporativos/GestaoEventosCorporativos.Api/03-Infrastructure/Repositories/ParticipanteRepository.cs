using GestaoEventosCorporativos.Api._02_Core.Entities;
using GestaoEventosCorporativos.Api._02_Core.Interfaces.Repositories;
using GestaoEventosCorporativos.Api._03_Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace GestaoEventosCorporativos.Api._03_Infrastructure.Repositories
{
    public class ParticipanteRepository : IParticipanteRepository
    {
        private readonly AppDbContext _context;

        public ParticipanteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Participante> Participantes, int TotalCount)> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Participantes.AsNoTracking();

            int totalCount = await query.CountAsync();

            var participantes = await query
                .OrderBy(p => p.NomeCompleto) // ordena por nome
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (participantes, totalCount);
        }


        public async Task<Participante> GetByIdAsync(int id)
        {
            return await _context.Participantes
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Participante> GetByCpfAsync(string cpf)
        {
            return await _context.Participantes
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.CPF == cpf);
        }

        public async Task<Participante> GetByCpfWithEventosAsync(string cpf)
        {
            return await _context.Participantes
                .Include(p => p.Eventos)
                    .ThenInclude(pe => pe.Evento)
                .FirstOrDefaultAsync(p => p.CPF == cpf);
        }


        public async Task AddAsync(Participante participante)
        {
            await _context.Participantes.AddAsync(participante);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Participante participante)
        {
            _context.Participantes.Update(participante);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Participante participante)
        {
            _context.Participantes.Remove(participante);
            await _context.SaveChangesAsync();
        }
    }
}
