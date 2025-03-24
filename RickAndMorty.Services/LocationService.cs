using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RickAndMorty.Contracts;
using RickAndMorty.DB.Models;
using RickAndMorty.DTO;

namespace RickAndMorty.Services;

public class LocationService(IRickAndMortyContextFactory contextFactory,
    IRickAndMortyApiService apiService,
    IMapper mapper,
    ILogger<LocationService> logger) : ILocationService
{
    public async Task<IReadOnlyList<LocationDto>> GetAsync()
    {
        var context = await contextFactory.CreateContextAsync();
        return await context.Locations
            .AsNoTracking()
            .ProjectTo<LocationDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<LocationDto?> Get(int id)
    {
        var context = await contextFactory.CreateContextAsync();
        return await context.Locations
            .AsNoTracking()
            .Where(x => x.Id == id)
            .ProjectTo<LocationDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<LocationDto>> Get(string name)
    {
        var context = await contextFactory.CreateContextAsync();
        return await context.Locations
            .AsNoTracking()
            .Where(x => x.Name == name)
            .ProjectTo<LocationDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task AddAsync(LocationDto dto)
    {
        var context = await contextFactory.CreateContextAsync();
        var entity = mapper.Map<Location>(dto);
        context.Locations.Add(entity);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var context = await contextFactory.CreateContextAsync();
        var entity = await context.Locations.FindAsync(id);
        if (entity is null) return;
        context.Locations.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<int> CountAsync()
    {
        var context = await contextFactory.CreateContextAsync();
        return await context.Locations.CountAsync();
    }

    public async Task<int> GetAllFromApiAsync()
    {
        int currentPage = 0;
        string url = "api/location/";
        var context = await contextFactory.CreateContextAsync();

        while (string.IsNullOrEmpty(url) == false)
        {
            currentPage++;
            var response = await apiService.GetAsync<LocationDto>(url);

            if (response.Results.Count > 0)
            {
                var entities = mapper.Map<List<Location>>(response.Results);
                context.Locations.AddRange(entities);
            }

            url = string.IsNullOrEmpty(response.Info.Next) ? string.Empty : new Uri(response.Info.Next).PathAndQuery;
            logger.LogInformation($"Retrieved page {currentPage} of {response.Info.Pages}");
        }

        await context.SaveChangesAsync();
        return await CountAsync();
    }
}
