using LinkUpPro.Application.Interfaces.Services;
using LinkUpPro.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LinkUpPro.Application;

public static class ServiceRegistration
{
    public static void AddApplicationLayer(this IServiceCollection services)
    {
        services.AddTransient<IAmigoService, AmigoService>();
        services.AddTransient<ISolicitudAmistadService, SolicitudAmistadService>();
        services.AddTransient<IBattleshipService, BattleshipService>();
    }
}
