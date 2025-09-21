using GestaoEventosCorporativos.Api._02_Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestaoEventosCorporativos.Api._03_Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<TipoEvento> TiposEventos { get; set; }
        public DbSet<Participante> Participantes { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<ParticipanteEvento> ParticipantesEventos { get; set; }
        public DbSet<EventoFornecedor> EventosFornecedores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
