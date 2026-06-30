using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using LinkUpPro.Domain.Entities;

public class AppUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<Usuario, IdentityRole<int>>
{
    public AppUserClaimsPrincipalFactory(
        UserManager<Usuario> userManager,
        RoleManager<IdentityRole<int>> roleManager,
        IOptions<IdentityOptions> options) : base(userManager, roleManager, options) { }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(Usuario user)
    {
        var identity = await base.GenerateClaimsAsync(user);
        // Esto valida si el email está confirmado (tu criterio de cuenta activa)
        identity.AddClaim(new Claim("EsActivo", user.EmailConfirmed.ToString()));
        return identity;
    }
}