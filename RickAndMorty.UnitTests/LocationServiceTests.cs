using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RickAndMorty.Contracts;
using RickAndMorty.DB;
using RickAndMorty.DB.Models;
using RickAndMorty.DTO.Location;
using RickAndMorty.Services;


namespace RickAndMorty.UnitTests;

public class LocationServiceTests
{
    private readonly IMapper _mapper;
    private readonly DbContextOptions<RickAndMortyContext> _dbOptions;


    public LocationServiceTests()
    {
        // Set up AutoMapper
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<LocationProfile>();
        });

        _mapper = config.CreateMapper();

        // EF InMemory setup
        _dbOptions = new DbContextOptionsBuilder<RickAndMortyContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
    }

    private IRickAndMortyContextFactory GetFactoryWithSeedData(List<Location> seedlocations)
    {
        var context = new RickAndMortyContext(_dbOptions);
        context.Locations.AddRange(seedlocations);
        context.SaveChanges();

        var mockFactory = new Mock<IRickAndMortyContextFactory>();
        mockFactory.Setup(f => f.CreateContextAsync()).ReturnsAsync(context);
        return mockFactory.Object;
    }

    [Fact]
    public async Task GetAsync_ShouldReturnAllLocations_WithAllProperties()
    {
        var created = new DateTime(2020, 12, 2);
        var locations = new List<Location>
    {
        new() { Id = 1, Name = "Earth", Type = "Planet", Dimension = "Dimension C-137", Url = "http://example.com/loc1", Created = created }
    };

        var factory = GetFactoryWithSeedData(locations);
        var apiMock = new Mock<IRickAndMortyApiService>();
        var service = new LocationService(factory, apiMock.Object, _mapper);

        var result = await service.GetAsync();

        result.Should().HaveCount(1);
        var loc = result[0];
        loc.Id.Should().Be(1);
        loc.Name.Should().Be("Earth");
        loc.Type.Should().Be("Planet");
        loc.Dimension.Should().Be("Dimension C-137");
        loc.Url.Should().Be("http://example.com/loc1");
        loc.Created.Should().Be(created);
    }

    [Fact]
    public async Task GetAsync_ById_ShouldReturnMatchingLocation_WithAllProperties()
    {
        var created = new DateTime(2020, 12, 2);
        var locations = new List<Location>
    {
        new() { Id = 2, Name = "Gazorpazorp", Type = "Planet", Dimension = "Unknown", Url = "http://example.com/loc2", Created = created }
    };

        var factory = GetFactoryWithSeedData(locations);
        var apiMock = new Mock<IRickAndMortyApiService>();
        var service = new LocationService(factory, apiMock.Object, _mapper);

        var result = await service.GetAsync(2);

        result.Should().NotBeNull();
        result!.Id.Should().Be(2);
        result.Name.Should().Be("Gazorpazorp");
        result.Type.Should().Be("Planet");
        result.Dimension.Should().Be("Unknown");
        result.Url.Should().Be("http://example.com/loc2");
        result.Created.Should().Be(created);
    }

    [Fact]
    public async Task GetAsync_ByName_ShouldReturnMatchingLocations_WithAllProperties()
    {
        var created = new DateTime(2020, 12, 2);
        var locations = new List<Location>
    {
        new() { Id = 3, Name = "Citadel of Ricks", Type = "Space station", Dimension = "Unknown", Url = "http://example.com/loc3", Created = created },
        new() { Id = 4, Name = "Citadel of Ricks", Type = "Space station", Dimension = "Another Dimension", Url = "http://example.com/loc4", Created = created }
    };

        var factory = GetFactoryWithSeedData(locations);
        var apiMock = new Mock<IRickAndMortyApiService>();
        var service = new LocationService(factory, apiMock.Object, _mapper);

        var result = await service.GetAsync("Citadel of Ricks");

        result.Should().HaveCount(2);
        result.All(l => l.Name == "Citadel of Ricks").Should().BeTrue();
        result[0].Created.Should().Be(created);
        result[1].Created.Should().Be(created);
    }

    [Fact]
    public async Task AddAsync_ShouldAddLocation_WithAllProperties()
    {
        var created = new DateTime(2020, 12, 2);
        var dto = new NewApiLocationDto
        {
            Id = 5,
            Name = "Blips and Chitz",
            Type = "Arcade",
            Dimension = "Unknown",
            Url = "http://example.com/loc5",
            Created = created
        };

        var factory = GetFactoryWithSeedData([]);
        var apiMock = new Mock<IRickAndMortyApiService>();
        var service = new LocationService(factory, apiMock.Object, _mapper);

        await service.AddAsync(dto);
        var result = await service.GetAsync();

        result.Should().ContainSingle();
        var loc = result[0];
        loc.Id.Should().Be(5);
        loc.Name.Should().Be("Blips and Chitz");
        loc.Type.Should().Be("Arcade");
        loc.Dimension.Should().Be("Unknown");
        loc.Url.Should().Be("http://example.com/loc5");
        loc.Created.Should().Be(created);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveLocation()
    {
        var created = new DateTime(2020, 12, 2);
        var locations = new List<Location>
    {
        new() { Id = 6, Name = "Bird World", Type = "Planet", Dimension = "Bird Dimension", Url = "http://example.com/loc6", Created = created }
    };

        var factory = GetFactoryWithSeedData(locations);
        var apiMock = new Mock<IRickAndMortyApiService>();
        var service = new LocationService(factory, apiMock.Object, _mapper);

        await service.DeleteAsync(6);
        var result = await service.GetAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        var created = new DateTime(2020, 12, 2);
        var locations = new List<Location>
    {
        new() { Id = 7, Name = "Froopyland", Type = "Fantasy land", Dimension = "Unknown", Url = "http://example.com/loc7", Created = created },
        new() { Id = 8, Name = "Anatomy Park", Type = "Theme park", Dimension = "Human body", Url = "http://example.com/loc8", Created = created }
    };

        var factory = GetFactoryWithSeedData(locations);
        var apiMock = new Mock<IRickAndMortyApiService>();
        var service = new LocationService(factory, apiMock.Object, _mapper);

        var count = await service.CountAsync();
        count.Should().Be(2);
    }

    [Fact]
    public async Task GetAllFromApiAsync_ShouldFetchFromApi_AndSaveAllProperties()
    {
        var created = new DateTime(2020, 12, 2);
        var fakeDtos = new List<NewApiLocationDto>
    {
        new()
        {
            Id = 9,
            Name = "Nuptia 4",
            Type = "Planet",
            Dimension = "Romantic Dimension",
            Url = "http://api.com/loc9",
            Created = created
        }
    };

        var apiMock = new Mock<IRickAndMortyApiService>();
        apiMock.Setup(api => api.FetchAllEpisodesAsync<NewApiLocationDto>("api/location/"))
               .ReturnsAsync(fakeDtos);

        var factory = GetFactoryWithSeedData([]);
        var service = new LocationService(factory, apiMock.Object, _mapper);

        var count = await service.GetAllFromApiAsync();

        count.Should().Be(1);
        var result = await service.GetAsync();
        result.Should().ContainSingle();
        var loc = result[0];
        loc.Id.Should().Be(9);
        loc.Name.Should().Be("Nuptia 4");
        loc.Type.Should().Be("Planet");
        loc.Dimension.Should().Be("Romantic Dimension");
        loc.Url.Should().Be("http://api.com/loc9");
        loc.Created.Should().Be(created);
    }

}
