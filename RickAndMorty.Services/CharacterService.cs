using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RickAndMorty.Contracts;
using RickAndMorty.DB.Models;
using RickAndMorty.DTO;

namespace RickAndMorty.Services;

public class CharacterService(IRickAndMortyContextFactory contextFactory,
    IRickAndMortyApiService apiService,
    IMapper mapper,
    ILogger<CharacterService> logger) : ICharacterService
{
    public async Task<IReadOnlyList<CharacterDto>> GetAsync()
    {
        var context = await contextFactory.CreateContextAsync();
        return await context.Characters
            .AsNoTracking()
            .ProjectTo<CharacterDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<CharacterDto?> Get(int id)
    {
        var context = await contextFactory.CreateContextAsync();
        return await context.Characters
            .AsNoTracking()
            .Where(x => x.Id == id)
            .ProjectTo<CharacterDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<CharacterDto>> Get(string name)
    {
        var context = await contextFactory.CreateContextAsync();
        return await context.Characters
            .AsNoTracking()
            .Where(x => x.Name == name)
            .ProjectTo<CharacterDto>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task AddAsync(CharacterDto dto)
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
        try
        {
            int currentPage = 0;
            string url = "api/character/?status=alive";
            var context = await contextFactory.CreateContextAsync();

            while (string.IsNullOrEmpty(url) == false)
            {
                currentPage++;
                var response = await apiService.GetAsync<CharacterDto>(url);

                if (response.Results.Count > 0)
                {
                    var entities = mapper.Map<List<Character>>(response.Results);
                    context.Characters.AddRange(entities);
                }

                url = string.IsNullOrEmpty(response.Info.Next) ? string.Empty : new Uri(response.Info.Next).PathAndQuery;
                logger.LogInformation($"Retrieved page {currentPage} of {response.Info.Pages}");
            }

            await context.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            throw ex;
        }

        return await CountAsync();

    }
}
