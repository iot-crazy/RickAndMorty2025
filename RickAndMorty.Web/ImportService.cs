using Microsoft.AspNetCore.SignalR;
using RickAndMorty.Contracts;

namespace RickAndMorty.Web;

public sealed class ImportService(ILogger<ImportService> logger,
                    ICharacterService characterService,
                    ILocationService locationService,
                    IEpisodeService episodeService,
                    IDatabaseCleanerService databaseCleanerService,
                    IHubContext<ImportHub> hubContext
                    ) : IImportService
{

    private string ConnectionId = string.Empty;

    public async Task StartAsync(string connectionId)
    {
        ConnectionId = connectionId;

        await ReportAsync("Starting import...");

        await ReportAsync("Step 1 of 4 - Clearing database");
        logger.LogInformation("Clearing database.");
        await databaseCleanerService.CleanAsync();

        await ReportAsync("Step 2 of 4 - Getting locations");
        logger.LogInformation("Getting locations");
        await locationService.GetAllFromApiAsync();

        await ReportAsync("Step 3 of 4 - Getting episodes");
        logger.LogInformation("Getting episodes");
        await episodeService.GetAllFromApiAsync();

        await ReportAsync("Final step - Getting characters");
        logger.LogInformation("Getting characters");
        await characterService.GetAllFromApiAsync();

        await RepostFinishedAsync();
    }

    private async Task ReportAsync(string message)
    {
        await hubContext.Clients.Client(ConnectionId).SendAsync("ProgressChanged", message);
    }

    private async Task RepostFinishedAsync()
    {
        await hubContext.Clients.Client(ConnectionId).SendAsync("ImportComplete");
    }

}
