using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LinkUpPro.Application.ViewModels.MiPerfil;

public class PerfilViewModel
{
    [Display(Name = "Nombre de Usuario")]
    public string? NombreUsuario { get; set; } 

    [Display(Name = "Correo Electrónico")]
    public string? Correo { get; set; }

    [Required(ErrorMessage = "Debe ingresar su nombre.")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "Debe ingresar su apellido.")]
    public string Apellido { get; set; } = null!;

    [Required(ErrorMessage = "Debe ingresar su teléfono.")]
    [RegularExpression(@"^(809|829|849)-\d{3}-\d{4}$", ErrorMessage = "Debe ingresar un número telefónico válido de República Dominicana.")]
    public string Telefono { get; set; } = null!;

    public string? FotoPerfilUrl { get; set; }

    [Display(Name = "Nueva Foto de Perfil")]
    public IFormFile? FotoPerfil { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Contraseña Actual")]
    public string? PasswordActual { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Nueva Contraseña")]
    public string? PasswordNuevo { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar Contraseña")]
    public string? PasswordConfirmacion { get; set; }
}