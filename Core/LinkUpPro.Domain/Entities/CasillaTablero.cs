namespace LinkUpPro.Domain.Entities;

public class CasillaTablero
{
    public int Id { get; set; }
    public int PartidaId { get; set; }
    public int UsuarioId { get; set; }
    public int Fila { get; set; }
    public int Columna { get; set; }
    public bool TieneBarco { get; set; }
    public bool Impactada { get; set; }

    public PartidaBattleship Partida { get; set; } = null!;
    public Usuario Usuario { get; set; } = null!;
}
