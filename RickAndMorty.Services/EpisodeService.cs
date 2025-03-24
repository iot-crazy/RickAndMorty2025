using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RickAndMorty.Contracts;
using RickAndMorty.DB.Models;
using RickAndMorty.DTO;

namespace RickAndMorty.Services;

public class EpisodeService(IRickAndMortyContextFactory contextFactory,
    IRickAndMortyApiService apiService,
    IMapper mapper,
     ILogger<EpisodeService> logger) : IEpisodeService
{
    public async Task<IReadOnlyList<EpisodeDto>> GetAsync()
    {
        var context = await contextFactory.CreateContextAsync();
        return await context.Episodes
            .AsNoTracking()
            .ProjectTo<EpisodeDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<EpisodeDto?> Get(int id)
    {
        var context = await contextFactory.CreateContextAsync();
        return await context.Episodes
            .AsNoTracking()
            .Where(x => x.Id == id)
            .ProjectTo<EpisodeDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<EpisodeDto>> Get(string name)
    {
        var context = await contextFactory.CreateContextAsync();
        return await context.Episodes
            .AsNoTracking()
            .Where(x => x.Name == name)
            .ProjectTo<EpisodeDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task AddAsync(EpisodeDto dto)
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
        int currentPage = 0;
        string url = "api/episode/";
        var context = await contextFactory.CreateContextAsync();

        while (string.IsNullOrEmpty(url) == false)
        {
            currentPage++;
            var response = await apiService.GetAsync<EpisodeDto>(url);

            if (response.Results.Count > 0)
            {
                var entities = mapper.Map<List<Episode>>(response.Results);
                context.Episodes.AddRange(entities);
            }

            url = string.IsNullOrEmpty(response.Info.Next) ? string.Empty : new Uri(response.Info.Next).PathAndQuery;
            logger.LogInformation($"Retrieved page {currentPage} of {response.Info.Pages}");
        }

        await context.SaveChangesAsync();
        return await CountAsync();
    }
}
