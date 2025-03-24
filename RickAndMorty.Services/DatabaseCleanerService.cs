using RickAndMorty.Contracts;

namespace RickAndMorty.Services;

public class DatabaseCleanerService(IRickAndMortyContextFactory contextFactory) : IDatabaseCleanerService
{
    public async Task CleanAsync()
    {
        var context = await contextFactory.CreateContextAsync();

        // DO NOT USE THIS METHOD ON A PRODUCTION SERVER !!
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}
