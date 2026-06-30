namespace LinkUpPro.Application.DTOs;

public class UsuarioDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string NombreUsuario { get; set; } = null!;
    public string? FotoPerfilUrl { get; set; }
}
