using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RickAndMorty.Contracts;
using RickAndMorty.DTO;
using System.Text.Json;

namespace RickAndMorty.Services;

public class RickAndMortyApiService(ILogger<RickAndMortyApiService> logger, IHttpClientFactory clientFactory,
    IConfiguration config) : IRickAndMortyApiService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<ApiResponse<T>> GetAsync<T>(string url)
    {
        string? baseUrl = config.GetValue<string>("ApiBaseAddress");

        if (string.IsNullOrEmpty(baseUrl))
        {
            throw new Exception(nameof(baseUrl));
        }

        var client = clientFactory.CreateClient();
        client.BaseAddress = new Uri(baseUrl);
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var response = await client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            return await Read<T>(response);
        }

        logger.LogWarning($"Unable to retrieve character data. Status code was {response.StatusCode}.");
        throw new Exception("Unable to obtain API data - reason unknown!");
    }

    private async Task<ApiResponse<T>> Read<T>(HttpResponseMessage response)
    {
        using var responseStream = await response.Content.ReadAsStreamAsync();
        var responseData = await JsonSerializer.DeserializeAsync
                <ApiResponse<T>>(responseStream, _jsonOptions);
        if (responseData is null)
        {
            logger.LogError("Api Response was null.");
            throw new Exception($"API resonse was empty.");
        }
        return responseData;
    }
}
