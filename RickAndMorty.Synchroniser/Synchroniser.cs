using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RickAndMorty.Contracts;

namespace RickAndMorty.Synchroniser;

internal sealed class Synchroniser(ILogger<Synchroniser> logger,
                    IHostApplicationLifetime appLifetime,
                    ICharacterService characterService,
                    ILocationService locationService,
                    IEpisodeService episodeService,
                    IDatabaseCleanerService databaseCleanerService
                    ) : IHostedService
{

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogDebug($"Starting with arguments: {string.Join(" ", Environment.GetCommandLineArgs())}");

        appLifetime.ApplicationStarted.Register(() =>
        {
            Task.Run(async () =>
            {
                try
                {
                    logger.LogInformation("Starting character retrieval...");

                    // TODO: get locations
                    // TODO: match charater to episodes and the location

                    logger.LogInformation("Clearing database.");
                    await databaseCleanerService.CleanAsync();

                    logger.LogInformation("Getting locations");
                    await locationService.GetAllFromApiAsync();

                    logger.LogInformation("Getting episodes");
                    await episodeService.GetAllFromApiAsync();

                    logger.LogInformation("Getting characters");
                    await characterService.GetAllFromApiAsync();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unhandled exception!");
                }
                finally
                {
                    // Stop the application once the work is done
                    appLifetime.StopApplication();
                }
            });
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
