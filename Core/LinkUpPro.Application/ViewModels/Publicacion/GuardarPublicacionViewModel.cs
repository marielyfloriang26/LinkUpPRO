using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LinkUpPro.Application.ViewModels.Publicacion;

public class GuardarPublicacionViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El contenido de la publicación no puede estar vacío.")]
    [StringLength(500, ErrorMessage = "La publicación no puede exceder los 500 caracteres.")]
    public string ContenidoTexto { get; set; } = null!;

    public string? YouTubeVideoUrl { get; set; }
    
    [Required]
    public string Privacidad { get; set; } = "SoloAmigos";

    public IFormFile? ImagenArchivo { get; set; }
    //public string? NombreImagen { get; set; }
    public string? ImagenUrl { get; set; } 
}