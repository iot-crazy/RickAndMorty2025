using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using RickAndMorty.Contracts;

namespace RickAndMorty.Web.Controllers;

[ApiController]
[Route("api/[controller]")]

public class EpisodeController(IEpisodeService episodeService) : Controller
{
    [OutputCache]
    [OutputCacheWithHeader]
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await episodeService.GetAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id) => Ok(await episodeService.GetAsync(id));
}
