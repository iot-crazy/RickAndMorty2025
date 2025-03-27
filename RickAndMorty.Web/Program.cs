using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using RickAndMorty.Contracts;
using RickAndMorty.DB;
using RickAndMorty.Services;
using RickAndMorty.Web;
using RickAndMorty.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

builder.Services.AddControllers();
builder.Services.AddSignalR();

// Add essential services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// ** Scan for services to inject so we don't need to add them all by hand! ** //
builder.Services.Scan(scan => scan
        .FromApplicationDependencies(assembly => assembly.FullName!.StartsWith("RickAndMorty.Services"))
        .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service") && !type.IsAbstract))
        .AsImplementedInterfaces()
        .WithScopedLifetime()
);

// ** Scan for all providers also - they live in the same namespace **  //
builder.Services.Scan(scan => scan
        .FromApplicationDependencies(assembly => assembly.FullName!.StartsWith("RickAndMorty.Services"))
        .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Provider") && !type.IsAbstract))
        .AsImplementedInterfaces()
        .WithScopedLifetime()
);


builder.Services.AddScoped<IImportService, ImportService>();

// ** Register the database context and factory ** //
builder.Services.AddScoped<IRickAndMortyContextFactory, RickAndMortyContextFactory>()
    .AddScoped<ICacheInvalidator, CacheInvalidator>();

builder.Services.AddDbContextFactory<RickAndMortyContext>((provider, options) =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

});


// ** Automapper ** //
builder.Services.AddAutoMapper(typeof(CharacterProfile).Assembly);

// ** Setup OutputCache **//
int cacheMinutes = builder.Configuration.GetValue<int>("CacheMinutes");
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromMinutes(cacheMinutes)));
});


//  ** To access our own API from WASM ** //
builder.Services.AddHttpClient<IRickAndMortyApiService, RickAndMortyApiService>((provider, client) =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    client.BaseAddress = new Uri(config.GetValue<string>("ApiBaseAddress") ?? "");
});

builder.Services.AddHttpClient("ServerAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7036/"); // or use builder.HostEnvironment.BaseAddress
});
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("ServerAPI"));

// ** Add swagger ** //
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var app = builder.Build();

// ** Ensure the database is created and the schema is migrated ** //
using (var scope = app.Services.CreateScope())
{
    try
    {
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<RickAndMortyContext>>();
        await using var context = await factory.CreateDbContextAsync();
        await context.Database.EnsureCreatedAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Migration failed:");
        Console.WriteLine(ex);
    }
}

// ** Our SignalR hub for the import servers ** //
app.MapHub<ImportHub>("/importhub");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseOutputCache();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(RickAndMorty.Web.Client._Imports).Assembly);

app.Run();
