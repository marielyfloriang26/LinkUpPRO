using System.ComponentModel.DataAnnotations;

namespace LinkUpPro.Presentation.ViewModels.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "El nombre de usuario es requerido.")]
    public string NombreUsuario { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es requerida.")]
    [DataType(DataType.Password)]
    public string Contraseña { get; set; } = null!;

    public bool MantenerSesionIniciada { get; set; }
}