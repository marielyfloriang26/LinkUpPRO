using LinkUpPro.Application.Interfaces.Repositories;
using LinkUpPro.Domain.Entities;
using LinkUpPro.Infrastructure.Persistence.Contexts;
using LinkUpPro.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity; // Para AddIdentity
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;



namespace LinkUpPro.Infrastructure.Persistence;

public static class ServiceRegistration
{
    public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddTransient(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));

        services.AddIdentity<Usuario, IdentityRole<int>>(options =>
        {
            // Reglas de contraseña
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;

            // Bloqueo temporal por intentos fallidos (Requerimiento del documento)
            options.Lockout.AllowedForNewUsers = true;
            options.Lockout.MaxFailedAccessAttempts = 5; // Bloqueo tras 5 intentos
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // Duración del bloqueo
            
            // Requerimiento de correo único
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // 3. Registro de Repositorios
        services.AddTransient(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));
    }
}
