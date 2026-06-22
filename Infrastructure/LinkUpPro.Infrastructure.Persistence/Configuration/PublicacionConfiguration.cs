using LinkUpPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUpPro.Infrastructure.Persistence.Configuration;

public class PublicacionConfiguration : IEntityTypeConfiguration<Publicacion>
{
    public void Configure(EntityTypeBuilder<Publicacion> builder)
    {
        builder.ToTable("Publicaciones");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.ContenidoTexto).HasColumnType("TEXT").IsRequired();
        builder.Property(p => p.ImagenUrl).HasColumnType("VARCHAR(255)");
        builder.Property(p => p.YouTubeVideoUrl).HasColumnType("VARCHAR(255)");
        builder.Property(p => p.Privacidad).HasColumnType("VARCHAR(15)").IsRequired().HasDefaultValue("Publico");
        builder.Property(p => p.FechaCreacion).HasColumnType("DATETIME").HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(p => p.Usuario)
            .WithMany(u => u.Publicaciones)
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
