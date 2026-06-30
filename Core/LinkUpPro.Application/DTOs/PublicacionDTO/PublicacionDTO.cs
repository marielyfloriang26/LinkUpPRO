
namespace LinkUpPro.Application.DTOs.Publicacion;

public class PublicacionDto
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string ContenidoTexto { get; set; } = null!;
    public string? ImagenUrl { get; set; }
    public string? YouTubeVideoUrl { get; set; }
    public string Privacidad { get; set; } = "SoloAmigos";
    public bool PermiteComentarios { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
}