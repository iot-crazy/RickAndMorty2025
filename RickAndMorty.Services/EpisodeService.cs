using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RickAndMorty.Contracts;
using RickAndMorty.DB.Models;
using RickAndMorty.DTO.Episode;

namespace RickAndMorty.Services;

public sealed class EpisodeService(IRickAndMortyContextFactory contextFactory,
    IRickAndMortyApiService apiService,
    IMapper mapper) : IEpisodeService
{
    public async Task<IReadOnlyList<EpisodeDto>> GetAsync()
    {
        var context = await contextFactory.CreateContextAsync();
        return await context.Episodes
            .AsNoTracking()
            .ProjectTo<EpisodeDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<EpisodeDto?> GetAsync(int id)
    {
        var context = await contextFactory.CreateContextAsync();
        return await context.Episodes
            .AsNoTracking()
            .Where(x => x.Id == id)
            .ProjectTo<EpisodeDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<EpisodeDto>> GetAsync(string name)
    {
        var context = await contextFactory.CreateContextAsync();
        return await context.Episodes
            .AsNoTracking()
            .Where(x => x.Name == name)
            .ProjectTo<EpisodeDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task AddAsync(NewApiEpisodeDto dto)
    {
        var context = await contextFactory.CreateContextAsync();
        var entity = mapper.Map<Episode>(dto);
        context.Episodes.Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var context = await contextFactory.CreateContextAsync();
        var entity = await context.Episodes.FindAsync(id);
        if (entity is null) return;
        context.Episodes.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<int> CountAsync()
    {
        var context = await contextFactory.CreateContextAsync();
        return await context.Episodes.CountAsync();
    }

    public async Task<int> GetAllFromApiAsync()
    {
        var context = await contextFactory.CreateContextAsync();
        var episodes = await apiService.FetchAllEpisodesAsync<NewApiEpisodeDto>("api/episode/");
        var newEpisodes = mapper.Map<IEnumerable<Episode>>(episodes);
        context.Episodes.AddRange(newEpisodes);
        await context.SaveChangesAsync();
        return await CountAsync();
    }
}
