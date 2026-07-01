using LinkUpPro.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LinkUpPro.Infrastructure.Persistence.Contexts;

public class ApplicationDbContext : IdentityDbContext<Usuario, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /*public DbSet<Usuario> Usuarios { get; set; }*/
    public DbSet<Publicacion> Publicaciones { get; set; }
    public DbSet<Comentario> Comentarios { get; set; }
    public DbSet<Reaccion> Reacciones { get; set; }
    public DbSet<SolicitudAmistad> SolicitudesAmistad { get; set; }
    public DbSet<Amistad> Amistades { get; set; }
    public DbSet<Notificacion> Notificaciones { get; set; }
    public DbSet<PartidaBattleship> PartidasBattleship { get; set; }
    public DbSet<CasillaTablero> CasillasTablero { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
