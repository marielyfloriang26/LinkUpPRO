
namespace LinkUpPro.Application.DTOs.Publicacion;

public class PublicacionDto
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string ContenidoTexto { get; set; } = null!;
    public string? ImagenUrl { get; set; }
    public string? YouTubeVideoUrl { get; set; }
    public string Privacidad { get; set; } = "Publico";
    public DateTime FechaCreacion { get; set; }
}