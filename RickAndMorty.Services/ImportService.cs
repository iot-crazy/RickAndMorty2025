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

        Report("Starting import...");


        Report("Step 1 of 4 - Clearing database");
        logger.LogInformation("Clearing database.");
        await databaseCleanerService.CleanAsync();

        Report("Step 2 of 4 - Getting locations");
        logger.LogInformation("Getting locations");
        await locationService.GetAllFromApiAsync();

        Report("Step 3 of 4 - Getting episodes");
        logger.LogInformation("Getting episodes");
        await episodeService.GetAllFromApiAsync();

        Report("Final step - Getting characters");
        logger.LogInformation("Getting characters");
        await characterService.GetAllFromApiAsync();

        Report("All done - checking status... ");
    }

    public event Action<string>? ProgressChanged;

    private void Report(string message)
    {
        logger.LogInformation(message);
        ProgressChanged?.Invoke(message);
    }

}
