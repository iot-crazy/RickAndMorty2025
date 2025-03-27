using RickAndMorty.DTO.Character;

namespace RickAndMorty.Contracts;

public interface IApiCharacterDataProvider
{
    Task<IReadOnlyList<CharacterDto>> GetAsync(CharacterFilter? filter = null);
    Task<CharacterDto?> GetAsync(int id);
    Task AddAsync(NewCharacterDto dto);
}
