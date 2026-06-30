using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LinkUpPro.Presentation.ViewModels.Amigo
{
    public class AmigoViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string NombreCompleto => $"{Nombre} {Apellido}";
        public string NombreUsuario { get; set; } = null!;
        public string? FotoPerfilUrl { get; set; }
        public int AmigosEnComun { get; set; }
        public DateTime FechaAmistad { get; set; }
    }

    public class AmigoListViewModel
    {
        public List<AmigoViewModel> Amigos { get; set; } = new List<AmigoViewModel>();
        public int TotalAmigos { get; set; }
        public int PublicacionesDisponibles { get; set; }
        public string? SearchQuery { get; set; }

        public List<LinkUpPro.Application.ViewModels.Publicacion.PublicacionViewModel> Publicaciones { get; set; } = new();
        public string? TextoBusquedaPub { get; set; }
        public int? AmigoIdPub { get; set; }
        public string? TipoContenidoPub { get; set; } = "Todos";
        public DateTime? FechaDesdePub { get; set; }
        public DateTime? FechaHastaPub { get; set; }
        public string? EstadoEdicionPub { get; set; } = "Todas";
    }
}
