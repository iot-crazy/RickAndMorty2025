using RickAndMorty.Contracts;
using RickAndMorty.DTO.Character;
using System.Net.Http.Json;

namespace RickAndMorty.Services;

public sealed class ApiCharacterDataProvider(HttpClient http) : ICharacterDataProvider
{
    public async Task<IReadOnlyList<CharacterDto>> GetAsync(CharacterFilter? filter = null)
        => await http.GetFromJsonAsync<List<CharacterDto>>($"api/character{filter?.ToQueryString()}") ?? [];

    public async Task<CharacterDto?> GetAsync(int id)
        => await http.GetFromJsonAsync<CharacterDto>($"api/character/{id}");

    public async Task AddAsync(NewCharacterDto dto)
        => await http.PostAsJsonAsync<NewCharacterDto>("api/character", dto);
}
