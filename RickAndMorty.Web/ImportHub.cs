using Microsoft.AspNetCore.SignalR;

namespace RickAndMorty.Web;

public class ImportHub(IImportService importService, ILogger<ImportHub> logger) : Hub
{
    public async Task StartImport()
    {
        logger.LogInformation("Import starting.");
        var connectionId = Context.ConnectionId;
        await importService.StartAsync(connectionId);
    }
}
