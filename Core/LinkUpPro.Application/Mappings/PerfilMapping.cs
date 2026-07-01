using AutoMapper;
using LinkUpPro.Application.DTOs.MiPerfilDTO;
using LinkUpPro.Application.ViewModels.MiPerfil;
using LinkUpPro.Domain.Entities;

namespace LinkUpPro.Application.Mappings;

public class PerfilMapping : Profile
{
    public PerfilMapping()
    {
        // Mapeo de la entidad de base de datos al vm para rellenar los datos del formulario
        CreateMap<Usuario, PerfilViewModel>()
            .ForMember(dest => dest.NombreUsuario, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Correo, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Telefono, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.FotoPerfil, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordActual, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordNuevo, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordConfirmacion, opt => opt.Ignore());

        // Mapeo de PerfilViewModel a ModificarPerfilDto
        CreateMap<PerfilViewModel, ModificarPerfilDto>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        // Mapeo de ModificarPerfilDto de vuelta a la entidad Usuario
        CreateMap<ModificarPerfilDto, Usuario>()
            .ForMember(dest => dest.UserName, opt => opt.Ignore())
            .ForMember(dest => dest.Email, opt => opt.Ignore())
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
            .ForMember(dest => dest.FotoPerfilUrl, opt => opt.Condition(src => src.FotoPerfilUrl != null));
    }
}