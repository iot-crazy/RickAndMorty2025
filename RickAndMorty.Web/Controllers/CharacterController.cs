using Microsoft.AspNetCore.Mvc;
using RickAndMorty.Contracts;

namespace RickAndMorty.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CharacterController(ICharacterService characterService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await characterService.GetAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id) => Ok(await characterService.GetAsync(id));
}
