using LinkUpPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUpPro.Infrastructure.Persistence.Configuration;

public class NotificacionConfiguration : IEntityTypeConfiguration<Notificacion>
{
    public void Configure(EntityTypeBuilder<Notificacion> builder)
    {
        builder.ToTable("Notificaciones");
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Mensaje).HasColumnType("VARCHAR(255)").IsRequired();
        builder.Property(n => n.TipoActividad).HasColumnType("VARCHAR(50)").IsRequired();
        builder.Property(n => n.Leida).HasColumnType("BIT").HasDefaultValue(false);
        builder.Property(n => n.FechaNotificacion).HasColumnType("DATETIME").HasDefaultValueSql("GETUTCDATE()");

        // Relacion con el Destinatario de la notificacion
        builder.HasOne(n => n.Usuario)
            .WithMany(u => u.Notificaciones)
            .HasForeignKey(n => n.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

        // con el remitente 
        builder.HasOne(n => n.Remitente)
            .WithMany()
            .HasForeignKey(n => n.RemitenteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relacion con la Publicacion 
        builder.HasOne(n => n.Publicacion)
            .WithMany()
            .HasForeignKey(n => n.PublicacionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
