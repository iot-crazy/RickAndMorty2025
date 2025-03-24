using RickAndMorty.DTO;

namespace RickAndMorty.Contracts;

public interface ILocationService
{
    Task AddAsync(LocationDto dto);
    Task<int> CountAsync();
    Task DeleteAsync(int id);
    Task<LocationDto?> Get(int id);
    Task<IReadOnlyList<LocationDto>> Get(string name);
    Task<IReadOnlyList<LocationDto>> GetAsync();
    Task<int> GetAllFromApiAsync();
}