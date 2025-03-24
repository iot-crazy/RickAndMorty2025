namespace RickAndMorty.Contracts;

public interface IDatabaseCleanerService
{
    Task CleanAsync();
}