using AutoMapper;
using LinkUpPro.Application.DTOs.Battleship;
using LinkUpPro.Application.Interfaces.Repositories;
using LinkUpPro.Application.Interfaces.Services;
using LinkUpPro.Application.Wrappers;
using LinkUpPro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LinkUpPro.Application.Services;

public class BattleshipService : IBattleshipService
{
    private readonly IBattleshipRepository _battleshipRepository;
    private readonly IAmistadRepository _amistadRepository;
    private readonly IMapper _mapper;

    public BattleshipService(
        IBattleshipRepository battleshipRepository,
        IAmistadRepository amistadRepository,
        IMapper mapper)
    {
        _battleshipRepository = battleshipRepository;
        _amistadRepository = amistadRepository;
        _mapper = mapper;
    }

    public async Task<Response<List<PartidaBattleshipDTO>>> GetPartidasActivasAsync(int usuarioId)
    {
        await CheckAbandonoAutomáticoAsync(usuarioId);

        var partidas = await _battleshipRepository.GetPartidasActivasByUsuarioIdAsync(usuarioId);
        var dtos = partidas.Select(p => MapToDTO(p)).ToList();
        return new Response<List<PartidaBattleshipDTO>>(dtos);
    }

    public async Task<Response<List<PartidaBattleshipDTO>>> GetHistorialPartidasAsync(int usuarioId)
    {
        await CheckAbandonoAutomáticoAsync(usuarioId);

        var partidas = await _battleshipRepository.GetHistorialPartidasByUsuarioIdAsync(usuarioId);
        var dtos = partidas.Select(p => MapToDTO(p)).ToList();
        return new Response<List<PartidaBattleshipDTO>>(dtos);
    }

    public async Task<Response<int>> IniciarPartidaAsync(int usuario1Id, int usuario2Id)
    {
        // Check if there is an active friendship
        var amistad = await _amistadRepository.GetAmistadEntreUsuariosAsync(usuario1Id, usuario2Id);
        if (amistad == null || amistad.Estado == "Eliminada")
            return new Response<int>("Debe existir una amistad activa para iniciar una partida.");

        // Check if there's already an active match
        var partidaActiva = await _battleshipRepository.GetPartidaActivaEntreUsuariosAsync(usuario1Id, usuario2Id);
        if (partidaActiva != null)
            return new Response<int>("Ya existe una partida activa con este usuario.");

        var nuevaPartida = new PartidaBattleship
        {
            Jugador1Id = usuario1Id,
            Jugador2Id = usuario2Id,
            TurnoUsuarioId = usuario1Id, // initially Jugador1
            EstadoPartida = "Configurando",
            FechaInicio = DateTime.UtcNow,
            UltimoMovimiento = DateTime.UtcNow
        };

        var partidaCreada = await _battleshipRepository.AddAsync(nuevaPartida);
        return new Response<int>(partidaCreada.Id);
    }

    public async Task<Response<bool>> RendirseAsync(int partidaId, int usuarioId)
    {
        await CheckAbandonoAutomáticoAsync(usuarioId);

        var partida = await _battleshipRepository.GetPartidaByIdWithIncludesAsync(partidaId);
        if (partida == null || partida.EstadoPartida == "Finalizada")
            return new Response<bool>("La partida no existe o ya ha finalizado.");

        if (partida.Jugador1Id != usuarioId && partida.Jugador2Id != usuarioId)
            return new Response<bool>("No posee permisos para realizar esta acción.");

        int ganadorId = partida.Jugador1Id == usuarioId ? partida.Jugador2Id : partida.Jugador1Id;

        partida.EstadoPartida = "Finalizada";
        partida.GanadorId = ganadorId;
        partida.UltimoMovimiento = DateTime.UtcNow;

        await _battleshipRepository.UpdateAsync(partida);

        return new Response<bool>(true);
    }

