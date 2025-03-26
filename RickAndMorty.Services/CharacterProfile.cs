using AutoMapper;
using RickAndMorty.DB.Models;
using RickAndMorty.DTO.Character;
using System.Text.RegularExpressions;

namespace RickAndMorty.Services;

public sealed class CharacterProfile : Profile
{
    private const string pattern = @"/(\d+)$";

    public CharacterProfile()
    {
        CreateMap<NewApiCharacterDto, Character>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
        .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
        .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
        .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
        .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url))
        .ForMember(dest => dest.Created, opt => opt.MapFrom(src => src.Created))

        .ForMember(dest => dest.OriginId,
             opt => opt.MapFrom(src =>
                 string.IsNullOrEmpty(src.Origin.Url)
                     ? (int?)null
                     : int.Parse(Regex.Match(src.Origin.Url, pattern).Groups[1].Value)))

        .ForMember(dest => dest.LocationId,
            opt => opt.MapFrom(src =>
                string.IsNullOrEmpty(src.Location.Url)
                    ? (int?)null
                    : int.Parse(Regex.Match(src.Location.Url, pattern).Groups[1].Value)))

       .ForMember(dest => dest.Location, opt => opt.Ignore())
       .ForMember(dest => dest.Origin, opt => opt.Ignore())
       .ForMember(dest => dest.CharacterEpisodes, opt => opt.MapFrom(src =>
            src.Episode
                .Where(url => !string.IsNullOrEmpty(url))
                .Select(url => new CharacterEpisode
                {
                    CharacterId = src.Id,
                    EpisodeId = int.Parse(Regex.Match(url, pattern).Groups[1].Value)
                }).ToList()));

        CreateMap<Character, CharacterDto>()
            .ForMember(dest => dest.Episodes, opt => opt.MapFrom(src => src.CharacterEpisodes.Select(e => e.Episode)));

        CreateMap<NewCharacterDto, Character>()
             .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
        .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
        .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
        .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
        .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image))
        .ForMember(dest => dest.Url, opt => opt.MapFrom(src => src.Url))
        .ForMember(dest => dest.Created, opt => opt.MapFrom(_ => DateTime.UtcNow))
        .ForMember(dest => dest.OriginId, opt => opt.MapFrom(src => src.OriginLocationId))
        .ForMember(dest => dest.LocationId, opt => opt.MapFrom(src => src.LocationId))
        ;
    }
}
