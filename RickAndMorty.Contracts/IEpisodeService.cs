using RickAndMorty.DTO.Episode;

namespace RickAndMorty.Contracts;

public interface IEpisodeService
{
    Task AddAsync(NewEpisodeDto dto);
    Task<int> CountAsync();
    Task DeleteAsync(int id);
    Task<EpisodeDto?> GetAsync(int id);
    Task<IReadOnlyList<EpisodeDto>> GetAsync(string name);
    Task<IReadOnlyList<EpisodeDto>> GetAsync();
    Task<int> GetAllFromApiAsync();
}