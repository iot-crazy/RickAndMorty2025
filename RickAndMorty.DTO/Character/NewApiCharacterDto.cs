namespace RickAndMorty.DTO.Character;

public class NewApiCharacterDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Status { get; set; }
    public required string Type { get; set; }
    public required string Gender { get; set; }

    public NewLocationLinkDto Origin { get; set; } = default!;

    public NewLocationLinkDto Location { get; set; } = default!;
    public required string Image { get; set; }
    public required string Url { get; set; }
    public DateTime Created { get; set; }

    public List<string> Episode { get; set; } = [];
}
