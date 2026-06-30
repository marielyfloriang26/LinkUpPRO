using AutoMapper;
using System.Linq;
using LinkUpPro.Application.DTOs.Publicacion;
using LinkUpPro.Application.ViewModels.Publicacion;
using LinkUpPro.Domain.Entities;
using LinkUpPro.Application.DTOs.PublicacionDTO;
using LinkUpPro.Application.DTOs.ComentarioDTO;

namespace LinkUpPro.Application.Mappings;

public class PublicacionMapping : Profile
{
        public PublicacionMapping()
    {
        CreateMap<GuardarPublicacionViewModel, CrearPublicacionDto>()
            .ForMember(dest => dest.ImagenUrl, opt => opt.Ignore()) 
            .ForMember(dest => dest.UsuarioId, opt => opt.Ignore());

        CreateMap<GuardarPublicacionViewModel, ModificarPublicacionDto>();

        // Mapeo de Publicacion a DTO
        CreateMap<Publicacion, PublicacionDto>()
            .ForMember(dest => dest.AutorNombreCompleto, opt => opt.MapFrom(src => $"{src.Usuario.Nombre} {src.Usuario.Apellido}"))
            .ForMember(dest => dest.AutorFotoPerfilUrl, opt => opt.MapFrom(src => src.Usuario.FotoPerfilUrl))
            .ForMember(dest => dest.CantidadMeGusta, opt => opt.MapFrom(src => src.Reacciones.Count(r => r.TipoReaccion == "Like")))
            .ForMember(dest => dest.CantidadNoMeGusta, opt => opt.MapFrom(src => src.Reacciones.Count(r => r.TipoReaccion == "Dislike")))
            .ForMember(dest => dest.Comentarios, opt => opt.MapFrom(src => src.Comentarios))
            .ForMember(dest => dest.ReaccionUsuarioAutenticado, opt => opt.MapFrom(src => src.Reacciones.FirstOrDefault(r => r.UsuarioId == 1).TipoReaccion));

        CreateMap<PublicacionDto, PublicacionViewModel>().ReverseMap();
        CreateMap<PublicacionDto, GuardarPublicacionViewModel>().ReverseMap();

        // Mapeo de Comentarios
        CreateMap<CrearComentarioDto, Comentario>();
        CreateMap<Comentario, ComentarioDto>()
            .ForMember(dest => dest.AutorNombre, opt => opt.MapFrom(src => $"{src.Usuario.Nombre} {src.Usuario.Apellido}"))
            .ForMember(dest => dest.AutorFotoPerfil, opt => opt.MapFrom(src => src.Usuario.FotoPerfilUrl));
        CreateMap<ComentarioDto, ComentarioViewModel>().ReverseMap();

        CreateMap<Publicacion, CrearPublicacionDto>().ReverseMap();
        CreateMap<Publicacion, ModificarPublicacionDto>().ReverseMap();
    }
}