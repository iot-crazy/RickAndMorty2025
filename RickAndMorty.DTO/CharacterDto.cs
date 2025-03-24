namespace RickAndMorty.DTO;

public class CharacterDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Status { get; set; }
    public required string Type { get; set; }
    public required string Gender { get; set; }

    public LocationLinkDto Origin { get; set; } = default!;

    public LocationLinkDto Location { get; set; } = default!;
    public required string Image { get; set; }
    public required string Url { get; set; }
    public DateTime Created { get; set; }

    public List<string> Episode { get; set; } = [];
}
