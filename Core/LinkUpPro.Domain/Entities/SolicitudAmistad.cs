using System;

namespace LinkUpPro.Domain.Entities;

public class SolicitudAmistad
{
    public int Id { get; set; }
    public int EmisorId { get; set; }
    public int ReceptorId { get; set; }
    public string Estado { get; set; } = "Pendiente";
    public DateTime FechaEnvio { get; set; }

    public Usuario Emisor { get; set; } = null!;
    public Usuario Receptor { get; set; } = null!;
}
