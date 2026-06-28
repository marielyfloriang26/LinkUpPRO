using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LinkUpPro.Application.ViewModels.User;

public class RegisterViewModel
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [RegularExpression(@"^(?!^\s+$).+$", ErrorMessage = "El nombre no debe contener únicamente espacios.")]
    [StringLength(50)]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "El apellido es requerido")]
    [RegularExpression(@"^(?!^\s+$).+$", ErrorMessage = "El apellido no debe contener únicamente espacios.")]
    [StringLength(50)]
    public string Apellido { get; set; } = null!;

    [Required(ErrorMessage = "El teléfono es requerido")]
    [RegularExpression(@"^(809|829|849)-\d{3}-\d{4}$", ErrorMessage = "El formato debe ser 809-XXX-XXXX")]
    public string Telefono { get; set; } = null!;

    [Required(ErrorMessage = "El correo es requerido")]
    [EmailAddress(ErrorMessage = "Correo electrónico inválido")]
    public string Correo { get; set; } = null!;

    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [StringLength(30, MinimumLength = 3)]
    public string NombreUsuario { get; set; } = null!;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Debe confirmar la contraseña")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "La contraseña y su confirmación no coinciden.")]
    public string ConfirmPassword { get; set; } = null!;

    [Required(ErrorMessage = "Debe seleccionar una foto de perfil")]
    [FileExtensions(Extensions = "jpg,jpeg,png,gif", ErrorMessage = "Por favor, seleccione una imagen válida (jpg, png, gif).")]
    public IFormFile FotoPerfil { get; set; } = null!;
}