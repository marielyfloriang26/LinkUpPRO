
namespace LinkUpPro.Application.ViewModels.Publicacion;

public class ComentarioViewModel
{
    public int Id { get; set; }
    public int PublicacionId { get; set; }
    public int UsuarioId { get; set; }
    public string AutorNombre { get; set; } = null!;
    public string? AutorFotoPerfil { get; set; }
    public string Contenido { get; set; } = null!;
    public int? ComentarioPadreId { get; set; }
    public DateTime? FechaModificacion { get; set; }
    public DateTime FechaCreacion { get; set; }

    // Soporte para respuestas anidadas 
    public List<ComentarioViewModel> Respuestas { get; set; } = new();
}