using Microsoft.Extensions.Logging;
using RickAndMorty.Contracts;
using RickAndMorty.DTO;
using System.Text.Json;

namespace RickAndMorty.Services;

public sealed class RickAndMortyApiService(ILogger<RickAndMortyApiService> logger,
    HttpClient httpClient) : IRickAndMortyApiService
{
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<IEnumerable<T>> FetchAllEpisodesAsync<T>(string url)
    {
        var allRecords = new List<T>();
        int currentPage = 0;

        while (!string.IsNullOrEmpty(url))
        {
            currentPage++;
            var response = await GetAsync<T>(url);

            if (response.Results.Count > 0)
            {
                allRecords.AddRange(response.Results);
            }

            url = string.IsNullOrEmpty(response.Info.Next) ? string.Empty : new Uri(response.Info.Next).PathAndQuery;
            logger.LogInformation($"Retrieved page {currentPage} of {response.Info.Pages}");
        }

        return allRecords;
    }

    private async Task<ApiResponse<T>> GetAsync<T>(string url)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var response = await httpClient.SendAsync(request);
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
