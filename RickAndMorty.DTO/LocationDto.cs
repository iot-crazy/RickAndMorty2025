namespace RickAndMorty.DTO;

public class LocationDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Type { get; set; }
    public required string Dimension { get; set; }
    public required string Url { get; set; }
    public DateTime Created { get; set; }
}
