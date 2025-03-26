using RickAndMorty.DTO.Episode;

namespace RickAndMorty.Contracts;

public interface IEpisodeDataProvider
{
    Task<IReadOnlyList<EpisodeDto>> GetAsync();
    Task<EpisodeDto?> GetAsync(int id);
}
