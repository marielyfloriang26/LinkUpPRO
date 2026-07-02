using System;

namespace LinkUpPro.Application.ViewModels.Notificacion;

public class NotificacionViewModel
{
    public int Id { get; set; }
    public int? RemitenteId { get; set; }
    public string RemitenteNombreUsuario { get; set; } = null!;
    public string RemitenteFotoPerfilUrl { get; set; } = null!;
    public int? PublicacionId { get; set; }
    public string TipoActividad { get; set; } = null!;
    public string Mensaje { get; set; } = null!;
    public bool Leida { get; set; }
    public DateTime FechaNotificacion { get; set; }
}