using LinkUpPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUpPro.Infrastructure.Persistence.Configuration;

public class SolicitudAmistadConfiguration : IEntityTypeConfiguration<SolicitudAmistad>
{
    public void Configure(EntityTypeBuilder<SolicitudAmistad> builder)
    {
        builder.ToTable("SolicitudesAmistad");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Estado).HasMaxLength(50).IsRequired().HasDefaultValue("En espera de respuesta");
        builder.Property(s => s.FechaEnvio).HasColumnType("DATETIME").HasDefaultValueSql("GETUTCDATE()");
        builder.Property(s => s.FechaRespuesta).HasColumnType("DATETIME").IsRequired(false);
        builder.Property(s => s.OcultaParaEmisor).HasDefaultValue(false);

        builder.HasOne(s => s.Emisor)
            .WithMany(u => u.SolicitudesEnviadas)
            .HasForeignKey(s => s.EmisorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Receptor)
            .WithMany(u => u.SolicitudesRecibidas)
            .HasForeignKey(s => s.ReceptorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
