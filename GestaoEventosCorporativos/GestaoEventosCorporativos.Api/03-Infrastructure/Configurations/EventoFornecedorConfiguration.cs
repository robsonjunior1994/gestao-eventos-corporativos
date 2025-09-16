using GestaoEventosCorporativos.Api._02_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoEventosCorporativos.Api._03_Infrastructure.Configurations
{
    public class EventoFornecedorConfiguration : IEntityTypeConfiguration<EventoFornecedor>
    {
        public void Configure(EntityTypeBuilder<EventoFornecedor> builder)
        {
            builder.ToTable("EventosFornecedores");

            builder.HasKey(ef => new { ef.EventoId, ef.FornecedorId }); // chave composta

            builder.Property(ef => ef.ValorContratado)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.HasOne(ef => ef.Evento)
                .WithMany(e => e.Fornecedores)
                .HasForeignKey(ef => ef.EventoId);

            builder.HasOne(ef => ef.Fornecedor)
                .WithMany(f => f.Eventos)
                .HasForeignKey(ef => ef.FornecedorId);
        }
    }
}
