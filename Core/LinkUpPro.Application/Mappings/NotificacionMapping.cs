using AutoMapper;
using LinkUpPro.Application.DTOs.Notificacion;
using LinkUpPro.Application.ViewModels.Notificacion;
using LinkUpPro.Domain.Entities;

namespace LinkUpPro.Application.Mappings;

public class NotificacionMapping : Profile
{
    public NotificacionMapping()
    {
        CreateMap<Notificacion, NotificacionDto>()
            .ForMember(dest => dest.RemitenteNombreUsuario, opt => opt.MapFrom(src => src.Remitente != null ? src.Remitente.UserName : "Sistema"))
            .ForMember(dest => dest.RemitenteFotoPerfilUrl, opt => opt.MapFrom(src => src.Remitente != null ? src.Remitente.FotoPerfilUrl : string.Empty));

        CreateMap<NotificacionDto, NotificacionViewModel>().ReverseMap();
    }
}