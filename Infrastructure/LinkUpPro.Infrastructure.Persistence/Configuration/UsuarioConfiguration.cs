using LinkUpPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUpPro.Infrastructure.Persistence.Configuration;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        // Esto le dice a EF que mapee el modelo a la tabla de Identity
        builder.ToTable("Usuarios");

        // Tus campos personalizados
        builder.Property(u => u.Nombre).HasColumnType("VARCHAR(50)").IsRequired();
        builder.Property(u => u.Apellido).HasColumnType("VARCHAR(50)").IsRequired();
        builder.Property(u => u.FotoPerfilUrl).HasColumnType("VARCHAR(255)");
        builder.Property(u => u.FechaRegistro).HasColumnType("DATETIME").HasDefaultValueSql("GETUTCDATE()");

        // NOTA IMPORTANTE:
        // No mapees manualmente Email, UserName, PasswordHash o AccessFailedCount.
        // Identity ya lo hace internamente y sus nombres de columna son fijos (ej. 'Email', 'NormalizedUserName').
        // Si necesitas que el campo 'Email' sea 'Correo' en la BD, se hace mediante un Map, 
        // pero para evitar errores en las migraciones, te recomiendo dejar que Identity use sus nombres estándar.
    }
}