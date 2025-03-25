using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RickAndMorty.Contracts;
using RickAndMorty.DB.Models;
using RickAndMorty.DTO.Character;

namespace RickAndMorty.Services;

public sealed class CharacterService(IRickAndMortyContextFactory contextFactory,
    IRickAndMortyApiService apiService,
    IMapper mapper) : ICharacterService
{
    public async Task<IReadOnlyList<CharacterDto>> GetAsync()
    {
        var context = await contextFactory.CreateContextAsync();
        return await context.Characters
            .AsNoTracking()
            .ProjectTo<CharacterDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<CharacterDto?> GetAsync(int id)
    {
        var context = await contextFactory.CreateContextAsync();
        return await context.Characters
            .AsNoTracking()
            .Where(x => x.Id == id)
            .ProjectTo<CharacterDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<CharacterDto>> GetAsync(string name)
    {
        var context = await contextFactory.CreateContextAsync();
        return await context.Characters
            .AsNoTracking()
            .Where(x => x.Name == name)
            .ProjectTo<CharacterDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task AddAsync(NewCharacterDto dto)
    {
        var context = await contextFactory.CreateContextAsync();
        var entity = mapper.Map<Character>(dto);
        context.Characters.Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var context = await contextFactory.CreateContextAsync();
        var entity = await context.Characters.FindAsync(id);
        if (entity is null) return;
        context.Characters.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<int> CountAsync()
    {
        var context = await contextFactory.CreateContextAsync();
        return await context.Characters.CountAsync();
    }

    public async Task<int> GetAllFromApiAsync()
    {
        var context = await contextFactory.CreateContextAsync();
        var characters = await apiService.FetchAllEpisodesAsync<NewCharacterDto>("api/character/?status=alive");
        var newCharacters = mapper.Map<IEnumerable<Character>>(characters);
        context.Characters.AddRange(newCharacters);
        await context.SaveChangesAsync();
        return await CountAsync();
    }
}
