using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using RickAndMorty.DTO;
using RickAndMorty.DTO.Episode;
using RickAndMorty.Services;
using System.Net;
using System.Text;

namespace RickAndMorty.UnitTests;

public class RickAndMortyApiServiceTests
{

    [Fact]
    public async Task GetAsync_ShouldReturnCorrectData()
    {
        // Arrange
        var expectedPage1 = JsonHelper.GetJson<ApiResponse<NewApiEpisodeDto>>("TestDataFiles/EpisodesPage1.json");
        var expectedPage2 = JsonHelper.GetJson<ApiResponse<NewApiEpisodeDto>>("TestDataFiles/EpisodesPage2.json");

        //  var content = new StringContent(expected.Json, Encoding.UTF8, "application/json");
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
         .Protected()
         .Setup<Task<HttpResponseMessage>>(
             "SendAsync",
             ItExpr.IsAny<HttpRequestMessage>(),
             ItExpr.IsAny<CancellationToken>())
         .Returns<HttpRequestMessage, CancellationToken>((request, _) =>
         {
             var uri = request.RequestUri?.ToString();
             string json;

             if (uri is not null && uri.Contains("page=2"))
             {
                 json = expectedPage2.Json;
             }
             else
             {
                 json = expectedPage1.Json;
             }

             return Task.FromResult(new HttpResponseMessage
             {
                 StatusCode = HttpStatusCode.OK,
                 Content = new StringContent(json, Encoding.UTF8, "application/json")
             });
         });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://fakeapi.com/")
        };

        var loggerMock = new Mock<ILogger<RickAndMortyApiService>>();
        var service = new RickAndMortyApiService(loggerMock.Object, httpClient);

        // Act
        var result = await service.FetchAllEpisodesAsync<NewApiEpisodeDto>("some-endpoint");

        // Assert
        var expectedResults = expectedPage1.ObjectContent.Results.Concat(expectedPage2.ObjectContent.Results).ToList();
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedResults);
    }

}

