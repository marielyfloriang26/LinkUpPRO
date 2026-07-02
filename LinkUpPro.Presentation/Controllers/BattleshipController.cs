using LinkUpPro.Application.DTOs.Battleship;
using LinkUpPro.Application.Interfaces.Services;
using LinkUpPro.Application.ViewModels.Battleship;
using LinkUpPro.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace LinkUpPro.Presentation.Controllers;

[Authorize]
public class BattleshipController : Controller
{
    private readonly IBattleshipService _battleshipService;
    private readonly IAmigoService _amigoService;
    private readonly UserManager<Usuario> _userManager;

    public BattleshipController(
        IBattleshipService battleshipService,
        IAmigoService amigoService,
        UserManager<Usuario> userManager)
    {
        _battleshipService = battleshipService;
        _amigoService = amigoService;
        _userManager = userManager;
    }

    private async Task<int> GetCurrentUserIdAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        return user?.Id ?? 0;
    }

    public async Task<IActionResult> Index()
    {
        int usuarioId = await GetCurrentUserIdAsync();
        ViewBag.UsuarioId = usuarioId;
        
        var activasResult = await _battleshipService.GetPartidasActivasAsync(usuarioId);
        var historialResult = await _battleshipService.GetHistorialPartidasAsync(usuarioId);

        var viewModel = new BattleshipIndexViewModel
        {
            PartidasActivas = activasResult.Data ?? new(),
            HistorialPartidas = historialResult.Data ?? new(),
            TotalJugadas = (historialResult.Data?.Count ?? 0) + (activasResult.Data?.Count ?? 0),
            TotalGanadas = historialResult.Data?.Count(p => p.GanadorId == usuarioId) ?? 0,
            TotalPerdidas = historialResult.Data?.Count(p => p.GanadorId != null && p.GanadorId != usuarioId) ?? 0
        };

        return View(viewModel);
    }

    public async Task<IActionResult> IniciarPartida(string? terminoBusqueda)
    {
        int usuarioId = await GetCurrentUserIdAsync();
        
        // Obtener amigos activos
        var amigosResult = await _amigoService.GetAmigosAsync(usuarioId);
        var amigos = amigosResult ?? new List<LinkUpPro.Application.DTOs.UsuarioDto>();

        if (!string.IsNullOrEmpty(terminoBusqueda))
        {
            amigos = amigos.Where(a => 
                a.Nombre.Contains(terminoBusqueda, System.StringComparison.OrdinalIgnoreCase) ||
                a.Apellido.Contains(terminoBusqueda, System.StringComparison.OrdinalIgnoreCase) ||
                a.NombreUsuario.Contains(terminoBusqueda, System.StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // Obtener partidas activas para filtrar amigos con los que ya se juega
        var activasResult = await _battleshipService.GetPartidasActivasAsync(usuarioId);
        var ocupados = activasResult.Data?.Select(p => p.Jugador1Id == usuarioId ? p.Jugador2Id : p.Jugador1Id).ToHashSet() ?? new();

        amigos = amigos.Where(a => !ocupados.Contains(a.Id)).ToList();

        var viewModel = new IniciarPartidaViewModel
        {
            Amigos = amigos.Select(a => new AmigoBattleshipViewModel
            {
                Id = a.Id,
                NombreUsuario = a.NombreUsuario,
                NombreCompleto = $"{a.Nombre} {a.Apellido}",
                FotoPerfilUrl = a.FotoPerfilUrl
            }).ToList(),
            TerminoBusqueda = terminoBusqueda
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> IniciarPartidaConfirm(int amigoSeleccionadoId)
    {
        int usuarioId = await GetCurrentUserIdAsync();
        var response = await _battleshipService.IniciarPartidaAsync(usuarioId, amigoSeleccionadoId);
        
        if (!response.Succeeded)
        {
            TempData["ErrorMessage"] = response.Message;
            return RedirectToAction(nameof(IniciarPartida));
        }

        return RedirectToAction(nameof(PosicionarBarcoSeleccion), new { partidaId = response.Data });
    }

    public async Task<IActionResult> PosicionarBarcoSeleccion(int partidaId)
    {
        int usuarioId = await GetCurrentUserIdAsync();
        var partidaResponse = await _battleshipService.GetPartidaAsync(partidaId, usuarioId);
        if (!partidaResponse.Succeeded)
        {
            TempData["ErrorMessage"] = partidaResponse.Message;
            return RedirectToAction(nameof(Index));
        }

        var partida = partidaResponse.Data;
        if (partida.EstadoPartida != "Configurando")
        {
            return RedirectToAction(nameof(Jugar), new { partidaId });
        }

        var faltantesResponse = await _battleshipService.GetBarcosFaltantesAsync(partidaId, usuarioId);
        var casillasResponse = await _battleshipService.GetTableroPropioAsync(partidaId, usuarioId);

        var viewModel = new SeleccionarBarcoViewModel
        {
            PartidaId = partidaId,
            BarcosDisponibles = faltantesResponse.Data ?? new(),
            MisCasillas = casillasResponse.Data ?? new()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SeleccionarCeldaParaBarco(int partidaId, int barcoSize)
    {
        return RedirectToAction(nameof(ElegirDireccionBarco), new { partidaId, barcoSize });
    }

    public async Task<IActionResult> ElegirDireccionBarco(int partidaId, int barcoSize, int? fila, int? columna)
    {
        int usuarioId = await GetCurrentUserIdAsync();
        var casillasResponse = await _battleshipService.GetTableroPropioAsync(partidaId, usuarioId);
        if (!casillasResponse.Succeeded)
        {
            TempData["ErrorMessage"] = casillasResponse.Message;
            return RedirectToAction(nameof(Index));
        }

        if (fila.HasValue && columna.HasValue)
        {
            var vm = new PosicionarBarcoFormViewModel
            {
                PartidaId = partidaId,
                TamañoBarco = barcoSize,
                Fila = fila.Value,
                Columna = columna.Value
            };
            return View("DireccionBarco", vm);
        }

        var viewModel = new SeleccionarBarcoViewModel
        {
            PartidaId = partidaId,
            BarcosDisponibles = new List<int> { barcoSize },
            MisCasillas = casillasResponse.Data ?? new()
        };
        ViewBag.SeleccionandoCelda = true;
        ViewBag.BarcoSize = barcoSize;

        return View("TableroParaCelda", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GuardarPosicionBarco(PosicionarBarcoFormViewModel model)
    {
        int usuarioId = await GetCurrentUserIdAsync();
        var dto = new PosicionarBarcoDTO
        {
            PartidaId = model.PartidaId,
            Fila = model.Fila,
            Columna = model.Columna,
            Direccion = model.Direccion,
            TamañoBarco = model.TamañoBarco
        };

        var result = await _battleshipService.PosicionarBarcoAsync(usuarioId, dto);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(ElegirDireccionBarco), new { partidaId = model.PartidaId, barcoSize = model.TamañoBarco, fila = model.Fila, columna = model.Columna });
        }

        return RedirectToAction(nameof(PosicionarBarcoSeleccion), new { partidaId = model.PartidaId });
    }

    public async Task<IActionResult> Jugar(int partidaId)
    {
        int usuarioId = await GetCurrentUserIdAsync();
        var partidaResponse = await _battleshipService.GetPartidaAsync(partidaId, usuarioId);
        
        if (!partidaResponse.Succeeded)
        {
            TempData["ErrorMessage"] = partidaResponse.Message;
            return RedirectToAction(nameof(Index));
        }

        var partida = partidaResponse.Data;
        
        if (partida.EstadoPartida == "Configurando")
        {
            return RedirectToAction(nameof(PosicionarBarcoSeleccion), new { partidaId });
        }

        if (partida.EstadoPartida == "Finalizada")
        {
            return RedirectToAction(nameof(Resultado), new { partidaId });
        }

        var tableroAtaque = await _battleshipService.GetTableroAtaqueAsync(partidaId, usuarioId);

        var viewModel = new TableroViewModel
        {
            PartidaId = partidaId,
            EsMiTurno = partida.TurnoUsuarioId == usuarioId,
            EstadoPartida = partida.EstadoPartida,
            OponenteNombre = partida.Jugador1Id == usuarioId ? partida.Jugador2Nombre : partida.Jugador1Nombre,
            CasillasOponente = tableroAtaque.Data ?? new()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Atacar(int partidaId, int fila, int columna)
    {
        int usuarioId = await GetCurrentUserIdAsync();
        
        var dto = new AtacarDTO
        {
            PartidaId = partidaId,
            Fila = fila,
            Columna = columna
        };

        var result = await _battleshipService.AtacarAsync(usuarioId, dto);
        
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
        }

        return RedirectToAction(nameof(Jugar), new { partidaId });
    }

    [HttpGet]
    public async Task<IActionResult> RendirseConfirmacion(int partidaId, string? origen = "Index")
    {
        int usuarioId = await GetCurrentUserIdAsync();
        var partidaResponse = await _battleshipService.GetPartidaAsync(partidaId, usuarioId);
        if (!partidaResponse.Succeeded)
        {
            TempData["ErrorMessage"] = partidaResponse.Message;
            return RedirectToAction(nameof(Index));
        }

        ViewBag.PartidaId = partidaId;
        ViewBag.Origen = origen;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Rendirse(int partidaId)
    {
        int usuarioId = await GetCurrentUserIdAsync();
        var result = await _battleshipService.RendirseAsync(partidaId, usuarioId);
        if (!result.Succeeded)
        {
            TempData["ErrorMessage"] = result.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Resultado(int partidaId)
    {
        int usuarioId = await GetCurrentUserIdAsync();
        var partidaResponse = await _battleshipService.GetPartidaAsync(partidaId, usuarioId);
        
        if (!partidaResponse.Succeeded)
        {
            TempData["ErrorMessage"] = partidaResponse.Message;
            return RedirectToAction(nameof(Index));
        }
        
        var tableroAtaque = await _battleshipService.GetTableroAtaqueAsync(partidaId, usuarioId);

        var viewModel = new TableroViewModel
        {
            PartidaId = partidaId,
            EsMiTurno = false,
            EstadoPartida = partidaResponse.Data.EstadoPartida,
            CasillasOponente = tableroAtaque.Data ?? new()
        };
        ViewBag.GanadorId = partidaResponse.Data.GanadorId;
        ViewBag.UsuarioId = usuarioId;

        return View(viewModel);
    }

    public async Task<IActionResult> VerMiTablero(int partidaId)
    {
        int usuarioId = await GetCurrentUserIdAsync();
        var partidaResponse = await _battleshipService.GetPartidaAsync(partidaId, usuarioId);
        if (!partidaResponse.Succeeded)
        {
            TempData["ErrorMessage"] = partidaResponse.Message;
            return RedirectToAction(nameof(Index));
        }

        var casillasResponse = await _battleshipService.GetTableroPropioAsync(partidaId, usuarioId);

        var viewModel = new TableroViewModel
        {
            PartidaId = partidaId,
            EstadoPartida = partidaResponse.Data.EstadoPartida,
            MisCasillas = casillasResponse.Data ?? new()
        };

        return View(viewModel);
    }

    public async Task<IActionResult> VerTableroRival(int partidaId)
    {
        int usuarioId = await GetCurrentUserIdAsync();
        var partidaResponse = await _battleshipService.GetPartidaAsync(partidaId, usuarioId);
        
        if (!partidaResponse.Succeeded)
        {
            TempData["ErrorMessage"] = partidaResponse.Message;
            return RedirectToAction(nameof(Index));
        }

        if (partidaResponse.Data.EstadoPartida != "Finalizada")
        {
            TempData["ErrorMessage"] = "No posee permisos para realizar esta acción.";
            return RedirectToAction(nameof(Index));
        }
        
        int oponenteId = partidaResponse.Data.Jugador1Id == usuarioId ? partidaResponse.Data.Jugador2Id : partidaResponse.Data.Jugador1Id;
        
        var casillasResponse = await _battleshipService.GetTableroPropioAsync(partidaId, oponenteId);

        var viewModel = new TableroViewModel
        {
            PartidaId = partidaId,
            MisCasillas = casillasResponse.Data ?? new()
        };

        return View(viewModel);
    }
}
