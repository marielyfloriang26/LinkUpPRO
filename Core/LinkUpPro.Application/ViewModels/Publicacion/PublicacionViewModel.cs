namespace LinkUpPro.Application.ViewModels.Publicacion;

public class PublicacionViewModel
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string AutorNombreCompleto { get; set; } = null!;
    public string? AutorFotoPerfilUrl { get; set; }
    public string ContenidoTexto { get; set; } = null!;
    public string? ImagenUrl { get; set; }
    public string? YouTubeVideoUrl { get; set; }
    public string Privacidad { get; set; } = "Publico";
    public DateTime FechaCreacion { get; set; }

    // Totales calculados para los botones de interaccion
    public int CantidadMeGusta { get; set; }
    public int CantidadNoMeGusta { get; set; }
    
    public string? ReaccionUsuarioAutenticado { get; set; } 

    // Lista de comentarios ya listos para renderizar
    public List<ComentarioViewModel> Comentarios { get; set; } = new();
}