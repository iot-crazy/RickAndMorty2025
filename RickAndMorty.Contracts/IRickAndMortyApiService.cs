namespace RickAndMorty.Contracts;

public interface IRickAndMortyApiService
{
    Task<IEnumerable<T>> FetchAllEpisodesAsync<T>(string url);
}