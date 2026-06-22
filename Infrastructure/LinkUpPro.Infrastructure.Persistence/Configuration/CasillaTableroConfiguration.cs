using LinkUpPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUpPro.Infrastructure.Persistence.Configuration;

public class CasillaTableroConfiguration : IEntityTypeConfiguration<CasillaTablero>
{
    public void Configure(EntityTypeBuilder<CasillaTablero> builder)
    {
        builder.ToTable("CasillasTablero");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Fila).HasColumnType("INT").IsRequired();
        builder.Property(c => c.Columna).HasColumnType("INT").IsRequired();
        builder.Property(c => c.TieneBarco).HasColumnType("BIT").HasDefaultValue(false);
        builder.Property(c => c.Impactada).HasColumnType("BIT").HasDefaultValue(false);

        builder.HasOne(c => c.Partida)
            .WithMany(p => p.Casillas)
            .HasForeignKey(c => c.PartidaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Usuario)
            .WithMany(u => u.CasillasTableros)
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
