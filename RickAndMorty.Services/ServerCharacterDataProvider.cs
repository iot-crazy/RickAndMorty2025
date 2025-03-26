using RickAndMorty.Contracts;
using RickAndMorty.DTO.Character;

namespace RickAndMorty.Services;

public sealed class ServerCharacterDataProvider : ICharacterDataProvider
{
    private readonly ICharacterService _service;

    public ServerCharacterDataProvider(ICharacterService service)
    {
        _service = service;
    }

    public Task<IReadOnlyList<CharacterDto>> GetAsync(CharacterFilter? filter) => _service.GetAsync(filter);
    public Task<CharacterDto?> GetAsync(int id) => _service.GetAsync(id);

    public Task AddAsync(NewCharacterDto dto) => _service.AddAsync(dto);
}
