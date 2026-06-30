using System;

namespace LinkUpPro.Application.DTOs;

public class SolicitudAmistadDto
{
    public int Id { get; set; }
    public int EmisorId { get; set; }
    public int ReceptorId { get; set; }
    public string Estado { get; set; } = null!;
    public DateTime FechaEnvio { get; set; }
    public UsuarioDto Emisor { get; set; } = null!;
    public UsuarioDto Receptor { get; set; } = null!;
}
