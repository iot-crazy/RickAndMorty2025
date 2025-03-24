using RickAndMorty.DB;

namespace RickAndMorty.Contracts;

public interface IRickAndMortyContextFactory
{
    Task<RickAndMortyContext> CreateContextAsync();
}
