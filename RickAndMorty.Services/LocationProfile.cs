using AutoMapper;
using RickAndMorty.DB.Models;
using RickAndMorty.DTO.Location;

namespace RickAndMorty.Services;

public sealed class LocationProfile : Profile
{
    public LocationProfile()
    {
        CreateMap<NewApiLocationDto, Location>();
        CreateMap<Location, LocationDto>();
    }
}
