using System.ComponentModel.DataAnnotations;

namespace LinkUpPro.Application.ViewModels.User;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    public string NombreUsuario { get; set; } = null!;
}