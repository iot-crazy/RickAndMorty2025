namespace RickAndMorty.DB.Models;

public class CharacterEpisode
{
    public int CharacterId { get; set; }
    public Character Character { get; set; } = default!;

    public int EpisodeId { get; set; }
    public Episode Episode { get; set; } = default!;
}
