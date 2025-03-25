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
        var service = new CharacterService(factory, apiMock.Object, _mapper);

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
        var service = new CharacterService(factory, apiMock.Object, _mapper);

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
    public async Task Get_ByName_ShouldReturnCharacters_WithAllProperties()
    {
        var created = new DateTime(2020, 12, 2);
        var characters = new List<Character>
        {
            new()
            {
                Id = 3,
                Name = "Summer Smith",
                Status = "Alive",
                Type = "Human",
                Gender = "Female",
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
                Image = "http://example.com/summer.png",
                Url = "http://example.com/character/3",
                Created = created
            },
            new()
            {
                Id = 4,
                Name = "Summer Smith",
                Status = "Alive",
                Type = "Clone",
                Gender = "Female",
                               Origin = new Location
                        {
                            Id = 102,
                            Name = "Earth",
                            Type = "Planet",
                            Dimension = "Dimension C-137",
                            Url = "http://example.com/locations/102",
                            Created = new DateTime(2020, 12, 2)
                        },
                Location = new Location
                        {
                            Id = 103,
                            Name = "Citadel of Ricks",
                            Type = "Space station",
                            Dimension = "Unknown",
                            Url = "http://example.com/locations/103",
                            Created = new DateTime(2020, 12, 2)
                        },
                Image = "http://example.com/summer2.png",
                Url = "http://example.com/character/2",
                Created = created
            }
        };

        var factory = GetFactoryWithSeedData(characters);
        var apiMock = new Mock<IRickAndMortyApiService>();
        var service = new CharacterService(factory, apiMock.Object, _mapper);

        var result = await service.GetAsync("Summer Smith");

        result.Should().HaveCount(2);
        result.All(c => c.Name == "Summer Smith").Should().BeTrue();
        result.All(c => c.Created == created).Should().BeTrue();
    }

    [Fact]
    public async Task AddAsync_ShouldAddCharacter_WithAllProperties()
    {
        var created = new DateTime(2020, 12, 2);
        var dto = new NewCharacterDto
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
        var service = new CharacterService(factory, apiMock.Object, _mapper);

        await service.AddAsync(dto);
        var result = await service.GetAsync();

        result.Should().ContainSingle();
        var character = result[0];
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
        var service = new CharacterService(factory, apiMock.Object, _mapper);

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
        var service = new CharacterService(factory, apiMock.Object, _mapper);

        var count = await service.CountAsync();

        count.Should().Be(2);
    }

    [Fact]
    public async Task GetAllFromApiAsync_ShouldFetchFromApi_AndSaveAllProperties()
    {
        var created = new DateTime(2020, 12, 2);
        var fakeDtos = new List<NewCharacterDto>
        {
            new()
            {
                Id = 9,
                Name = "Unity",
                Status = "Alive",
                Type = "Hive-mind",
                Gender = "Genderless",
                Origin = new() { Name = "System A113", Url = "http://example.com/location/113" },
                Location = new() { Name = "Earth", Url = "http://example.com/location/1" },
                Image = "http://example.com/unity.png",
                Url = "http://example.com/character/9",
                Created = created,
                Episode = ["http://example.com/episode/3"]
            }
        };

        var apiMock = new Mock<IRickAndMortyApiService>();
        apiMock.Setup(api => api.FetchAllEpisodesAsync<NewCharacterDto>("api/character/?status=alive"))
               .ReturnsAsync(fakeDtos);

        var factory = GetFactoryWithSeedData([]);
        var service = new CharacterService(factory, apiMock.Object, _mapper);

        var count = await service.GetAllFromApiAsync();

        count.Should().Be(1);
        var result = await service.GetAsync();
        result.Should().ContainSingle();
        var character = result[0];
        character.Id.Should().Be(9);
        character.Name.Should().Be("Unity");
        character.Status.Should().Be("Alive");
        character.Type.Should().Be("Hive-mind");
        character.Gender.Should().Be("Genderless");
        character.Image.Should().Be("http://example.com/unity.png");
        character.Url.Should().Be("http://example.com/character/9");
        character.Created.Should().Be(created);
    }
}
