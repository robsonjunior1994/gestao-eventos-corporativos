using GestaoEventosCorporativos.Api._02_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoEventosCorporativos.Api._03_Infrastructure.Configurations
{
    public class ParticipanteEventoConfiguration : IEntityTypeConfiguration<ParticipanteEvento>
    {
        public void Configure(EntityTypeBuilder<ParticipanteEvento> builder)
        {
            builder.ToTable("ParticipantesEventos");

            builder.HasKey(pe => new { pe.ParticipanteId, pe.EventoId }); // chave composta

            builder.Property(pe => pe.DataInscricao)
                .HasDefaultValueSql("GETDATE()");

            builder.HasOne(pe => pe.Participante)
                .WithMany(p => p.Eventos)
                .HasForeignKey(pe => pe.ParticipanteId);

            builder.HasOne(pe => pe.Evento)
                .WithMany(e => e.Participantes)
                .HasForeignKey(pe => pe.EventoId);
        }
    }
}
