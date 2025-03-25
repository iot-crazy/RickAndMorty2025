using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RickAndMorty.Contracts;
using RickAndMorty.DB;
using RickAndMorty.DB.Models;
using RickAndMorty.DTO.Episode;
using RickAndMorty.Services;

namespace RickAndMorty.UnitTests;

public class EpisodeServiceTests
{
    private readonly IMapper _mapper;
    private readonly DbContextOptions<RickAndMortyContext> _dbOptions;

    public EpisodeServiceTests()
    {
        // Set up AutoMapper
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<EpisodeProfile>();
        });

        _mapper = config.CreateMapper();

        // EF InMemory setup
        _dbOptions = new DbContextOptionsBuilder<RickAndMortyContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private IRickAndMortyContextFactory GetFactoryWithSeedData(List<Episode> seedEpisodes)
    {
        var context = new RickAndMortyContext(_dbOptions);
        context.Episodes.AddRange(seedEpisodes);
        context.SaveChanges();

        var mockFactory = new Mock<IRickAndMortyContextFactory>();
        mockFactory.Setup(f => f.CreateContextAsync()).ReturnsAsync(context);
        return mockFactory.Object;
    }

    [Fact]
    public async Task GetAsync_ShouldReturnProjectedEpisodes()
    {
        // Arrange
        var airDate = new DateTime(2013, 12, 2);
        var createdDate = new DateTime(2020, 12, 2);
        var episodes = new List<Episode>
    {
        new()
        {
            Id = 1,
            Name = "Pilot",
            AirDate = airDate,
            Code = "S01E01",
            Url = "http://example.com",
            Created = createdDate
        }
    };

        var factory = GetFactoryWithSeedData(episodes);
        var apiMock = new Mock<IRickAndMortyApiService>();
        var service = new EpisodeService(factory, apiMock.Object, _mapper);

        // Act
        var result = await service.GetAsync();

        // Assert
        result.Should().HaveCount(1);
        var dto = result[0];
        dto.Id.Should().Be(1);
        dto.Name.Should().Be("Pilot");
        dto.Code.Should().Be("S01E01");
        dto.Url.Should().Be("http://example.com");
        dto.AirDate.Should().Be(airDate);
        dto.Created.Should().Be(createdDate);
    }


    [Fact]
    public async Task AddAsync_ShouldAddEpisodeToDatabase_WithAllProperties()
    {
        // Arrange
        var airDate = new DateTime(2013, 12, 2);
        var createdDate = new DateTime(2020, 12, 2);
        var dto = new NewEpisodeDto
        {
            Id = 2,
            Name = "Episode 2",
            AirDate = "December 2, 2013",
            Code = "S01E02",
            Url = "http://example.com/2",
            Created = createdDate
        };

        var factory = GetFactoryWithSeedData([]);
        var apiMock = new Mock<IRickAndMortyApiService>();
        var service = new EpisodeService(factory, apiMock.Object, _mapper);

        // Act
        await service.AddAsync(dto);
        var allEpisodes = await service.GetAsync();

        // Assert
        allEpisodes.Should().ContainSingle();
        var result = allEpisodes[0];
        result.Id.Should().Be(2);
        result.Name.Should().Be("Episode 2");
        result.AirDate.Should().Be(airDate);
        result.Code.Should().Be("S01E02");
        result.Url.Should().Be("http://example.com/2");
        result.Created.Should().Be(createdDate);
    }


    [Fact]
    public async Task GetAllFromApiAsync_ShouldFetchFromApi_AndSaveToDb()
    {
        // Arrange
        var airDate = new DateTime(2013, 12, 2);
        var createdDate = new DateTime(2020, 12, 2);
        var fakeDtos = new List<NewEpisodeDto>
    {
        new()
        {
            Id = 3,
            Name = "From API",
            AirDate = airDate.ToString("MMMM d, yyyy"),
            Code = "S01E03",
            Url = "http://api.com/ep3",
            Created = createdDate
        }
    };

        var apiMock = new Mock<IRickAndMortyApiService>();
        apiMock.Setup(api => api.FetchAllEpisodesAsync<NewEpisodeDto>("api/episode/"))
               .ReturnsAsync(fakeDtos);

        var factory = GetFactoryWithSeedData([]);
        var service = new EpisodeService(factory, apiMock.Object, _mapper);

        // Act
        var count = await service.GetAllFromApiAsync();

        // Assert
        count.Should().Be(1);
        var episodes = await service.GetAsync();
        episodes.Should().ContainSingle();
        var result = episodes[0];
        result.Id.Should().Be(3);
        result.Name.Should().Be("From API");
        result.Code.Should().Be("S01E03");
        result.Url.Should().Be("http://api.com/ep3");
        result.AirDate.Should().Be(airDate);
        result.Created.Should().Be(createdDate);
    }

    [Fact]
    public async Task GetAsync_ById_ShouldReturnMatchingEpisode()
    {
        // Arrange
        var airDate = new DateTime(2013, 12, 2);
        var createdDate = new DateTime(2020, 12, 2);
        var episodes = new List<Episode>
    {
        new() { Id = 10, Name = "Test Episode", AirDate = airDate, Code = "S01E10", Url = "http://ep10", Created = createdDate }
    };

        var factory = GetFactoryWithSeedData(episodes);
        var apiMock = new Mock<IRickAndMortyApiService>();
        var service = new EpisodeService(factory, apiMock.Object, _mapper);

        // Act
        var result = await service.GetAsync(10);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(10);
        result.Name.Should().Be("Test Episode");
        result.Code.Should().Be("S01E10");
        result.Url.Should().Be("http://ep10");
        result.AirDate.Should().Be(airDate);
        result.Created.Should().Be(createdDate);
    }

    [Fact]
    public async Task GetAsync_ByName_ShouldReturnMatchingEpisodes()
    {
        // Arrange
        var airDate = new DateTime(2013, 12, 2);
        var createdDate = new DateTime(2020, 12, 2);
        var episodes = new List<Episode>
    {
        new() { Id = 11, Name = "Special", AirDate = airDate, Code = "S01E11", Url = "http://ep11", Created = createdDate },
        new() { Id = 12, Name = "Special", AirDate = airDate, Code = "S01E12", Url = "http://ep12", Created = createdDate }
    };

        var factory = GetFactoryWithSeedData(episodes);
        var apiMock = new Mock<IRickAndMortyApiService>();
        var service = new EpisodeService(factory, apiMock.Object, _mapper);

        // Act
        var result = await service.GetAsync("Special");

        // Assert
        result.Should().HaveCount(2);
        foreach (var ep in result)
        {
            ep.Name.Should().Be("Special");
            ep.AirDate.Should().Be(airDate);
            ep.Created.Should().Be(createdDate);
            ep.Url.Should().StartWith("http://ep");
            ep.Code.Should().StartWith("S01E");
        }
    }


    [Fact]
    public async Task DeleteAsync_ShouldRemoveEpisode_WhenItExists()
    {
        // Arrange
        var episodes = new List<Episode>
    {
        new() { Id = 20, Name = "To Delete", AirDate = new DateTime(2024, 3, 1), Code = "S01E20", Url = "http://ep20", Created = DateTime.UtcNow }
    };

        var factory = GetFactoryWithSeedData(episodes);
        var apiMock = new Mock<IRickAndMortyApiService>();
        var service = new EpisodeService(factory, apiMock.Object, _mapper);

        // Act
        await service.DeleteAsync(20);
        var result = await service.GetAsync(20);

        // Assert
        result.Should().BeNull();
    }

}
