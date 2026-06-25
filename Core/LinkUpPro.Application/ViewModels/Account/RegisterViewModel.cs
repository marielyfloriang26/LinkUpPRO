using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LinkUpPro.Application.Account.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "El nombre es requerido.")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "El nombre no debe contener únicamente espacios.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es requerido.")]
    [RegularExpression(@"^(?!\s*$).+", ErrorMessage = "El apellido no debe contener únicamente espacios.")]
    public string Apellido { get; set; } = null!;

    [Required(ErrorMessage = "El teléfono es requerido.")]
    [RegularExpression(@"^(809|829|849)-\d{3}-\d{4}$", ErrorMessage = "El teléfono debe tener un formato válido de República Dominicana (Ej: 809-555-1234).")]
    public string Telefono { get; set; } = null!;

    [Required(ErrorMessage = "El correo electrónico es requerido.")]
    [EmailAddress(ErrorMessage = "El correo electrónico debe tener un formato válido.")]
    public string Correo { get; set; } = null!;

    [Required(ErrorMessage = "La foto de perfil es requerida.")]
    public IFormFile? FotoPerfil { get; set; }

    [Required(ErrorMessage = "El nombre de usuario es requerido.")]
    public string NombreUsuario { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es requerida.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
    [DataType(DataType.Password)]
    public string Contraseña { get; set; } = null!;

    [Required(ErrorMessage = "La confirmación de la contraseña es requerida.")]
    [DataType(DataType.Password)]
    [Compare("Contraseña", ErrorMessage = "La contraseña y su confirmación no coinciden.")]
    public string ConfirmarContraseña { get; set; } = null!;
}
