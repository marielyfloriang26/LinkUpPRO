using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkUpPro.Application.DTOs.Battleship;

public class PartidaBattleshipDTO
{
    public int Id { get; set; }
    public int Jugador1Id { get; set; }
    public string Jugador1Nombre { get; set; } = null!;
    public int Jugador2Id { get; set; }
    public string Jugador2Nombre { get; set; } = null!;
    public int TurnoUsuarioId { get; set; }
    public string EstadoPartida { get; set; } = null!;
    public int? GanadorId { get; set; }
    public string? GanadorNombre { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime UltimoMovimiento { get; set; }
    public double DuracionHoras { get; set; }
}

public class NuevaPartidaDTO
{
    public int Jugador2Id { get; set; }
}

public class PosicionarBarcoDTO
{
    public int PartidaId { get; set; }
    public int Fila { get; set; }
    public int Columna { get; set; }
    public string Direccion { get; set; } = null!; // "arriba", "abajo", "derecha", "izquierda"
    public int TamañoBarco { get; set; }
}

public class AtacarDTO
{
    public int PartidaId { get; set; }
    public int Fila { get; set; }
    public int Columna { get; set; }
}

public class CasillaTableroDTO
{
    public int Fila { get; set; }
    public int Columna { get; set; }
    public bool TieneBarco { get; set; }
    public bool Impactada { get; set; }
}
