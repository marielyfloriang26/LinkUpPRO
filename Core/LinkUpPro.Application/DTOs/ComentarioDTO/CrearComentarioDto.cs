using System;

namespace LinkUpPro.Application.DTOs.ComentarioDTO
{
    public class CrearComentarioDto
    {
        public int PublicacionId { get; set; }
        public int UsuarioId { get; set; }
        public string Contenido { get; set; } = null!;
        public int? ComentarioPadreId { get; set; }
    }
}
