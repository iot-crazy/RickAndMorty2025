using RickAndMorty.DTO;

namespace RickAndMorty.Contracts;

public interface IRickAndMortyApiService
{
    Task<ApiResponse<T>> GetAsync<T>(string url);
}