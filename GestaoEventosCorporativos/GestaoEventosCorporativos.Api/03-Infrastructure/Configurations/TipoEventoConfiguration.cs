using GestaoEventosCorporativos.Api._02_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoEventosCorporativos.Api._03_Infrastructure.Configurations
{
    public class TipoEventoConfiguration : IEntityTypeConfiguration<TipoEvento>
    {
        public void Configure(EntityTypeBuilder<TipoEvento> builder)
        {
            builder.ToTable("TiposEventos");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Descricao)
                .IsRequired()
                .HasMaxLength(100);
        }
    }
}
