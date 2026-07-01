using System.Collections.Generic;
using LinkUpPro.Application.ViewModels.Publicacion;
using LinkUpPro.Presentation.ViewModels.Amigo;

namespace LinkUpPro.Presentation.ViewModels.Perfil
{
    public class PerfilAmigoViewModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string NombreCompleto => $"{Nombre} {Apellido}";
        public string NombreUsuario { get; set; } = null!;
        public string? FotoPerfilUrl { get; set; }
        public int AmigosEnComun { get; set; }
        public List<PublicacionViewModel> Publicaciones { get; set; } = new();
    }
}
