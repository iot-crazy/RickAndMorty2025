
namespace RickAndMorty.DB.Models;

public class Character
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Status { get; set; }
    public required string Type { get; set; }
    public required string Gender { get; set; }
    public required Location? Origin { get; set; }
    public int? OriginId { get; set; }
    public required Location? Location { get; set; }
    public int? LocationId { get; set; }
    public required string Image { get; set; }
    public required string Url { get; set; }
    public DateTime Created { get; set; }
    public List<CharacterEpisode> CharacterEpisodes { get; set; } = [];
}
