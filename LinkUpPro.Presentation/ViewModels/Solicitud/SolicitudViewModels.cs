using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LinkUpPro.Presentation.ViewModels.Solicitud
{
    public class UsuarioDisponibleViewModel
    {
        public string Id { get; set; }
        public string NombreCompleto { get; set; }
        public string NombreUsuario { get; set; }
        public string FotoPerfilUrl { get; set; }
        public int AmigosEnComun { get; set; }
    }

    public class SolicitudAmistadViewModel
    {
        public int Id { get; set; }
        public string UsuarioId { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public string NombreUsuario { get; set; } = null!;
        public string FotoPerfilUrl { get; set; } = null!;
        public DateTime FechaSolicitud { get; set; }
        public DateTime FechaEnvio { get; set; }
        public string Estado { get; set; } = null!;
        public int AmigosEnComun { get; set; }
    }

    public class SolicitudesIndexViewModel
    {
        public List<SolicitudAmistadViewModel> Pendientes { get; set; } = new List<SolicitudAmistadViewModel>();
        public List<SolicitudAmistadViewModel> Enviadas { get; set; } = new List<SolicitudAmistadViewModel>();
    }
}
