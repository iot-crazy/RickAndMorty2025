using RickAndMorty.DTO.Location;

namespace RickAndMorty.Contracts;

public interface ILocationDataProvider
{
    Task<IReadOnlyList<LocationDto>> GetAsync();
    Task<LocationDto?> GetAsync(int id);
}
