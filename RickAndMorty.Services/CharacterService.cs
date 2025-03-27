using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RickAndMorty.Contracts;
using RickAndMorty.DB.Models;
using RickAndMorty.DTO.Character;

namespace RickAndMorty.Services;

public sealed class CharacterService(IRickAndMortyContextFactory contextFactory,
    IRickAndMortyApiService apiService,
    IMapper mapper,
    ICacheInvalidator cacheInvalidator) : ICharacterService
{
    public async Task<CharacterDto?> GetAsync(int id)
    {
        var context = await contextFactory.CreateContextAsync();
        return await context.Characters
             .Include(x => x.Location)
            .Include(x => x.Origin)
            .Include(x => x.CharacterEpisodes).ThenInclude(e => e.Episode)
            .AsNoTracking()
            .Where(x => x.Id == id)
            .ProjectTo<CharacterDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<CharacterDto>> GetAsync(CharacterFilter? filter = null)
    {
        var context = await contextFactory.CreateContextAsync();
        var query = context.Characters
            .Include(x => x.Location)
            .Include(x => x.Origin)
            .Include(x => x.CharacterEpisodes).ThenInclude(e => e.Episode)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter?.Name))
            query = query.Where(c => EF.Functions.Like(c.Name, filter.Name));

        if (!string.IsNullOrWhiteSpace(filter?.Planet))
            query = query.Where(c => (c.Location != null && EF.Functions.Like(c.Location.Name, filter.Planet))
            ||
            (c.Origin != null && EF.Functions.Like(c.Origin.Name, filter.Planet))
            );

        if (!string.IsNullOrWhiteSpace(filter?.Status))
            query = query.Where(c => EF.Functions.Like(c.Status, filter.Status));

        if (!string.IsNullOrWhiteSpace(filter?.Gender))
            query = query.Where(c => EF.Functions.Like(c.Gender, filter.Gender));

        return await query.ProjectTo<CharacterDto>(mapper.ConfigurationProvider).ToListAsync();
    }


    public async Task AddAsync(NewApiCharacterDto dto)
    {
        var context = await contextFactory.CreateContextAsync();
        var entity = mapper.Map<Character>(dto);
        context.Characters.Add(entity);
        await context.SaveChangesAsync();
        await InvalidateCache();
    }

    public async Task<CharacterDto?> AddAsync(NewCharacterDto dto)
    {
        var context = await contextFactory.CreateContextAsync();
        var entity = mapper.Map<Character>(dto);
        entity.Id = (await context.Characters.MaxAsync(x => x.Id)) + 1;
        context.Characters.Add(entity);

        foreach (var episodeId in dto.Episodes)
        {
            entity.CharacterEpisodes.Add(new() { CharacterId = entity.Id, EpisodeId = episodeId });
        }

        await context.SaveChangesAsync();
        await InvalidateCache();
        return await GetAsync(entity.Id);
    }

    public async Task DeleteAsync(int id)
    {
        var context = await contextFactory.CreateContextAsync();
        var entity = await context.Characters.FindAsync(id);
        if (entity is null) return;
        context.Characters.Remove(entity);
        await context.SaveChangesAsync();
        await InvalidateCache();
    }

    public async Task<int> CountAsync()
    {
        var context = await contextFactory.CreateContextAsync();
        return await context.Characters.CountAsync();
    }

    public async Task<int> GetAllFromApiAsync()
    {
        var context = await contextFactory.CreateContextAsync();
        var characters = await apiService.FetchAllEpisodesAsync<NewApiCharacterDto>("api/character/?status=alive");
        var newCharacters = mapper.Map<IEnumerable<Character>>(characters);
        context.Characters.AddRange(newCharacters);
        await context.SaveChangesAsync();
        await InvalidateCache();
        return await CountAsync();
    }

    private async Task InvalidateCache()
    {
        await cacheInvalidator.Invalidate("CharactersById");
        await cacheInvalidator.Invalidate("CharactersFilter");
    }

}
