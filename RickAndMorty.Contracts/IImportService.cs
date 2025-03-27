
namespace RickAndMorty.Contracts
{
    public interface IImportService
    {
        event Action<string>? ProgressChanged;
        Task StartAsync();
    }
}