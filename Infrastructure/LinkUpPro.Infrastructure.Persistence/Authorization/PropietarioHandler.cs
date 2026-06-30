using LinkUpPro.Application.Authorization;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using LinkUpPro.Application.Interfaces;

public class PropietarioHandler : AuthorizationHandler<PropietarioRequirement, IEntidadPropietaria>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
        PropietarioRequirement requirement, IEntidadPropietaria recurso)
    {
        var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        // Validamos que el ID del usuario logueado coincida con el ID del dueño del recurso
        if (userId != null && recurso.UsuarioId.ToString() == userId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}