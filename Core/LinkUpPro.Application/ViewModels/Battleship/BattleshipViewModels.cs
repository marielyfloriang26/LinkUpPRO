using LinkUpPro.Application.DTOs.Battleship;
using System.Collections.Generic;

namespace LinkUpPro.Application.ViewModels.Battleship;

public class BattleshipIndexViewModel
{
    public List<PartidaBattleshipDTO> PartidasActivas { get; set; } = new();
    public List<PartidaBattleshipDTO> HistorialPartidas { get; set; } = new();
    public int TotalJugadas { get; set; }
    public int TotalGanadas { get; set; }
    public int TotalPerdidas { get; set; }
}

public class IniciarPartidaViewModel
{
    public List<AmigoBattleshipViewModel> Amigos { get; set; } = new();
    public string? TerminoBusqueda { get; set; }
    public int? AmigoSeleccionadoId { get; set; }
}

public class AmigoBattleshipViewModel
{
    public int Id { get; set; }
    public string NombreUsuario { get; set; } = null!;
    public string NombreCompleto { get; set; } = null!;
    public string? FotoPerfilUrl { get; set; }
}

public class TableroViewModel
{
    public int PartidaId { get; set; }
    public bool EsMiTurno { get; set; }
    public string EstadoPartida { get; set; } = null!;
    public string OponenteNombre { get; set; } = null!;
    public List<CasillaTableroDTO> MisCasillas { get; set; } = new();
    public List<CasillaTableroDTO> CasillasOponente { get; set; } = new();
}

public class SeleccionarBarcoViewModel
{
    public int PartidaId { get; set; }
    public List<int> BarcosDisponibles { get; set; } = new();
    public int? BarcoSeleccionado { get; set; }
    public List<CasillaTableroDTO> MisCasillas { get; set; } = new();
}

public class PosicionarBarcoFormViewModel
{
    public int PartidaId { get; set; }
    public int Fila { get; set; }
    public int Columna { get; set; }
    public int TamañoBarco { get; set; }
    public string Direccion { get; set; } = null!;
}
