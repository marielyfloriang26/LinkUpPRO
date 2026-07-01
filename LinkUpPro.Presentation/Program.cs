using LinkUpPro.Application;
using LinkUpPro.Application.Interfaces.Repositories;
using LinkUpPro.Application.Interfaces.Services;
using LinkUpPro.Application.Services;
using LinkUpPro.Infrastructure.Persistence;
using LinkUpPro.Infrastructure.Persistence.Repositories;
using AutoMapper;
using LinkUpPro.Infrastructure.Shared.Services;
using LinkUpPro.Infrastructure.Persistence.Contexts;
using LinkUpPro.Domain.Entities;
using LinkUpPro.Application.Interfaces.Services.Interfaces;
using LinkUpPro.Infrastructure.Persistence.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LinkUpPro.Application.Interface.Services;
using LinkUpPro.Application.Interfaces.Shared;

var builder = WebApplication.CreateBuilder(args);

// Agrega servicios al contenedor
builder.Services.AddControllersWithViews(options =>
{
    // options.Filters.Add<GlobalExceptionFilter>();
});

// Registrar la infraestructura (que incluye Identity configurado)
builder.Services.AddPersistenceInfrastructure(builder.Configuration);

// Complementary transient services from incoming branch
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<LinkUpPro.Application.Interfaces.Shared.IEmailService, EmailService>();
builder.Services.AddTransient<IUploadFileService, UploadFileService>();

builder.Services.AddTransient<IPublicacionRepository, PublicacionRepository>();
builder.Services.AddTransient<IPublicacionService, PublicacionService>();
builder.Services.AddTransient<IFileService, FileService>();
builder.Services.AddTransient<IComentarioService, ComentarioService>();
builder.Services.AddTransient<IReaccionService, ReaccionService>();

builder.Services.AddAutoMapper(cfg => 
{
    cfg.AddProfile<LinkUpPro.Application.Mappings.PublicacionMapping>();
});
builder.Services.AddApplicationLayer();
builder.Services.AddSharedInfrastructure();

var app = builder.Build();

// Configura el pipeline de peticiones HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 

app.UseRouting();

// Primero se identifica quién es el usuario (Authentication)
// Luego se verifica si tiene permiso para lo que intenta hacer (Authorization)
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Ruta por defecto: Redirige al Login al iniciar
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();