    public async Task<Response<bool>> PosicionarBarcoAsync(int usuarioId, PosicionarBarcoDTO dto)
    {
        var partida = await _battleshipRepository.GetPartidaByIdWithIncludesAsync(dto.PartidaId);
        if (partida == null || partida.EstadoPartida != "Configurando")
            return new Response<bool>("La partida no está en fase de configuración.");

        if (partida.Jugador1Id != usuarioId && partida.Jugador2Id != usuarioId)
            return new Response<bool>("No posee permisos para realizar esta acción.");

        // Calculate positions based on direction
        List<(int fila, int columna)> posiciones = new();
        int f = dto.Fila;
        int c = dto.Columna;

        string dirNormalized = dto.Direccion.ToLower().Replace("hacia ", "").Trim();

        for (int i = 0; i < dto.TamañoBarco; i++)
        {
            posiciones.Add((f, c));
            switch (dirNormalized)
            {
                case "arriba": f--; break;
                case "abajo": f++; break;
                case "izquierda": c--; break;
                case "derecha": c++; break;
                default: return new Response<bool>("Dirección no válida.");
            }
        }

        // Validate limits
        if (posiciones.Any(p => p.fila < 1 || p.fila > 12 || p.columna < 1 || p.columna > 12))
            return new Response<bool>("La combinación actual no es válida y debe modificar la celda o la dirección seleccionada para poder continuar.");

        // Validate superpositions
        var casillasPropias = await _battleshipRepository.GetCasillasByPartidaAndUsuarioAsync(dto.PartidaId, usuarioId);
        
        foreach (var pos in posiciones)
        {
            if (casillasPropias.Any(c => c.Fila == pos.fila && c.Columna == pos.columna && c.TieneBarco))
                return new Response<bool>("Debe cambiar la celda seleccionada o la dirección, ya que con la combinación actual el barco quedaría posicionado encima de otro barco.");
        }

        // Save positions
        foreach (var pos in posiciones)
        {
            var casilla = casillasPropias.FirstOrDefault(c => c.Fila == pos.fila && c.Columna == pos.columna);
            if (casilla == null)
            {
                casilla = new CasillaTablero
                {
                    PartidaId = dto.PartidaId,
                    UsuarioId = usuarioId,
                    Fila = pos.fila,
                    Columna = pos.columna,
                    TieneBarco = true,
                    Impactada = false
                };
                await _battleshipRepository.AddCasillaAsync(casilla);
            }
            else
            {
                casilla.TieneBarco = true;
                await _battleshipRepository.UpdateCasillaAsync(casilla);
            }
        }

        // Check if all ships are positioned (5 ships total = 2+3+3+4+5 = 17 cells)
        casillasPropias = await _battleshipRepository.GetCasillasByPartidaAndUsuarioAsync(dto.PartidaId, usuarioId);
        int celdasConBarco = casillasPropias.Count(c => c.TieneBarco);

        // Optional: transition phase if both players have positioned
        var casillasOponente = await _battleshipRepository.GetCasillasByPartidaAndUsuarioAsync(dto.PartidaId, partida.Jugador1Id == usuarioId ? partida.Jugador2Id : partida.Jugador1Id);
        if (celdasConBarco == 17 && casillasOponente.Count(c => c.TieneBarco) == 17)
        {
            partida.EstadoPartida = "EnProgreso";
            partida.UltimoMovimiento = DateTime.UtcNow;
            await _battleshipRepository.UpdateAsync(partida);
        }
        else
        {
            partida.UltimoMovimiento = DateTime.UtcNow;
            await _battleshipRepository.UpdateAsync(partida);
        }

        return new Response<bool>(true);
    }

