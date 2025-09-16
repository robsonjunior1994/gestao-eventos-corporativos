using GestaoEventosCorporativos.Api._02_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoEventosCorporativos.Api._03_Infrastructure.Configurations
{
    public class EventoConfiguration : IEntityTypeConfiguration<Evento>
    {
        public void Configure(EntityTypeBuilder<Evento> builder)
        {
            builder.ToTable("Eventos");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Nome)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.OrcamentoMaximo)
                .HasColumnType("decimal(18,2)");

            builder.HasOne(e => e.TipoEvento)
                .WithMany(t => t.Eventos)
                .HasForeignKey(e => e.TipoEventoId);

            builder.HasMany(e => e.Participantes)
                .WithOne(pe => pe.Evento)
                .HasForeignKey(pe => pe.EventoId);

            builder.HasMany(e => e.Fornecedores)
                .WithOne(ef => ef.Evento)
                .HasForeignKey(ef => ef.EventoId);
        }
    }
}
