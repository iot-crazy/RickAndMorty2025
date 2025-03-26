using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using RickAndMorty.Contracts;
using RickAndMorty.DTO.Character;

namespace RickAndMorty.Web.Controllers.api;

[ApiController]
[Route("api/[controller]")]

public class CharacterController(ICharacterService characterService) : Controller
{
    //[OutputCache(Tags = ["Characters"])]
    //[OutputCacheWithHeader]
    //[HttpGet]
    //public async Task<IActionResult> Get()
    //{
    //    return Ok(await characterService.GetAsync());
    //}

    [OutputCache(Tags = ["Characters"], VaryByRouteValueNames = ["id"])]
    [OutputCacheWithHeader]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id) => Ok(await characterService.GetAsync(id));

    [OutputCache(Tags = ["Characters"], VaryByQueryKeys = ["name", "planet", "status", "gender"])]
    [OutputCacheWithHeader]
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] CharacterFilter? filter = null) => Ok(await characterService.GetAsync(filter));


    [HttpPost]
    public async Task<IActionResult> Save(NewCharacterDto newCharacter) => Ok(await characterService.AddAsync(newCharacter));
}
