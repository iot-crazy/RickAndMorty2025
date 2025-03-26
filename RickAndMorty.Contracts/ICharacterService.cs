using RickAndMorty.DTO.Character;

namespace RickAndMorty.Contracts
{
    public interface ICharacterService
    {
        Task AddAsync(NewApiCharacterDto dto);
        Task<CharacterDto?> AddAsync(NewCharacterDto dto);
        Task DeleteAsync(int id);
        Task<CharacterDto?> GetAsync(int id);
        //  Task<IReadOnlyList<CharacterDto>> GetAsync(string name);
        //Task<IReadOnlyList<CharacterDto>> GetAsync();
        Task<IReadOnlyList<CharacterDto>> GetAsync(CharacterFilter? filter = null);
        Task<int> CountAsync();

        Task<int> GetAllFromApiAsync();
    }
}