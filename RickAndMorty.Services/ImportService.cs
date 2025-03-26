using Microsoft.Extensions.Logging;
using RickAndMorty.Contracts;

namespace RickAndMorty.Services;

public sealed class ImportService(ILogger<ImportService> logger,
                    ICharacterService characterService,
                    ILocationService locationService,
                    IEpisodeService episodeService,
                    IDatabaseCleanerService databaseCleanerService
                    ) : IImportService
{

    public async Task StartAsync()
    {
        logger.LogDebug($"Starting with arguments: {string.Join(" ", Environment.GetCommandLineArgs())}");


        logger.LogInformation("Starting character retrieval...");

        logger.LogInformation("Clearing database.");
        await databaseCleanerService.CleanAsync();

        logger.LogInformation("Getting locations");
        await locationService.GetAllFromApiAsync();

        logger.LogInformation("Getting episodes");
        await episodeService.GetAllFromApiAsync();

        logger.LogInformation("Getting characters");
        await characterService.GetAllFromApiAsync();

    }

}
