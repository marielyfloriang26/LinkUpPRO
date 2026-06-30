using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LinkUpPro.Presentation.ViewModels.Amigo
{
    public class AmigoViewModel
    {
        public string Id { get; set; }
        public string NombreCompleto { get; set; }
        public string NombreUsuario { get; set; }
        public string FotoPerfilUrl { get; set; }
        public string Profesion { get; set; }
        public int AmigosEnComun { get; set; }
        public DateTime FechaAmistad { get; set; }
    }

    public class AmigoListViewModel
    {
        public List<AmigoViewModel> Amigos { get; set; } = new List<AmigoViewModel>();
        public int TotalAmigos { get; set; }
        public string SearchQuery { get; set; }
    }
}
