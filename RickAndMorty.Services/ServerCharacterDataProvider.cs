using RickAndMorty.Contracts;
using RickAndMorty.DTO.Character;

namespace RickAndMorty.Services;

public class ServerCharacterDataProvider : ICharacterDataProvider
{
    private readonly ICharacterService _service;

    public ServerCharacterDataProvider(ICharacterService service)
    {
        _service = service;
    }

    public Task<IReadOnlyList<CharacterDto>> GetAsync() => _service.GetAsync();
    public Task<CharacterDto?> GetAsync(int id) => _service.GetAsync(id);
}
