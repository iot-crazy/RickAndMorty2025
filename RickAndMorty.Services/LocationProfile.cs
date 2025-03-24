using AutoMapper;
using RickAndMorty.DB.Models;
using RickAndMorty.DTO;

namespace RickAndMorty.Services;

public class LocationProfile : Profile
{
    public LocationProfile()
    {
        CreateMap<Location, LocationDto>().ReverseMap();
    }
}
