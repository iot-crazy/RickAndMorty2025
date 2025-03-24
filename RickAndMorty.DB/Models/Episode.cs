namespace RickAndMorty.DB.Models;

public class Episode
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public DateTime AirDate { get; set; }
    public required string Code { get; set; } // Renamed from "episode" to avoid naming conflict
    public required string Url { get; set; }
    public DateTime Created { get; set; }
    public List<CharacterEpisode> CharacterEpisodes { get; set; } = [];
}
