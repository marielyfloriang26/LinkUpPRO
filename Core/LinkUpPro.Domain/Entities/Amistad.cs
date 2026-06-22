using System;

namespace LinkUpPro.Domain.Entities;

public class Amistad
{
    public int Id { get; set; }
    public int UsuarioId1 { get; set; }
    public int UsuarioId2 { get; set; }
    public DateTime FechaAmistad { get; set; }

    public Usuario Usuario1 { get; set; } = null!;
    public Usuario Usuario2 { get; set; } = null!;
}
