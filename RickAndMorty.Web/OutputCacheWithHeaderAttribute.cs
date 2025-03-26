using Microsoft.AspNetCore.Mvc.Filters;

namespace RickAndMorty.Web;

public class OutputCacheWithHeaderAttribute : Attribute, IFilterFactory
{
    public bool IsReusable => false;

    public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
    {
        return new CacheHeaderFilter();
    }

    private class CacheHeaderFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            // Only add the header if we are generating the response (cache miss)
            context.HttpContext.Response.OnStarting(() =>
            {
                context.HttpContext.Response.Headers["from-database"] = "true";
                return Task.CompletedTask;
            });

            await next();
        }
    }
}

