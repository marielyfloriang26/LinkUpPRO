using LinkUpPro.Application.Interfaces.Repositories;
using LinkUpPro.Application.Interfaces.Services;
using LinkUpPro.Application.Services;
using LinkUpPro.Infrastructure.Persistence;
using LinkUpPro.Infrastructure.Persistence.Repositories;
using AutoMapper;
using LinkUpPro.Infrastructure.Shared.Services;

namespace LinkUpPro.Presentation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Agregar servicios al contenedor
            builder.Services.AddControllersWithViews(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            });
            
            // 2. Registrar la infraestructura (que incluye Identity configurado)
            builder.Services.AddPersistenceInfrastructure(builder.Configuration);

            builder.Services.AddTransient<IPublicacionRepository, PublicacionRepository>();
            builder.Services.AddTransient<IPublicacionService, PublicacionService>();
            builder.Services.AddTransient<IFileService, FileService>();

            builder.Services.AddAutoMapper(cfg => 
            {
                cfg.AddProfile<LinkUpPro.Application.Mappings.PublicacionMapping>();
            });
            builder.Services.AddSharedInfrastructure();

            var app = builder.Build();

            // 3. Configurar el pipeline de peticiones HTTP
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles(); 
            
            app.UseRouting();

            // ¡IMPORTANTE! El orden aquí es crítico:
            // Primero se identifica quién es el usuario (Authentication)
            // Luego se verifica si tiene permiso para lo que intenta hacer (Authorization)
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            
            // 4. Ruta por defecto: Redirigimos al Login al iniciar
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}