using RickAndMorty.DTO.Character;

namespace RickAndMorty.Contracts
{
    public interface ICharacterService
    {
        Task AddAsync(NewCharacterDto dto);
        Task DeleteAsync(int id);
        Task<CharacterDto?> GetAsync(int id);
        Task<IReadOnlyList<CharacterDto>> GetAsync(string name);
        Task<IReadOnlyList<CharacterDto>> GetAsync();
        Task<int> CountAsync();

        Task<int> GetAllFromApiAsync();
    }
}