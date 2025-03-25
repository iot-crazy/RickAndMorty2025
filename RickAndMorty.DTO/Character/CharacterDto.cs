namespace RickAndMorty.DTO.Character;

public class CharacterDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Status { get; set; }
    public required string Type { get; set; }
    public required string Gender { get; set; }

    //  public LocationDto Origin { get; set; } = default!;

    //  public LocationDto Location { get; set; } = default!;
    public required string Image { get; set; }
    public required string Url { get; set; }
    public DateTime Created { get; set; }

    // public List<EpisodeDto> Episodes { get; set; } = [];
}
