using RickAndMorty.Contracts;
using RickAndMorty.DTO.Episode;
using System.Net.Http.Json;

namespace RickAndMorty.Services;

public sealed class ApiEpisodeDataProvider(HttpClient http) : IEpisodeDataProvider
{
    public async Task<IReadOnlyList<EpisodeDto>> GetAsync()
        => await http.GetFromJsonAsync<List<EpisodeDto>>("api/episode") ?? [];

    public async Task<EpisodeDto?> GetAsync(int id)
        => await http.GetFromJsonAsync<EpisodeDto>($"api/episode/{id}");
}
