
using RickAndMorty.DTO;

namespace RickAndMorty.Contracts
{
    public interface ICharacterService
    {
        Task AddAsync(CharacterDto dto);
        Task DeleteAsync(int id);
        Task<CharacterDto?> Get(int id);
        Task<IReadOnlyList<CharacterDto>> Get(string name);
        Task<IReadOnlyList<CharacterDto>> GetAsync();
        Task<int> CountAsync();

        Task<int> GetAllFromApiAsync();
    }
}