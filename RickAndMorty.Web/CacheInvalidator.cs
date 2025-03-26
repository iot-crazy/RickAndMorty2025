using Microsoft.AspNetCore.OutputCaching;
using RickAndMorty.Contracts;

namespace RickAndMorty.Web;

public class CacheInvalidator(IOutputCacheStore cacheStore) : ICacheInvalidator
{
    public async Task Invalidate(string tag)
    {
        await cacheStore.EvictByTagAsync(tag, CancellationToken.None);
    }
}