    public async Task<Response<bool>> AtacarAsync(int usuarioId, AtacarDTO dto)
    {
        await CheckAbandonoAutomáticoAsync(usuarioId);

        var partida = await _battleshipRepository.GetPartidaByIdWithIncludesAsync(dto.PartidaId);
        if (partida == null || partida.EstadoPartida != "EnProgreso")
            return new Response<bool>("La partida no está en progreso.");

        if (partida.Jugador1Id != usuarioId && partida.Jugador2Id != usuarioId)
            return new Response<bool>("No posee permisos para realizar esta acción.");

        if (partida.TurnoUsuarioId != usuarioId)
            return new Response<bool>("No posee permisos para realizar esta acción.");

        int oponenteId = partida.Jugador1Id == usuarioId ? partida.Jugador2Id : partida.Jugador1Id;

        // Check if attack is out of bounds
        if (dto.Fila < 1 || dto.Fila > 12 || dto.Columna < 1 || dto.Columna > 12)
            return new Response<bool>("Coordenada fuera de límites.");

        var casillaOponente = await _battleshipRepository.GetCasillaAsync(dto.PartidaId, oponenteId, dto.Fila, dto.Columna);

        if (casillaOponente != null && casillaOponente.Impactada)
        {
            return new Response<bool>("Esta casilla ya fue atacada.");
        }

        if (casillaOponente == null)
        {
            casillaOponente = new CasillaTablero
            {
                PartidaId = dto.PartidaId,
                UsuarioId = oponenteId,
                Fila = dto.Fila,
                Columna = dto.Columna,
                TieneBarco = false,
                Impactada = true
            };
            await _battleshipRepository.AddCasillaAsync(casillaOponente);
        }
        else
        {
            casillaOponente.Impactada = true;
            await _battleshipRepository.UpdateCasillaAsync(casillaOponente);
        }

        // Check if all ships of opponent are sunk
        var todasCasillasOponente = await _battleshipRepository.GetCasillasByPartidaAndUsuarioAsync(dto.PartidaId, oponenteId);
        if (todasCasillasOponente.Where(c => c.TieneBarco).All(c => c.Impactada))
        {
            partida.EstadoPartida = "Finalizada";
            partida.GanadorId = usuarioId;
        }
        else
        {
            // Switch turn
            partida.TurnoUsuarioId = oponenteId;
        }

        partida.UltimoMovimiento = DateTime.UtcNow;
        await _battleshipRepository.UpdateAsync(partida);

        return new Response<bool>(true);
    }

    public async Task<Response<List<CasillaTableroDTO>>> GetTableroPropioAsync(int partidaId, int usuarioId)
    {
        await CheckAbandonoAutomáticoAsync(usuarioId);

        var partida = await _battleshipRepository.GetByIdAsync(partidaId);
        if (partida == null) return new Response<List<CasillaTableroDTO>>("Partida no encontrada.");
        if (partida.Jugador1Id != usuarioId && partida.Jugador2Id != usuarioId)
            return new Response<List<CasillaTableroDTO>>("No posee permisos para realizar esta acción.");

        var casillas = await _battleshipRepository.GetCasillasByPartidaAndUsuarioAsync(partidaId, usuarioId);
        var dtos = casillas.Select(c => new CasillaTableroDTO { Fila = c.Fila, Columna = c.Columna, TieneBarco = c.TieneBarco, Impactada = c.Impactada }).ToList();
        return new Response<List<CasillaTableroDTO>>(dtos);
    }

    public async Task<Response<List<CasillaTableroDTO>>> GetTableroAtaqueAsync(int partidaId, int usuarioId)
    {
        await CheckAbandonoAutomáticoAsync(usuarioId);

        var partida = await _battleshipRepository.GetByIdAsync(partidaId);
        if (partida == null) return new Response<List<CasillaTableroDTO>>("Partida no encontrada.");
        if (partida.Jugador1Id != usuarioId && partida.Jugador2Id != usuarioId)
            return new Response<List<CasillaTableroDTO>>("No posee permisos para realizar esta acción.");

        int oponenteId = partida.Jugador1Id == usuarioId ? partida.Jugador2Id : partida.Jugador1Id;
        var casillasOponente = await _battleshipRepository.GetCasillasByPartidaAndUsuarioAsync(partidaId, oponenteId);
        
        // Solo las impactadas, para que el usuario no vea los barcos no impactados
        var dtos = casillasOponente.Where(c => c.Impactada).Select(c => new CasillaTableroDTO { Fila = c.Fila, Columna = c.Columna, TieneBarco = c.TieneBarco, Impactada = c.Impactada }).ToList();
        return new Response<List<CasillaTableroDTO>>(dtos);
    }

