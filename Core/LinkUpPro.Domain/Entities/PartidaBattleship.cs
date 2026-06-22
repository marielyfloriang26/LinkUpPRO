using System;
using System.Collections.Generic;

namespace LinkUpPro.Domain.Entities;

public class PartidaBattleship
{
    public int Id { get; set; }
    public int Jugador1Id { get; set; }
    public int Jugador2Id { get; set; }
    public int TurnoUsuarioId { get; set; }
    public string EstadoPartida { get; set; } = "Configurando";
    public int? GanadorId { get; set; }
    public DateTime UltimoMovimiento { get; set; }
    public DateTime FechaInicio { get; set; }

    public Usuario Jugador1 { get; set; } = null!;
    public Usuario Jugador2 { get; set; } = null!;
    public Usuario TurnoUsuario { get; set; } = null!;
    public Usuario? Ganador { get; set; }
    
    public ICollection<CasillaTablero> Casillas { get; set; } = new List<CasillaTablero>();
}
