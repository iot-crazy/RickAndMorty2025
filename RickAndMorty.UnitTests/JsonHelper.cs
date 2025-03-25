using System.Text.Json;

namespace RickAndMorty.UnitTests;

internal static class JsonHelper
{

    private static JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    internal static JsonContent<T> GetJson<T>(string fileName)
    {
        var jsonFilepath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
        var json = File.ReadAllText(jsonFilepath);
        var content = JsonSerializer.Deserialize<T>(json, _jsonOptions);

        if (content == null)
        {
            throw new Exception($"Unable to deseralise json to type {typeof(T)}.");
        }

        return new JsonContent<T>
        {
            Json = json,
            ObjectContent = content
        };
    }

    internal static Stream GetJsonAsStream(string fileName)
    {
        var jsonFilepath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
        var stream = File.OpenRead(jsonFilepath);
        return stream;
    }

}

internal class JsonContent<T>
{
    public required string Json { get; set; }
    public required T ObjectContent { get; set; }
}