    public async Task<Response<List<int>>> GetBarcosFaltantesAsync(int partidaId, int usuarioId)
    {
        await CheckAbandonoAutomáticoAsync(usuarioId);

        var partida = await _battleshipRepository.GetByIdAsync(partidaId);
        if (partida == null) return new Response<List<int>>("Partida no encontrada.");
        if (partida.Jugador1Id != usuarioId && partida.Jugador2Id != usuarioId)
            return new Response<List<int>>("No posee permisos para realizar esta acción.");

        var casillas = await _battleshipRepository.GetCasillasByPartidaAndUsuarioAsync(partidaId, usuarioId);
        var tieneBarco = casillas.Where(c => c.TieneBarco).ToList();
        
        bool[,] grid = new bool[12, 12];
        foreach (var c in tieneBarco)
        {
            if (c.Fila >= 1 && c.Fila <= 12 && c.Columna >= 1 && c.Columna <= 12)
                grid[c.Fila - 1, c.Columna - 1] = true;
        }

        List<int> componentes = new List<int>();
        bool[,] visitado = new bool[12, 12];

        int Dfs(int f, int c)
        {
            if (f < 0 || f >= 12 || c < 0 || c >= 12) return 0;
            if (!grid[f, c] || visitado[f, c]) return 0;
            visitado[f, c] = true;
            return 1 + Dfs(f + 1, c) + Dfs(f - 1, c) + Dfs(f, c + 1) + Dfs(f, c - 1);
        }

        for (int i = 0; i < 12; i++)
        {
            for (int j = 0; j < 12; j++)
            {
                if (grid[i, j] && !visitado[i, j])
                {
                    componentes.Add(Dfs(i, j));
                }
            }
        }

        List<int> todosLosBarcos = new List<int> { 5, 4, 3, 3, 2 };
        
        foreach (var comp in componentes.OrderByDescending(x => x))
        {
            int size = comp;
            if (todosLosBarcos.Contains(size))
            {
                todosLosBarcos.Remove(size);
            }
            else
            {
                while(size > 0 && todosLosBarcos.Count > 0)
                {
                    var match = todosLosBarcos.OrderByDescending(x => x).FirstOrDefault(x => x <= size);
                    if (match == 0) break;
                    todosLosBarcos.Remove(match);
                    size -= match;
                }
            }
        }

        return new Response<List<int>>(todosLosBarcos.OrderByDescending(x => x).ToList());
    }

    public async Task<Response<PartidaBattleshipDTO>> GetPartidaAsync(int partidaId, int usuarioId)
    {
        await CheckAbandonoAutomáticoAsync(usuarioId);
        
        var partida = await _battleshipRepository.GetPartidaByIdWithIncludesAsync(partidaId);
        if (partida == null) return new Response<PartidaBattleshipDTO>("Partida no encontrada.");
        if (partida.Jugador1Id != usuarioId && partida.Jugador2Id != usuarioId)
            return new Response<PartidaBattleshipDTO>("No posee permisos para realizar esta acción.");

        return new Response<PartidaBattleshipDTO>(MapToDTO(partida));
    }

    private PartidaBattleshipDTO MapToDTO(PartidaBattleship partida)
    {
        return new PartidaBattleshipDTO
        {
            Id = partida.Id,
            Jugador1Id = partida.Jugador1Id,
            Jugador1Nombre = partida.Jugador1?.UserName ?? "",
            Jugador2Id = partida.Jugador2Id,
            Jugador2Nombre = partida.Jugador2?.UserName ?? "",
            TurnoUsuarioId = partida.TurnoUsuarioId,
            EstadoPartida = partida.EstadoPartida,
            GanadorId = partida.GanadorId,
            GanadorNombre = partida.Ganador?.UserName,
            FechaInicio = partida.FechaInicio,
            UltimoMovimiento = partida.UltimoMovimiento,
            DuracionHoras = (partida.EstadoPartida == "Finalizada" ? (partida.UltimoMovimiento - partida.FechaInicio) : (DateTime.UtcNow - partida.FechaInicio)).TotalHours
        };
    }

    private async Task CheckAbandonoAutomáticoAsync(int usuarioId)
    {
        var activas = await _battleshipRepository.GetPartidasActivasByUsuarioIdAsync(usuarioId);
        foreach (var p in activas)
        {
            if (p.EstadoPartida == "EnProgreso")
            {
                if ((DateTime.UtcNow - p.UltimoMovimiento).TotalHours >= 48)
                {
                    // El que tenía el turno perdió
                    p.EstadoPartida = "Finalizada";
                    p.GanadorId = p.TurnoUsuarioId == p.Jugador1Id ? p.Jugador2Id : p.Jugador1Id;
                    p.UltimoMovimiento = DateTime.UtcNow;
                    await _battleshipRepository.UpdateAsync(p);
                }
            }
        }
    }
}
