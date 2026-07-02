namespace LinkUpPro.Application.DTOs.MiPerfilDTO;

public class ModificarPerfilDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Apellido { get; set; } = null!;
    public string Telefono { get; set; } = null!;
    public string? FotoPerfilUrl { get; set; }
    public string? PasswordActual { get; set; }
    public string? PasswordNuevo { get; set; }
    public string? PasswordConfirmacion { get; set; }
}