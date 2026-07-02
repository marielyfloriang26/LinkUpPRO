using LinkUpPro.Application.DTOs.MiPerfilDTO;
using LinkUpPro.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using LinkUpPro.Domain.Entities;
using System.Threading.Tasks;
using System.Linq;

namespace LinkUpPro.Application.Services;

public class PerfilService : IPerfilService
{
    private readonly UserManager<Usuario> _userManager;

    public PerfilService(UserManager<Usuario> userManager)
    {
        _userManager = userManager;
    }

    public async Task<ModificarPerfilDto?> ObtenerPorIdAsync(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return null;

        return new ModificarPerfilDto
        {
            Id = user.Id,
            Nombre = user.Nombre,
            Apellido = user.Apellido,
            Telefono = user.PhoneNumber ?? string.Empty,
            FotoPerfilUrl = user.FotoPerfilUrl
        };
    }

    public async Task<(bool Succeeded, string[] Errors, bool PasswordChanged)> ActualizarPerfilAsync(ModificarPerfilDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.Id.ToString());
        if (user == null)
        {
            return (false, new[] { "El usuario no existe." }, false);
        }

        bool intentandoCambiarClave = !string.IsNullOrEmpty(dto.PasswordActual) || !string.IsNullOrEmpty(dto.PasswordNuevo) || !string.IsNullOrEmpty(dto.PasswordConfirmacion);

        // Valida y procesa cambio de contraseña
        if (intentandoCambiarClave)
        {
            if (string.IsNullOrEmpty(dto.PasswordActual) || string.IsNullOrEmpty(dto.PasswordNuevo) || string.IsNullOrEmpty(dto.PasswordConfirmacion))
            {
                return (false, new[] { "Para cambiar su contraseña debe completar la contraseña actual, la nueva contraseña y su confirmación." }, false);
            }

            var checkPassword = await _userManager.CheckPasswordAsync(user, dto.PasswordActual);
            if (!checkPassword)
            {
                return (false, new[] { "La contraseña actual es incorrecta." }, false);
            }

            if (dto.PasswordNuevo == dto.PasswordActual)
            {
                return (false, new[] { "La nueva contraseña debe ser diferente de la contraseña actual." }, false);
            }

            if (dto.PasswordNuevo != dto.PasswordConfirmacion)
            {
                return (false, new[] { "La nueva contraseña y su confirmación no coinciden." }, false);
            }

            // Cambiar contra
            var resultClave = await _userManager.ChangePasswordAsync(user, dto.PasswordActual, dto.PasswordNuevo);
            if (!resultClave.Succeeded)
            {
                return (false, resultClave.Errors.Select(e => e.Description).ToArray(), false);
            }
        }

        // Actualizar datos personales
        user.Nombre = dto.Nombre;
        user.Apellido = dto.Apellido;
        user.PhoneNumber = dto.Telefono;
        if (dto.FotoPerfilUrl != null)
        {
            user.FotoPerfilUrl = dto.FotoPerfilUrl;
        }

        var resultUpdate = await _userManager.UpdateAsync(user);
        if (!resultUpdate.Succeeded)
        {
            return (false, resultUpdate.Errors.Select(e => e.Description).ToArray(), false);
        }

        if (intentandoCambiarClave)
        {
            // Invalida sesiones anteriores al actualizar la firma de seguridad
            await _userManager.UpdateSecurityStampAsync(user);
            return (true, System.Array.Empty<string>(), true);
        }

        return (true, System.Array.Empty<string>(), false);
    }
}