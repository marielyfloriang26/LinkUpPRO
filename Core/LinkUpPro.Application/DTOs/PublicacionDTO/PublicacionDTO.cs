
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
    public bool EstaEliminada { get; set; }
        // Propiedades para comentarios, autor y reacciones
    public string AutorNombreCompleto { get; set; } = null!;
    public string? AutorFotoPerfilUrl { get; set; }
    public int CantidadMeGusta { get; set; }
    public int CantidadNoMeGusta { get; set; }
    public List<ComentarioDTO.ComentarioDto> Comentarios { get; set; } = new();
}