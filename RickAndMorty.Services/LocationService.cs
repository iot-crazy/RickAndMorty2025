using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RickAndMorty.Contracts;
using RickAndMorty.DB.Models;
using RickAndMorty.DTO.Location;

namespace RickAndMorty.Services;

public sealed class LocationService(IRickAndMortyContextFactory contextFactory,
    IRickAndMortyApiService apiService,
    IMapper mapper) : ILocationService
{
    public async Task<IReadOnlyList<LocationDto>> GetAsync()
    {
        var context = await contextFactory.CreateContextAsync();
        return await context.Locations
            .AsNoTracking()
            .ProjectTo<LocationDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<LocationDto?> GetAsync(int id)
    {
        var context = await contextFactory.CreateContextAsync();
        return await context.Locations
            .AsNoTracking()
            .Where(x => x.Id == id)
            .ProjectTo<LocationDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<LocationDto>> GetAsync(string name)
    {
        var context = await contextFactory.CreateContextAsync();
        return await context.Locations
            .AsNoTracking()
            .Where(x => x.Name == name)
            .ProjectTo<LocationDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task AddAsync(NewApiLocationDto dto)
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
        var context = await contextFactory.CreateContextAsync();
        var locations = await apiService.FetchAllEpisodesAsync<NewApiLocationDto>("api/location/");
        var newLocations = mapper.Map<IEnumerable<Location>>(locations);
        context.Locations.AddRange(newLocations);
        await context.SaveChangesAsync();
        return await CountAsync();
    }
}
