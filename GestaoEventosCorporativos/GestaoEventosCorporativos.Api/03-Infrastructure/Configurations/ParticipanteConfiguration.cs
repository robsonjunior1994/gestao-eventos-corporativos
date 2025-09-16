using GestaoEventosCorporativos.Api._02_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoEventosCorporativos.Api._03_Infrastructure.Configurations
{
    public class ParticipanteConfiguration : IEntityTypeConfiguration<Participante>
    {
        public void Configure(EntityTypeBuilder<Participante> builder)
        {
            builder.ToTable("Participantes");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.NomeCompleto)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(p => p.CPF)
                .IsRequired()
                .HasMaxLength(11);

            builder.Property(p => p.Telefone)
                .HasMaxLength(20);

            builder.HasMany(p => p.Eventos)
                .WithOne(pe => pe.Participante)
                .HasForeignKey(pe => pe.ParticipanteId);
        }
    }
}
