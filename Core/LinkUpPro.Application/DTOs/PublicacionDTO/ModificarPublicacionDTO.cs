namespace LinkUpPro.Application.DTOs.Publicacion;

public class ModificarPublicacionDto
{
    public int Id { get; set; }
    public string ContenidoTexto { get; set; } = null!;
    public string? ImagenUrl { get; set; }
    public string? YouTubeVideoUrl { get; set; }
    public string Privacidad { get; set; } = "Publico";
}