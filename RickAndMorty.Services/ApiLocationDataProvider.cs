using RickAndMorty.Contracts;
using RickAndMorty.DTO.Location;
using System.Net.Http.Json;

namespace RickAndMorty.Services;

public sealed class ApiLocationDataProvider(HttpClient http) : ILocationDataProvider
{
    public async Task<IReadOnlyList<LocationDto>> GetAsync()
        => await http.GetFromJsonAsync<List<LocationDto>>("api/location") ?? [];

    public async Task<LocationDto?> GetAsync(int id)
        => await http.GetFromJsonAsync<LocationDto>($"api/location/{id}");
}
