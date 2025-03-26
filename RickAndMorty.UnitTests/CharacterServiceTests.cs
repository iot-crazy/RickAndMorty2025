using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using RickAndMorty.Contracts;
using RickAndMorty.DB;
using RickAndMorty.DB.Models;
using RickAndMorty.DTO.Character;
using RickAndMorty.Services;

namespace RickAndMorty.UnitTests;

public class CharacterServiceTests
{
    private readonly IMapper _mapper;
    private readonly DbContextOptions<RickAndMortyContext> _dbOptions;

    public CharacterServiceTests()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CharacterProfile>();
            cfg.AddProfile<LocationProfile>();
            cfg.AddProfile<EpisodeProfile>();
        });

        _mapper = config.CreateMapper();

        _dbOptions = new DbContextOptionsBuilder<RickAndMortyContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    private IRickAndMortyContextFactory GetFactoryWithSeedData(List<Character> seedCharacters)
    {
        var context = new RickAndMortyContext(_dbOptions);
        context.Characters.AddRange(seedCharacters);
        context.SaveChanges();

        var mockFactory = new Mock<IRickAndMortyContextFactory>();
        mockFactory.Setup(f => f.CreateContextAsync()).ReturnsAsync(context);
        return mockFactory.Object;
    }

    [Fact]
    public async Task GetAsync_ShouldReturnAllCharacters_WithAllProperties()
    {
        var created = new DateTime(2020, 12, 2);
        var characters = new List<Character>
        {
            new()
            {
                Id = 1,
                Name = "Rick Sanchez",
                Status = "Alive",
                Type = "Human",
                Gender = "Male",
                Origin = new Location
                        {
                            Id = 100,
                            Name = "Earth",
                            Type = "Planet",
                            Dimension = "Dimension C-137",
                            Url = "http://example.com/location/100",
                            Created = new DateTime(2020, 12, 2)
                        },
                Location = new Location
                        {
                            Id = 101,
                            Name = "Citadel of Ricks",
                            Type = "Space station",
                            Dimension = "Unknown",
                            Url = "http://example.com/location/101",
                            Created = new DateTime(2020, 12, 2)
                        },
                Image = "http://example.com/rick.png",
                Url = "http://example.com/character/1",
                Created = created
            }
        };

        var factory = GetFactoryWithSeedData(characters);
        var apiMock = new Mock<IRickAndMortyApiService>();
        var mockCacheInvalidator = new Mock<ICacheInvalidator>();
        var service = new CharacterService(factory, apiMock.Object, _mapper, mockCacheInvalidator.Object);

        var result = await service.GetAsync();

        result.Should().ContainSingle();
        var character = result[0];
        character.Id.Should().Be(1);
        character.Name.Should().Be("Rick Sanchez");
        character.Status.Should().Be("Alive");
        character.Type.Should().Be("Human");
        character.Gender.Should().Be("Male");
        character.Image.Should().Be("http://example.com/rick.png");
        character.Url.Should().Be("http://example.com/character/1");
        character.Created.Should().Be(created);
    }

    [Fact]
    public async Task Get_ById_ShouldReturnCharacter_WithAllProperties()
    {
        var created = new DateTime(2020, 12, 2);
        var characters = new List<Character>
        {
            new()
            {
                Id = 2,
                Name = "Morty Smith",
                Status = "Alive",
                Type = "Human",
                Gender = "Male",
                Origin = new Location
                        {
                            Id = 100,
                            Name = "Earth",
                            Type = "Planet",
                            Dimension = "Dimension C-137",
                            Url = "http://example.com/location/2",
                            Created = new DateTime(2020, 12, 2)
                        },
                Location = new Location
                        {
                            Id = 101,
                            Name = "Citadel of Ricks",
                            Type = "Space station",
                            Dimension = "Unknown",
                            Url = "http://example.com/location/101",
                            Created = new DateTime(2020, 12, 2)
                        },
                Image = "http://example.com/morty.png",
                Url = "http://example.com/character/2",
                Created = created
            }
        };

        var factory = GetFactoryWithSeedData(characters);
        var apiMock = new Mock<IRickAndMortyApiService>();
        var mockCacheInvalidator = new Mock<ICacheInvalidator>();
        var service = new CharacterService(factory, apiMock.Object, _mapper, mockCacheInvalidator.Object);

        var result = await service.GetAsync(2);

        result.Should().NotBeNull();
        result!.Id.Should().Be(2);
        result.Name.Should().Be("Morty Smith");
        result.Status.Should().Be("Alive");
        result.Type.Should().Be("Human");
        result.Gender.Should().Be("Male");
        result.Image.Should().Be("http://example.com/morty.png");
        result.Url.Should().Be("http://example.com/character/2");
        result.Created.Should().Be(created);
    }

    [Fact]
    public async Task Get_WithVariousFilters_ShouldReturnExpectedCharacters()
    {
        var created = new DateTime(2020, 12, 2);

        // Shared location instances
        var locationEarthC137 = new Location
        {
            Id = 1,
            Name = "Earth (C-137)",
            Type = "Planet",
            Dimension = "Dimension C-137",
            Url = "https://rickandmortyapi.com/api/location/1",
            Created = created
        };

        var locationEarthReplacement = new Location
        {
            Id = 20,
            Name = "Earth (Replacement Dimension)",
            Type = "Planet",
            Dimension = "Replacement Dimension",
            Url = "https://rickandmortyapi.com/api/location/20",
            Created = created
        };

        var locationCitadel = new Location
        {
            Id = 3,
            Name = "Citadel of Ricks",
            Type = "Space station",
            Dimension = "Unknown",
            Url = "https://rickandmortyapi.com/api/location/3",
            Created = created
        };

        var locationFroopyland = new Location
        {
            Id = 10,
            Name = "Froopyland",
            Type = "Artificial Dimension",
            Dimension = "Froopy Dimension",
            Url = "https://rickandmortyapi.com/api/location/10",
            Created = created
        };

        var locationBirdWorld = new Location
        {
            Id = 15,
            Name = "Bird World",
            Type = "Planet",
            Dimension = "Bird Dimension",
            Url = "https://rickandmortyapi.com/api/location/15",
            Created = created
        };

        // Characters using shared locations
        var characters = new List<Character>
{
    new()
    {
        Id = 1,
        Name = "Rick Sanchez",
        Status = "Alive",
        Gender = "Male",
        Type = "Human",
        Image = "https://rickandmortyapi.com/api/character/avatar/1.jpeg",
        Url = "https://rickandmortyapi.com/api/character/1",
        Origin = locationEarthC137,
        Location = locationCitadel,
        Created = created
    },
    new()
    {
        Id = 2,
        Name = "Morty Smith",
        Status = "Alive",
        Gender = "Male",
        Type = "Human",
        Image = "https://rickandmortyapi.com/api/character/avatar/2.jpeg",
        Url = "https://rickandmortyapi.com/api/character/2",
        Origin = locationEarthC137,
        Location = locationEarthReplacement,
        Created = created
    },
    new()
    {
        Id = 3,
        Name = "Summer Smith",
        Status = "Alive",
        Gender = "Female",
        Type = "Human",
        Image = "https://rickandmortyapi.com/api/character/avatar/3.jpeg",
        Url = "https://rickandmortyapi.com/api/character/3",
        Origin = locationEarthC137,
        Location = locationCitadel,
        Created = created
    },
    new()
    {
        Id = 4,
        Name = "Beth Smith",
        Status = "Alive",
        Gender = "Female",
        Type = "Human",
        Image = "https://rickandmortyapi.com/api/character/avatar/4.jpeg",
        Url = "https://rickandmortyapi.com/api/character/4",
        Origin = locationFroopyland,
        Location = locationEarthReplacement,
        Created = created
    },
    new()
    {
        Id = 5,
        Name = "Birdperson",
        Status = "Dead",
        Gender = "Male",
        Type = "Alien",
        Image = "https://rickandmortyapi.com/api/character/avatar/47.jpeg",
        Url = "https://rickandmortyapi.com/api/character/47",
        Origin = locationBirdWorld,
        Location = locationEarthReplacement,
        Created = created
    }
};


        var factory = GetFactoryWithSeedData(characters);
        var apiMock = new Mock<IRickAndMortyApiService>();
        var mockCacheInvalidator = new Mock<ICacheInvalidator>();
        var service = new CharacterService(factory, apiMock.Object, _mapper, mockCacheInvalidator.Object);

        var filters = new List<(CharacterFilter Filter, Func<CharacterDto, bool> Predicate)>
    {
        (new CharacterFilter { Name = "Rick Sanchez" }, c => c.Name == "Rick Sanchez"),
        (new CharacterFilter { Status = "Alive", Gender = "Female" }, c => c.Status == "Alive" && c.Gender == "Female"),
        (new CharacterFilter { Planet = "Citadel of Ricks" }, c => c.Location?.Name == "Citadel of Ricks"),
        (new CharacterFilter { Gender = "Male", Planet = "Earth" }, c => c.Gender == "Male" && c.Location?.Name == "Earth"),
        (new CharacterFilter { Status = "Dead" }, c => c.Status == "Dead"),
    };

        foreach (var (filter, predicate) in filters)
        {
            var result = await service.GetAsync(filter);
            result.Should().NotBeNull();
            result.All(predicate).Should().BeTrue($"Filter: {filter.ToQueryString()}");
        }
    }

    [Fact]
    public async Task AddAsync_ShouldAddCharacter_WithAllProperties()
    {
        var created = new DateTime(2020, 12, 2);
        var dto = new NewApiCharacterDto
        {
            Id = 5,
            Name = "Birdperson",
            Status = "Alive",
            Type = "Bird",
            Gender = "Male",
            Origin = new() { Name = "Bird World", Url = "http://example.com/locations/201" },
            Location = new() { Name = "Earth", Url = "http://example.com/locations/200" },
            Image = "http://example.com/birdperson.png",
            Url = "http://example.com/character/5",
            Created = created,
            Episode = ["http://example.com/episode/3", "http://example.com/episode/2"]
        };

        var factory = GetFactoryWithSeedData([]);
        var apiMock = new Mock<IRickAndMortyApiService>();
        var mockCacheInvalidator = new Mock<ICacheInvalidator>();
        var service = new CharacterService(factory, apiMock.Object, _mapper, mockCacheInvalidator.Object);

        await service.AddAsync(dto);
        var character = await service.GetAsync(5);

        character.Should().NotBeNull();
        character.Id.Should().Be(5);
        character.Name.Should().Be("Birdperson");
        character.Status.Should().Be("Alive");
        character.Type.Should().Be("Bird");
        character.Gender.Should().Be("Male");
        character.Image.Should().Be("http://example.com/birdperson.png");
        character.Url.Should().Be("http://example.com/character/5");
        character.Created.Should().Be(created);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveCharacter()
    {
        var created = new DateTime(2020, 12, 2);
        var characters = new List<Character>
        {
            new()
            {
                Id = 6,
                Name = "Squanchy",
                Status = "Alive",
                Type = "Cat-Person",
                Gender = "Male",
                Origin = new Location
                    {
                        Id = 100,
                        Name = "Earth",
                        Type = "Planet",
                        Dimension = "Dimension C-137",
                        Url = "http://example.com/location/100",
                        Created = new DateTime(2020, 12, 2)
                    },
                    Location = new Location
                    {
                        Id = 101,
                        Name = "Citadel of Ricks",
                        Type = "Space station",
                        Dimension = "Unknown",
                        Url = "http://example.com/locations/101",
                        Created = new DateTime(2020, 12, 2)
                    },
                                    Image = "http://example.com/squanchy.png",
                Url = "http://example.com/character/6",
                Created = created
            }
        };

        var factory = GetFactoryWithSeedData(characters);
        var apiMock = new Mock<IRickAndMortyApiService>();
        var mockCacheInvalidator = new Mock<ICacheInvalidator>();
        var service = new CharacterService(factory, apiMock.Object, _mapper, mockCacheInvalidator.Object);

        await service.DeleteAsync(6);
        var result = await service.GetAsync();

        result.Should().BeEmpty();
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        var created = new DateTime(2020, 12, 2);
        var characters = new List<Character>
{
    new()
    {
        Id = 7,
        Name = "Beth",
        Status = "Alive",
        Type = "Human",
        Gender = "Female",
        Origin = new Location
        {
            Id = 201,
            Name = "Earth",
            Type = "Planet",
            Dimension = "Dimension C-137",
            Url = "http://example.com/location/201",
            Created = created
        },
        Location = new Location
        {
            Id = 202,
            Name = "Earth Hospital",
            Type = "Hospital",
            Dimension = "Dimension C-137",
            Url = "http://example.com/location/202",
            Created = created
        },
        Image = "http://example.com/beth.png",
        Url = "http://example.com/character/7",
        Created = created
    },
    new()
    {
        Id = 8,
        Name = "Jerry",
        Status = "Alive",
        Type = "Human",
        Gender = "Male",
        Origin = new Location
        {
            Id = 203,
            Name = "Earth",
            Type = "Planet",
            Dimension = "Dimension C-137",
            Url = "http://example.com/location/203",
            Created = created
        },
        Location = new Location
        {
            Id = 204,
            Name = "Jerryboree",
            Type = "Daycare",
            Dimension = "Unknown",
            Url = "http://example.com/locations/204",
            Created = created
        },
        Image = "http://example.com/jerry.png",
        Url = "http://example.com/character/8",
        Created = created
    }
};


        var factory = GetFactoryWithSeedData(characters);
        var apiMock = new Mock<IRickAndMortyApiService>();
        var mockCacheInvalidator = new Mock<ICacheInvalidator>();
        var service = new CharacterService(factory, apiMock.Object, _mapper, mockCacheInvalidator.Object);

        var count = await service.CountAsync();

        count.Should().Be(2);
    }

    [Fact]
    public async Task GetAllFromApiAsync_ShouldFetch10Characters_AndSaveCorrectValues()
    {
        // Geneated by AI, I undertand it but I'd have not made it this clever because it feels like it needs a test of the test, but left it here as an example of what's possible
        var created = new DateTime(2020, 12, 2);
        var fakeDtos = Enumerable.Range(1, 10).Select(i => new NewApiCharacterDto
        {
            Id = i,
            Name = $"Character {i}",
            Status = i % 2 == 0 ? "Alive" : "Dead",
            Type = i % 3 == 0 ? "Alien" : "Human",
            Gender = i % 2 == 0 ? "Male" : "Female",
            Origin = new() { Name = $"Origin {i}", Url = $"http://example.com/location/{100 + i}" },
            Location = new() { Name = $"Location {i}", Url = $"http://example.com/location/{i}" },
            Image = $"http://example.com/image/{i}.png",
            Url = $"http://example.com/character/{i}",
            Created = created,
            Episode = [$"http://example.com/episode/{i}"]
        }).ToList();

        var apiMock = new Mock<IRickAndMortyApiService>();
        apiMock.Setup(api => api.FetchAllEpisodesAsync<NewApiCharacterDto>("api/character/?status=alive"))
               .ReturnsAsync(fakeDtos);

        var factory = GetFactoryWithSeedData([]);
        var mockCacheInvalidator = new Mock<ICacheInvalidator>();
        var service = new CharacterService(factory, apiMock.Object, _mapper, mockCacheInvalidator.Object);

        var count = await service.GetAllFromApiAsync();

        count.Should().Be(10);

        var result = await service.GetAsync();
        result.Should().HaveCount(10);

        // Validate character with ID = 1
        var char1 = result.FirstOrDefault(c => c.Id == 1);
        char1.Should().NotBeNull();
        char1!.Name.Should().Be("Character 1");
        char1.Status.Should().Be("Dead");
        char1.Type.Should().Be("Human");
        char1.Gender.Should().Be("Female");
        char1.Image.Should().Be("http://example.com/image/1.png");
        char1.Url.Should().Be("http://example.com/character/1");
        char1.Created.Should().Be(created);

        // Validate character with ID = 5
        var char5 = result.FirstOrDefault(c => c.Id == 5);
        char5.Should().NotBeNull();
        char5!.Name.Should().Be("Character 5");
        char5.Status.Should().Be("Dead");
        char5.Type.Should().Be("Human");
        char5.Gender.Should().Be("Female");
        char5.Image.Should().Be("http://example.com/image/5.png");
        char5.Url.Should().Be("http://example.com/character/5");
        char5.Created.Should().Be(created);
    }


}
