using RickAndMorty.DTO.Character;

namespace RickAndMorty.Contracts;

public interface ICharacterDataProvider
{
    Task<IReadOnlyList<CharacterDto>> GetAsync();
    Task<CharacterDto?> GetAsync(int id);
}
