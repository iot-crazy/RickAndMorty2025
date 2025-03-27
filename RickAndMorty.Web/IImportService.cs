
namespace RickAndMorty.Web;
public interface IImportService
{
    Task StartAsync(string connectionId);
}