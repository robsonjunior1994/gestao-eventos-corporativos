using GestaoEventosCorporativos.Api._02_Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestaoEventosCorporativos.Api._03_Infrastructure.Configurations
{
    public class FornecedorConfiguration : IEntityTypeConfiguration<Fornecedor>
    {
        public void Configure(EntityTypeBuilder<Fornecedor> builder)
        {
            builder.ToTable("Fornecedores");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.NomeServico)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(f => f.CNPJ)
                .IsRequired()
                .HasMaxLength(14);

            builder.Property(f => f.ValorBase)
                .HasColumnType("decimal(18,2)");

            builder.HasMany(f => f.Eventos)
                .WithOne(ef => ef.Fornecedor)
                .HasForeignKey(ef => ef.FornecedorId);
        }
    }
}
