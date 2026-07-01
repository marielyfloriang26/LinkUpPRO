using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace LinkUpPro.Domain.Entities;

public class Usuario : IdentityUser<int>
{
    // IdentityUser<int> hereda automáticamente de forma segura:
    // Id, UserName, Email, PhoneNumber, PasswordHash, AccessFailedCount, LockoutEnd, EmailConfirmed, etc.
    
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string? FotoPerfilUrl { get; set; }
    public bool EsActivo { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    public DateTime? UltimoReenvioCorreo { get; set; }

    // Navigation properties
    public ICollection<Publicacion> Publicaciones { get; set; } = new List<Publicacion>();
    public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
    public ICollection<Reaccion> Reacciones { get; set; } = new List<Reaccion>();
    public ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
    public ICollection<SolicitudAmistad> SolicitudesEnviadas { get; set; } = new List<SolicitudAmistad>();
    public ICollection<SolicitudAmistad> SolicitudesRecibidas { get; set; } = new List<SolicitudAmistad>();
    public ICollection<Amistad> AmistadesIniciadas { get; set; } = new List<Amistad>();
    public ICollection<Amistad> AmistadesRecibidas { get; set; } = new List<Amistad>();
    public ICollection<PartidaBattleship> PartidasIniciadas { get; set; } = new List<PartidaBattleship>();
    public ICollection<PartidaBattleship> PartidasAceptadas { get; set; } = new List<PartidaBattleship>();
    public ICollection<PartidaBattleship> PartidasGanadas { get; set; } = new List<PartidaBattleship>();
    public ICollection<CasillaTablero> CasillasTableros { get; set; } = new List<CasillaTablero>();
}