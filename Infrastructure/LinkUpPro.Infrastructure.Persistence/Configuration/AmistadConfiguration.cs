using LinkUpPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUpPro.Infrastructure.Persistence.Configuration;

public class AmistadConfiguration : IEntityTypeConfiguration<Amistad>
{
    public void Configure(EntityTypeBuilder<Amistad> builder)
    {
        builder.ToTable("Amistades");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.FechaAmistad).HasColumnType("DATETIME").HasDefaultValueSql("GETUTCDATE()");
        builder.Property(a => a.Estado).HasMaxLength(50).HasDefaultValue("Activa");

        builder.HasOne(a => a.Usuario1)
            .WithMany(u => u.AmistadesIniciadas)
            .HasForeignKey(a => a.UsuarioId1)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Usuario2)
            .WithMany(u => u.AmistadesRecibidas)
            .HasForeignKey(a => a.UsuarioId2)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

