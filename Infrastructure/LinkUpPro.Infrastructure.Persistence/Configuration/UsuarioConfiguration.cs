using LinkUpPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUpPro.Infrastructure.Persistence.Configuration;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuarios");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Nombre).HasColumnType("VARCHAR(50)").IsRequired();
        builder.Property(u => u.Apellido).HasColumnType("VARCHAR(50)").IsRequired();
        builder.Property(u => u.Correo).HasColumnType("VARCHAR(100)").IsRequired();
        builder.HasIndex(u => u.Correo).IsUnique();
        builder.Property(u => u.NombreUsuario).HasColumnType("VARCHAR(30)").IsRequired();
        builder.HasIndex(u => u.NombreUsuario).IsUnique();
        builder.Property(u => u.PasswordHash).HasColumnType("VARCHAR(255)").IsRequired();
        builder.Property(u => u.FotoPerfilUrl).HasColumnType("VARCHAR(255)");
        builder.Property(u => u.EsActivo).HasColumnType("BIT").HasDefaultValue(false);
        builder.Property(u => u.CodigoVerificacion).HasColumnType("VARCHAR(100)");
        builder.Property(u => u.IntentosFallidos).HasColumnType("INT").HasDefaultValue(0);
        builder.Property(u => u.BloqueoHasta).HasColumnType("DATETIME");
        builder.Property(u => u.FechaRegistro).HasColumnType("DATETIME").HasDefaultValueSql("GETUTCDATE()");
    }
}
