using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LinkUpPro.Application.ViewModels.Publicacion;

public class GuardarPublicacionViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El contenido de la publicación no puede estar vacío.")]
    [StringLength(1000, ErrorMessage = "La publicación no puede exceder los 1000 caracteres.")]
    public string ContenidoTexto { get; set; } = null!;

    [RegularExpression(@"^(https?://)?(www\.)?(youtube\.com/watch\?v=|youtu\.be/|youtube\.com/shorts/)[a-zA-Z0-9_-]{11}(\S*)?$", 
        ErrorMessage = "Debe ingresar un enlace válido de YouTube.")]
    public string? YouTubeVideoUrl { get; set; }
    
    [Required]
    public string Privacidad { get; set; } = "SoloAmigos";

    public IFormFile? ImagenArchivo { get; set; }
    //public string? NombreImagen { get; set; }
    public string? ImagenUrl { get; set; } 
    public bool PermiteComentarios {get; set;} = true;
}