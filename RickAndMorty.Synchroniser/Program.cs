using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RickAndMorty.Contracts;
using RickAndMorty.DB;
using RickAndMorty.Services;

namespace RickAndMorty.Synchroniser;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting synchroniser");
        var host = Host.CreateDefaultBuilder(args)
           .ConfigureAppConfiguration((hostingContext, config) =>
           {
               config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
           }); ;

        host.ConfigureServices((context, services) =>
                  services
                  .AddHttpClient()
                  .AddLogging()
                  .AddHostedService<Synchroniser>()
                  .AddDbContextFactory<RickAndMortyContext>(opt => opt.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")))
                  .AddAutoMapper(typeof(CharacterProfile).Assembly)
                  .AddSingleton<IRickAndMortyContextFactory, RickAndMortyContextFactory>()
                  .Scan(scan => scan
                    .FromApplicationDependencies(assembly => assembly.FullName!.StartsWith("RickAndMorty.Services"))
                    .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service") && !type.IsAbstract))
                    .AsImplementedInterfaces()
                    .WithSingletonLifetime())
                  );

        var app = host.Build();

        var context = app.Services.GetService<RickAndMortyContext>() ?? throw new Exception("Database context is null.");
        try
        {
            context.Database.EnsureCreated();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

        app.Run();
    }
}