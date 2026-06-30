using System;

namespace LinkUpPro.Domain.Entities;

public class SolicitudAmistad
{
    public int Id { get; set; }
    public int EmisorId { get; set; }
    public int ReceptorId { get; set; }
    public string Estado { get; set; } = "En espera de respuesta";
    public DateTime FechaEnvio { get; set; }
    public DateTime? FechaRespuesta { get; set; }
    public bool OcultaParaEmisor { get; set; } = false;

    public Usuario Emisor { get; set; } = null!;
    public Usuario Receptor { get; set; } = null!;
}
