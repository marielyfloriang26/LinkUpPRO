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

        CreateMap<GuardarPublicacionViewModel, CrearPublicacionDto>()
            .ForMember(dest => dest.ImagenUrl, opt => opt.Ignore()) 
            .ForMember(dest => dest.UsuarioId, opt => opt.Ignore());

        CreateMap<GuardarPublicacionViewModel, ModificarPublicacionDto>();
        // Mapeos de entidades y dto
        CreateMap<Publicacion, PublicacionDto>().ReverseMap();
        CreateMap<Publicacion, CrearPublicacionDto>().ReverseMap();
        CreateMap<Publicacion, ModificarPublicacionDto>().ReverseMap();
        CreateMap<PublicacionDto, PublicacionViewModel>().ReverseMap();
    }
}