using System.ComponentModel.DataAnnotations;

namespace LinkUpPro.Application.ViewModels.User;
public class ResetPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmPassword { get; set; } = null!;

    [Required]
    public string Token { get; set; } = null!;
}