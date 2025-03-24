using RickAndMorty.DTO;

namespace RickAndMorty.Contracts;

public interface IEpisodeService
{
    Task AddAsync(EpisodeDto dto);
    Task<int> CountAsync();
    Task DeleteAsync(int id);
    Task<EpisodeDto?> Get(int id);
    Task<IReadOnlyList<EpisodeDto>> Get(string name);
    Task<IReadOnlyList<EpisodeDto>> GetAsync();
    Task<int> GetAllFromApiAsync();
}