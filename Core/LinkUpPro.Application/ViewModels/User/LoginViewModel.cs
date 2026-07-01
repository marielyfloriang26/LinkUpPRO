using System.ComponentModel.DataAnnotations;

namespace LinkUpPro.Application.ViewModels.User;

public class LoginViewModel
{
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    public string NombreUsuario { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Display(Name = "Mantener sesión iniciada")]
    public bool MantenerSesion { get; set; }
}