namespace RickAndMorty.Contracts;


/// <summary>
/// A wrapper to provide access to invalidte the cache because the cacheStore cannot be accessed outside a web project
/// </summary>
public interface ICacheInvalidator
{
    Task Invalidate(string tag);
}
