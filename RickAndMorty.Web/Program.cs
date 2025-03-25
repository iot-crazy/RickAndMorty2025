using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using RickAndMorty.Contracts;
using RickAndMorty.DB;
using RickAndMorty.Services;
using RickAndMorty.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.Scan(scan => scan
        .FromApplicationDependencies(assembly => assembly.FullName!.StartsWith("RickAndMorty.Services"))
        .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service") && !type.IsAbstract))
        .AsImplementedInterfaces()
        .WithScopedLifetime()
);

builder.Services
    .AddScoped<ICharacterDataProvider, ServerCharacterDataProvider>()
    .AddSingleton<IRickAndMortyContextFactory, RickAndMortyContextFactory>();

builder.Services.AddDbContextFactory<RickAndMortyContext>((provider, options) =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(typeof(CharacterProfile).Assembly);

// To access our own API

builder.Services.AddHttpClient<IRickAndMortyApiService, RickAndMortyApiService>((provider, client) =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config.GetValue<string>("ApiBaseAddress") ?? "");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.MapControllers();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(RickAndMorty.Web.Client._Imports).Assembly);

app.Run();
