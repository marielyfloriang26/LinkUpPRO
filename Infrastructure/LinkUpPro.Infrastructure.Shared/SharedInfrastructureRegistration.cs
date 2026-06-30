// LinkUpPro.Infrastructure.Shared/ServiceRegistration.cs
using LinkUpPro.Application.Interfaces.Shared;
using LinkUpPro.Application.Interfaces.Services;
using LinkUpPro.Infrastructure.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

public static class SharedInfrastructureRegistration
{
    public static void AddSharedInfrastructure(this IServiceCollection services)
    {
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IFileService, FileService>();
    }
}