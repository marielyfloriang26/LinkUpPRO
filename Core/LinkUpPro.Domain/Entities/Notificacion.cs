using System;

namespace LinkUpPro.Domain.Entities;

public class Notificacion
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string Mensaje { get; set; } = null!;
    public bool Leida { get; set; }
    public DateTime FechaNotificacion { get; set; }

    public Usuario Usuario { get; set; } = null!;
}
