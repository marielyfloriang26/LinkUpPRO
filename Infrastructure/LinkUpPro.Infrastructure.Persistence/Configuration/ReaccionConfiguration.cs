using LinkUpPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUpPro.Infrastructure.Persistence.Configuration;

public class ReaccionConfiguration : IEntityTypeConfiguration<Reaccion>
{
    public void Configure(EntityTypeBuilder<Reaccion> builder)
    {
        builder.ToTable("Reacciones");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.TipoReaccion).HasColumnType("VARCHAR(10)").IsRequired();

        builder.HasOne(r => r.Usuario)
            .WithMany(u => u.Reacciones)
            .HasForeignKey(r => r.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Publicacion)
            .WithMany(p => p.Reacciones)
            .HasForeignKey(r => r.PublicacionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Comentario)
            .WithMany(c => c.Reacciones)
            .HasForeignKey(r => r.ComentarioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
