using LinkUpPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUpPro.Infrastructure.Persistence.Configuration;

public class ComentarioConfiguration : IEntityTypeConfiguration<Comentario>
{
    public void Configure(EntityTypeBuilder<Comentario> builder)
    {
        builder.ToTable("Comentarios");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Contenido).HasColumnType("TEXT").IsRequired();
        builder.Property(c => c.FechaCreacion).HasColumnType("DATETIME").HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(c => c.Publicacion)
            .WithMany(p => p.Comentarios)
            .HasForeignKey(c => c.PublicacionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Usuario)
            .WithMany(u => u.Comentarios)
            .HasForeignKey(c => c.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.ComentarioPadre)
            .WithMany(cp => cp.Respuestas)
            .HasForeignKey(c => c.ComentarioPadreId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
