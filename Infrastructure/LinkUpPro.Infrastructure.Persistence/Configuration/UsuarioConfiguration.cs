using LinkUpPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUpPro.Infrastructure.Persistence.Configuration;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        // Renombra la tabla AspNetUsers nativa de Identity a "Usuarios" como requiere el proyecto
        builder.ToTable("Usuarios");

        // Configuramos los campos personalizados adicionales
        builder.Property(u => u.Nombre)
            .HasColumnType("VARCHAR(50)")
            .IsRequired();

        builder.Property(u => u.Apellido)
            .HasColumnType("VARCHAR(50)")
            .IsRequired();

        builder.Property(u => u.FotoPerfilUrl)
            .HasColumnType("VARCHAR(255)");

        builder.Property(u => u.EsActivo)
            .HasColumnType("BIT")
            .HasDefaultValue(false);

        builder.Property(u => u.FechaRegistro)
            .HasColumnType("DATETIME")
            .HasDefaultValueSql("GETUTCDATE()");
            
        // NOTA: Las propiedades heredadas de Identity (Email, UserName, PasswordHash, etc.) 
        // ya se autoconfiguran internamente y respetan los índices de unicidad de manera nativa.
    }
}