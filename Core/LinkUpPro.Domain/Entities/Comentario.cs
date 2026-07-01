using System;
using System.Collections.Generic;
using LinkUpPro.Domain.Common;


namespace LinkUpPro.Domain.Entities;

public class Comentario : IEntidadPropietaria
{
    public int Id { get; set; }
    public int PublicacionId { get; set; }
    public int UsuarioId { get; set; }
    public string Contenido { get; set; } = null!;
    public int? ComentarioPadreId { get; set; }
    public DateTime FechaCreacion { get; set; }

    public Publicacion Publicacion { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
    public DateTime? FechaModificacion { get; set; }
    public Comentario? ComentarioPadre { get; set; }
    public ICollection<Comentario> Respuestas { get; set; } = new List<Comentario>();
    public ICollection<Reaccion> Reacciones { get; set; } = new List<Reaccion>();
}
