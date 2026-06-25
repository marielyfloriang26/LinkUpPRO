using System.ComponentModel.DataAnnotations;

namespace LinkUpPro.Application.Account.ViewModels;

public class ResetPasswordViewModel
{
    public string UserId { get; set; } = null!;
    public string Token { get; set; } = null!;

    [Required(ErrorMessage = "La nueva contraseña es requerida.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    [DataType(DataType.Password)]
    public string Contraseña { get; set; } = null!;

    [Required(ErrorMessage = "La confirmación es requerida.")]
    [DataType(DataType.Password)]
    [Compare("Contraseña", ErrorMessage = "La contraseña y su confirmación no coinciden.")]
    public string ConfirmarContraseña { get; set; } = null!;
}