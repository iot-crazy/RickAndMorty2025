using AutoMapper;
using RickAndMorty.DB.Models;
using RickAndMorty.DTO.Location;

namespace RickAndMorty.Services;

public class LocationProfile : Profile
{
    public LocationProfile()
    {
        CreateMap<NewLocationDto, Location>();
        CreateMap<Location, LocationDto>();
    }
}
