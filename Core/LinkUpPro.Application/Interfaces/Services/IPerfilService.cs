using LinkUpPro.Application.DTOs.MiPerfilDTO;
using System.Threading.Tasks;

namespace LinkUpPro.Application.Interfaces.Services;

public interface IPerfilService
{
    Task<ModificarPerfilDto?> ObtenerPorIdAsync(int userId);
    Task<(bool Succeeded, string[] Errors, bool PasswordChanged)> ActualizarPerfilAsync(ModificarPerfilDto dto);
}