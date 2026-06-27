using System;
using System.Collections.Generic;

namespace LinkUpPro.Domain.Entities;

public class Publicacion
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public string ContenidoTexto { get; set; } = null!;
    public string? ImagenUrl { get; set; }
    public string? YouTubeVideoUrl { get; set; }
    public string Privacidad { get; set; } = "SoloAmigos";
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion {get; set;}

    public bool PermiteComentarios {get; set;} = true;
    public Usuario Usuario { get; set; } = null!;
    public ICollection<Comentario> Comentarios { get; set; } = new List<Comentario>();
    public ICollection<Reaccion> Reacciones { get; set; } = new List<Reaccion>();
}
