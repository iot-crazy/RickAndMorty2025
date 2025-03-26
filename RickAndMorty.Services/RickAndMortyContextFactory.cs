using Microsoft.EntityFrameworkCore;
using RickAndMorty.Contracts;
using RickAndMorty.DB;

namespace RickAndMorty.Services;

public sealed class RickAndMortyContextFactory(IDbContextFactory<RickAndMortyContext> contextFactory) : IRickAndMortyContextFactory
{
    public async Task<RickAndMortyContext> CreateContextAsync()
    {
        var context = await contextFactory.CreateDbContextAsync();
        return context;
    }
}