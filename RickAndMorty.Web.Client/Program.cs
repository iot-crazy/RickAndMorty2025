using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using RickAndMorty.Contracts;
using RickAndMorty.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();
builder.Services.AddScoped<IApiCharacterDataProvider, ApiCharacterDataProvider>();
builder.Services.AddScoped<ILocationDataProvider, ApiLocationDataProvider>();
builder.Services.AddScoped<IEpisodeDataProvider, ApiEpisodeDataProvider>();
builder.Services.AddScoped<ICharacterService, CharacterService>();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

await builder.Build().RunAsync();
