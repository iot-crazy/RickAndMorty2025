using RickAndMorty.Contracts;
using RickAndMorty.DTO.Character;
using System.Net.Http.Json;

namespace RickAndMorty.Web.Client;

public class ApiCharacterDataProvider : ICharacterDataProvider
{
    private readonly HttpClient _http;

    public ApiCharacterDataProvider(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<CharacterDto>> GetAsync()
        => await _http.GetFromJsonAsync<List<CharacterDto>>("api/character") ?? [];

    public async Task<CharacterDto?> GetAsync(int id)
        => await _http.GetFromJsonAsync<CharacterDto>($"api/character/{id}");
}
