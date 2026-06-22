namespace LinkUpPro.Domain.Entities;

public class Reaccion
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int? PublicacionId { get; set; }
    public int? ComentarioId { get; set; }
    public string TipoReaccion { get; set; } = null!;

    public Usuario Usuario { get; set; } = null!;
    public Publicacion? Publicacion { get; set; }
    public Comentario? Comentario { get; set; }
}
