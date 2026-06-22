using LinkUpPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUpPro.Infrastructure.Persistence.Configuration;

public class PartidaBattleshipConfiguration : IEntityTypeConfiguration<PartidaBattleship>
{
    public void Configure(EntityTypeBuilder<PartidaBattleship> builder)
    {
        builder.ToTable("PartidasBattleship");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.EstadoPartida).HasColumnType("VARCHAR(20)").IsRequired().HasDefaultValue("Configurando");
        builder.Property(p => p.UltimoMovimiento).HasColumnType("DATETIME").HasDefaultValueSql("GETUTCDATE()");
        builder.Property(p => p.FechaInicio).HasColumnType("DATETIME").HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(p => p.Jugador1)
            .WithMany(u => u.PartidasIniciadas)
            .HasForeignKey(p => p.Jugador1Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Jugador2)
            .WithMany(u => u.PartidasAceptadas)
            .HasForeignKey(p => p.Jugador2Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.TurnoUsuario)
            .WithMany()
            .HasForeignKey(p => p.TurnoUsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Ganador)
            .WithMany(u => u.PartidasGanadas)
            .HasForeignKey(p => p.GanadorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
