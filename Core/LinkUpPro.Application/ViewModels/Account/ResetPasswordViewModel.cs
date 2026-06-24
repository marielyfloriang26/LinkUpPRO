using System.ComponentModel.DataAnnotations;

namespace LinkUpPro.Presentation.ViewModels;

public class ResetPasswordViewModel
{
    public string UserId { get; set; } = null!;
    public string Token { get; set; } = null!;

    [Required(ErrorMessage = "La nueva contraseña es requerida.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Mínimo 8 caracteres.")]
    [DataType(DataType.Password)]
    public string Contraseña { get; set; } = null!;

    [Required(ErrorMessage = "La confirmación es requerida.")]
    [DataType(DataType.Password)]
    [Compare("Contraseña", ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmarContraseña { get; set; } = null!;
}