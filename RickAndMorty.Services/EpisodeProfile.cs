using AutoMapper;
using RickAndMorty.DB.Models;
using RickAndMorty.DTO.Episode;
using System.Globalization;

namespace RickAndMorty.Services;

public sealed class EpisodeProfile : Profile
{
    public EpisodeProfile()
    {
        CreateMap<NewApiEpisodeDto, Episode>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
        .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
        .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url))
        .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))

        .ForMember(dest => dest.AirDate,
            opt => opt.MapFrom(src => DateTime.ParseExact(src.AirDate, "MMMM d, yyyy", CultureInfo.InvariantCulture)));


        CreateMap<Episode, EpisodeDto>();
    }
}
