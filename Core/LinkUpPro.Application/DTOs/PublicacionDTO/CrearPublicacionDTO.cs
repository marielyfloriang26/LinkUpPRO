namespace LinkUpPro.Application.DTOs.PublicacionDTO;

public class CrearPublicacionDto
{
    public int UsuarioId { get; set; }
    public string ContenidoTexto { get; set; } = null!;
    public string? ImagenUrl { get; set; }
    public string? YouTubeVideoUrl { get; set; }
    public string Privacidad { get; set; } = "SoloAmigos";
}