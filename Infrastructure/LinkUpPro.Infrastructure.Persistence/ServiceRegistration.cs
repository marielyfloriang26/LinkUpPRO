using LinkUpPro.Application.Interfaces.Repositories;
using LinkUpPro.Domain.Entities;
using LinkUpPro.Infrastructure.Persistence.Contexts;
using LinkUpPro.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity; // Para AddIdentity
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using LinkUpPro.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;



namespace LinkUpPro.Infrastructure.Persistence;

public static class ServiceRegistration
{
    public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddIdentity<Usuario, IdentityRole<int>>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true; // Obligatorio para activar cuenta
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;

            
            /*options.SignIn.RequireConfirmedAccount = true;
            options.Password.RequiredLength = 8;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;*/
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();


        services.AddScoped<IUserClaimsPrincipalFactory<Usuario>, AppUserClaimsPrincipalFactory>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("CuentaActiva", policy => policy.RequireClaim("EsActivo", "True"));
        });

        services.AddSingleton<IAuthorizationHandler, PropietarioHandler>();



        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Obligatorio HTTPS
            options.Cookie.SameSite = SameSiteMode.Strict;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Cierre por inactividad
            options.SlidingExpiration = true; // Renueva la sesión al interactuar
        });

        services.AddTransient(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));
        services.AddTransient<IAmistadRepository, AmistadRepository>();
        services.AddTransient<ISolicitudAmistadRepository, SolicitudAmistadRepository>();
    }
}

