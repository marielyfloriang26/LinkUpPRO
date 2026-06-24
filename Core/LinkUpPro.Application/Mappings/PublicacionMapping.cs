using AutoMapper;
using System.Linq;
using LinkUpPro.Application.DTOs.Publicacion;
using LinkUpPro.Application.ViewModels.Publicacion;
using LinkUpPro.Domain.Entities;
using LinkUpPro.Application.DTOs.PublicacionDTO;

namespace LinkUpPro.Application.Mappings;

public class PublicacionMapping : Profile
{
    public PublicacionMapping()
    {
        // mapeos entre entidades y dtos
        
        CreateMap<Publicacion, PublicacionDto>();

        
        CreateMap<CrearPublicacionDto, Publicacion>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.Comentarios, opt => opt.Ignore())
            .ForMember(dest => dest.Reacciones, opt => opt.Ignore());

        // ModificarPublicacionDto -> Entidad
        CreateMap<ModificarPublicacionDto, Publicacion>()
            .ForMember(dest => dest.UsuarioId, opt => opt.Ignore())
            .ForMember(dest => dest.FechaCreacion, opt => opt.Ignore())
            .ForMember(dest => dest.Usuario, opt => opt.Ignore())
            .ForMember(dest => dest.Comentarios, opt => opt.Ignore())
            .ForMember(dest => dest.Reacciones, opt => opt.Ignore());

        // mapeo entre dto y vm
        
        CreateMap<PublicacionDto, PublicacionViewModel>().ReverseMap();

        CreateMap<GuardarPublicacionViewModel, CrearPublicacionDto>();

        CreateMap<GuardarPublicacionViewModel, ModificarPublicacionDto>();
    }
}