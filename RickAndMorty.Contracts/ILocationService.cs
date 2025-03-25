using RickAndMorty.DTO.Location;

namespace RickAndMorty.Contracts;

public interface ILocationService
{
    Task AddAsync(NewLocationDto dto);
    Task<int> CountAsync();
    Task DeleteAsync(int id);
    Task<LocationDto?> GetAsync(int id);
    Task<IReadOnlyList<LocationDto>> GetAsync(string name);
    Task<IReadOnlyList<LocationDto>> GetAsync();
    Task<int> GetAllFromApiAsync();
}